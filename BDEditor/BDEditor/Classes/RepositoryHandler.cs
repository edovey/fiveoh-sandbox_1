using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Amazon;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Amazon.S3;
using Amazon.S3.Model;

using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public class RepositoryHandler
    {
        private const string BD_ACCESS_KEY = @"AKIAJ6SRLQLH2ALT7ZBQ";
        private const string BD_SECRET_KEY = @"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";

        static private readonly RepositoryHandler aws = new RepositoryHandler();

        private AmazonSimpleDB simpleDb = null;
        private AmazonS3 s3 = null;

        private List<SyncInfo> syncInfoList = new List<SyncInfo>();

        private RepositoryHandler() { }

        static public RepositoryHandler Aws
        {
            get { return aws; }
        }

        public AmazonSimpleDB SimpleDb
        {
            get
            { 
                if (null == simpleDb) { simpleDb = AWSClientFactory.CreateAmazonSimpleDBClient(BD_ACCESS_KEY, BD_SECRET_KEY); }
                return simpleDb;
            }
        }

        public AmazonS3 S3
        {
            get
            {
                if (null == s3) { s3 = Amazon.AWSClientFactory.CreateAmazonS3Client(BD_ACCESS_KEY, BD_SECRET_KEY); }
                return s3;
            }
        }

        private SyncInfoDictionary InitializeSyncDictionary(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate, Boolean pCreateMissing, BDConstants.SyncType pSyncType)
        {
            SyncInfoDictionary syncDictionary = new SyncInfoDictionary();

            if (pSyncType == BDConstants.SyncType.Default)
            {
                // Create the SyncInfo instance and update the modified date of all the changed records to be the currentSyncDate
                syncDictionary.Add(BDNode.SyncInfo(pDataContext, pLastSyncDate, pCurrentSyncDate));
                syncDictionary.Add(BDNodeAssociation.SyncInfo(pDataContext, pLastSyncDate, pCurrentSyncDate));

                // It is relevant that LinkedNoteAssociation is BEFORE LinkedNote
                syncDictionary.Add(BDLinkedNoteAssociation.SyncInfo(pDataContext, pLastSyncDate, pCurrentSyncDate));
                syncDictionary.Add(BDLinkedNote.SyncInfo(pDataContext, pLastSyncDate, pCurrentSyncDate));

                syncDictionary.Add(BDTherapy.SyncInfo(pDataContext, pLastSyncDate, pCurrentSyncDate));
                syncDictionary.Add(BDTherapyGroup.SyncInfo(pDataContext, pLastSyncDate, pCurrentSyncDate));
                syncDictionary.Add(BDDeletion.SyncInfo(pDataContext, pLastSyncDate, pCurrentSyncDate));
                syncDictionary.Add(BDMetadata.SyncInfo(pDataContext, pLastSyncDate, pCurrentSyncDate));
            }

            if (pSyncType == BDConstants.SyncType.Publish)
            {
                syncDictionary.Add(BDHtmlPage.SyncInfo(pDataContext, pLastSyncDate, pCurrentSyncDate));
                syncDictionary.Add(BDSearchEntry.SyncInfo(pDataContext, pLastSyncDate, pCurrentSyncDate));
                syncDictionary.Add(BDSearchEntryAssociation.SyncInfo(pDataContext, pLastSyncDate, pCurrentSyncDate));
            }

            // List the remote domains
            ListDomainsResponse sdbListDomainsResponse = SimpleDb.ListDomains(new ListDomainsRequest());
            if (sdbListDomainsResponse.IsSetListDomainsResult())
            {
                ListDomainsResult listDomainsResult = sdbListDomainsResponse.ListDomainsResult;
                foreach (String domainName in listDomainsResult.DomainName)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Found domain: {0}", domainName));
                    if (syncDictionary.ContainsKey(domainName))
                        syncDictionary[domainName].ExistsOnRemote = true;
                }
            }

            if (pCreateMissing)
            {
                // Create missing domains
                foreach (SyncInfo syncInfoEntry in syncDictionary.Values)
                {
                    if (!syncInfoEntry.ExistsOnRemote)
                    {
                        try
                        {
                            CreateDomainRequest createDomainRequest = (new CreateDomainRequest()).WithDomainName(syncInfoEntry.RemoteEntityName);
                            CreateDomainResponse createResponse = simpleDb.CreateDomain(createDomainRequest);
                            syncInfoEntry.ExistsOnRemote = true;
                            System.Diagnostics.Debug.WriteLine(string.Format("Created domain for {0}", syncInfoEntry.RemoteEntityName));
                        }
                        catch (AmazonSimpleDBException ex)
                        {
                            syncInfoEntry.Exception = ex;
                            System.Diagnostics.Debug.WriteLine(string.Format("Failed to created domain for {0}", syncInfoEntry.RemoteEntityName));
                        }
                    }
                }
            }

            return syncDictionary;
        }

        /// <summary>
        /// Synchronize with the Amazon SimpleDb
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pLastSyncDate"></param>
        /// <returns></returns>
        public SyncInfoDictionary Sync(Entities pDataContext, DateTime? pLastSyncDate, BDConstants.SyncType pSyncType)
        {
            DateTime? currentSyncDate = DateTime.Now;
            #region Initialize Sync

            SyncInfoDictionary syncDictionary = InitializeSyncDictionary(pDataContext, pLastSyncDate, currentSyncDate, true, pSyncType);

            // Create the SyncInfo instance and update the modified date of all the changed records to be the currentSyncDate

            // prepare the push changes

            #endregion

            #region Pull

            syncDictionary = Pull(pDataContext, pLastSyncDate, syncDictionary);

            #endregion

            // Process all deletion records in database since last sync: will include records received in last pull
            BDDeletion.DeleteLocalSinceDate(pDataContext, pLastSyncDate);

            if (null != pLastSyncDate)
            {
                #region Push

                foreach (SyncInfo syncInfoEntry in syncDictionary.Values)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Push {0}", syncInfoEntry.RemoteEntityName));
                    foreach (IBDObject changeEntry in syncInfoEntry.PushList)
                    {
                        SimpleDb.PutAttributes(changeEntry.PutAttributes());
                        syncInfoEntry.RowsPushed++;

                        if (changeEntry is BDLinkedNote)
                        {
                            BDLinkedNote linkedNote = changeEntry as BDLinkedNote;
                            PutObjectRequest putObjectRequest = new PutObjectRequest()
                                        .WithContentType(@"text/plain")
                                        .WithContentBody(linkedNote.documentText)
                                        .WithBucketName(BDLinkedNote.AWS_BUCKET)
                                        .WithKey(linkedNote.storageKey);

                            S3Response s3Response = S3.PutObject(putObjectRequest);
                            s3Response.Dispose();
                        }

                        if (changeEntry is BDHtmlPage)
                        {
                            BDHtmlPage htmlPage = changeEntry as BDHtmlPage;
                            PutObjectRequest putObjectRequest = new PutObjectRequest()
                                        .WithContentType(@"text/html")
                                        .WithContentBody(htmlPage.documentText)
                                        .WithBucketName(BDHtmlPage.AWS_BUCKET)
                                        .WithKey(htmlPage.storageKey);

                            S3Response s3Response = S3.PutObject(putObjectRequest);
                            s3Response.Dispose();
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Pushed {0} Records for {1}", syncInfoEntry.RowsPushed, syncInfoEntry.RemoteEntityName);
                }
            #endregion

                #region Delete Remote
                // Process all deletion records in database since last sync: will include records received in last pull
                List<IBDObject> newDeletionsForRemote = BDDeletion.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                string domainName = "";
                foreach (IBDObject entryObject in newDeletionsForRemote)
                {
                    BDDeletion entry = entryObject as BDDeletion;
                    switch(entry.targetName)
                    {
                        case BDNode.KEY_NAME:
                            domainName = BDNode.AWS_DOMAIN;
                            break;
                        case BDNodeAssociation.KEY_NAME:
                            domainName = BDNodeAssociation.AWS_DOMAIN;
                            break;
                        case BDLinkedNote.KEY_NAME:
                            domainName = BDLinkedNote.AWS_DOMAIN;
                            DeleteObjectRequest s3DeleteRequest = new DeleteObjectRequest()
                                .WithBucketName(BDLinkedNote.AWS_BUCKET)
                                .WithKey(BDLinkedNote.GenerateStorageKey(entry.targetId.Value));
                            try
                            {
                                S3.DeleteObject(s3DeleteRequest);
                            }
                            catch (AmazonS3Exception s3Exception)
                            {
                                Console.WriteLine(s3Exception.Message, s3Exception.InnerException);
                            }

                            break;
                        case BDLinkedNoteAssociation.KEY_NAME:
                            domainName = BDLinkedNoteAssociation.AWS_DOMAIN;
                            break;
                        case BDTherapy.KEY_NAME:
                            domainName = BDTherapy.AWS_DOMAIN;
                            break;
                        case BDTherapyGroup.KEY_NAME:
                            domainName = BDTherapyGroup.AWS_DOMAIN;
                            break;
                        case BDSearchEntry.KEY_NAME:
                            domainName = BDSearchEntry.AWS_DOMAIN;
                            break;
                        case BDSearchEntryAssociation.KEY_NAME:
                            domainName = BDSearchEntryAssociation.AWS_DOMAIN;
                            break;
                        case BDMetadata.KEY_NAME:
                            domainName = BDMetadata.AWS_DOMAIN;
                            break;
                        case BDHtmlPage.KEY_NAME:
                            domainName = BDLinkedNote.AWS_DOMAIN;
                            DeleteObjectRequest s3DeleteHtmlPage = new DeleteObjectRequest()
                                .WithBucketName(BDHtmlPage.AWS_BUCKET)
                                .WithKey(BDHtmlPage.GenerateStorageKey(entry.targetId.Value));
                            try
                            {
                                S3.DeleteObject(s3DeleteHtmlPage);
                            }
                            catch (AmazonS3Exception s3Exception)
                            {
                                Console.WriteLine(s3Exception.Message, s3Exception.InnerException);
                            }
                            break;
                    }

                    DeleteAttributesRequest request = new DeleteAttributesRequest().WithDomainName(domainName).WithItemName(entry.targetId.Value.ToString().ToUpper());
                    SimpleDb.DeleteAttributes(request);
                }
                #endregion

            }

            pLastSyncDate = currentSyncDate;

            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(pDataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            systemSetting.settingDateTimeValue = currentSyncDate;
            pDataContext.SaveChanges();

            return syncDictionary;
        }

        public SyncInfoDictionary Pull(Entities pDataContext, DateTime? pLastSyncDate, SyncInfoDictionary pSyncDictionary)
        {
            BDCommon.Settings.IsSyncLoad = true;

            foreach (SyncInfo syncInfoEntry in pSyncDictionary.Values)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Pull {0}", syncInfoEntry.RemoteEntityName));
                SelectRequest selectRequestAction = new SelectRequest().WithSelectExpression(syncInfoEntry.GetLatestRemoteSelectString(pLastSyncDate));

                SelectResponse selectResponse = null;

                do
                {
                    selectResponse = simpleDb.Select(selectRequestAction);

                    if (selectResponse.IsSetSelectResult())
                    {
                        SelectResult selectResult = selectResponse.SelectResult;
                        foreach (Item item in selectResult.Item)
                        {
                            Guid? entryGuid = null;
                            syncInfoEntry.RowsPulled++;
                            AttributeDictionary attributeDictionary = ItemAttributesToDictionary(item);
                            switch (syncInfoEntry.RemoteEntityName)
                            {
                                case BDDeletion.AWS_DOMAIN:
                                    entryGuid = BDDeletion.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDLinkedNoteAssociation.AWS_DOMAIN:
                                    BDLinkedNoteAssociation.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDLinkedNote.AWS_DOMAIN:
                                    {
                                        entryGuid = BDLinkedNote.LoadFromAttributes(pDataContext, attributeDictionary, true); // We need the note for the S3 call, so save
                                        if (null != entryGuid) // retrieve the note body
                                        {
                                            BDLinkedNote note = BDLinkedNote.GetLinkedNoteWithId(pDataContext, entryGuid);
                                            if (null != note)
                                            {
                                                try
                                                {
                                                    GetObjectRequest getObjectRequest = new GetObjectRequest()
                                                        .WithBucketName(BDLinkedNote.AWS_BUCKET)
                                                        .WithKey(note.storageKey);

                                                    using (GetObjectResponse response = S3.GetObject(getObjectRequest))
                                                    {
                                                        if ((response.ContentType == @"text/html") || (response.ContentType == @"text/plain"))
                                                        {
                                                            using (StreamReader reader = new StreamReader(response.ResponseStream))
                                                            {
                                                                String encodedString = reader.ReadToEnd();
                                                                String unencodedString = System.Net.WebUtility.HtmlDecode(encodedString);
                                                                //note.documentText = unencodedString;
                                                                note.documentText = encodedString;
                                                            }
                                                        }

                                                    }
                                                }
                                                catch (AmazonS3Exception amazonS3Exception)
                                                {
                                                    if (amazonS3Exception.ErrorCode != null &&
                                                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                                                        amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                                                    {
                                                        Console.WriteLine("Please check the provided AWS Credentials.");
                                                        Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("An error occurred with the message '{0}' when reading an object", amazonS3Exception.Message);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case BDTherapy.AWS_DOMAIN:
                                    entryGuid = BDTherapy.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDTherapyGroup.AWS_DOMAIN:
                                    entryGuid = BDTherapyGroup.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDMetadata.AWS_DOMAIN:
                                    entryGuid = BDMetadata.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDSearchEntry.AWS_DOMAIN:
                                    entryGuid = BDSearchEntry.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDSearchEntryAssociation.AWS_DOMAIN:
                                    entryGuid = BDSearchEntryAssociation.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDNode.AWS_DOMAIN:
                                    entryGuid = BDNode.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDNodeAssociation.AWS_DOMAIN:
                                    entryGuid = BDNodeAssociation.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDHtmlPage.AWS_DOMAIN:
                                    {
                                        entryGuid = BDHtmlPage.LoadFromAttributes(pDataContext, attributeDictionary, true); // We need the note for the S3 call, so save
                                        if (null != entryGuid) // retrieve the note body
                                        {
                                            BDHtmlPage page = BDHtmlPage.RetrieveWithId(pDataContext, entryGuid);
                                            if (null != page)
                                            {
                                                try
                                                {
                                                    GetObjectRequest getObjectRequest = new GetObjectRequest()
                                                        .WithBucketName(BDHtmlPage.AWS_BUCKET)
                                                        .WithKey(page.storageKey);

                                                    using (GetObjectResponse response = S3.GetObject(getObjectRequest))
                                                    {
                                                        if ((response.ContentType == @"text/html") || (response.ContentType == @"text/plain"))
                                                        {
                                                            using (StreamReader reader = new StreamReader(response.ResponseStream))
                                                            {
                                                                String encodedString = reader.ReadToEnd();
                                                                page.documentText = encodedString;
                                                            }
                                                        }

                                                    }
                                                }
                                                catch (AmazonS3Exception amazonS3Exception)
                                                {
                                                    if (amazonS3Exception.ErrorCode != null &&
                                                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                                                        amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                                                    {
                                                        Console.WriteLine("Please check the provided AWS Credentials.");
                                                        Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("An error occurred with the message '{0}' when reading an object", amazonS3Exception.Message);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                            // The entry id will be null if a sync conflict prevented create/update so add it to the conflict list
                            if (null == entryGuid) syncInfoEntry.SyncConflictList.Add(attributeDictionary);
                        }
                        pDataContext.SaveChanges();
                    }

                    if (selectResponse.SelectResult.IsSetNextToken())
                    {
                        selectRequestAction.NextToken = selectResponse.SelectResult.NextToken;
                    }

                    System.Diagnostics.Debug.WriteLine("Pulled {0} Records for {1}", syncInfoEntry.RowsPulled, syncInfoEntry.RemoteEntityName);

                } while (selectResponse.SelectResult.IsSetNextToken());
            }

            BDCommon.Settings.IsSyncLoad = false;

            return pSyncDictionary;
        }

        public SyncInfoDictionary ImportFromProduction(Entities pDataContext, DateTime? pLastSyncDate)
        {
            DateTime? currentSyncDate = null; // DateTime.Now;

            SyncInfoDictionary syncDictionary = InitializeSyncDictionary(pDataContext, pLastSyncDate, currentSyncDate, false, BDConstants.SyncType.Default);

            BDCommon.Settings.IsSyncLoad = true;

            #region Pull from Prod

            foreach (SyncInfo syncInfoEntry in syncDictionary.Values)
            {
                System.Diagnostics.Debug.WriteLine(syncInfoEntry.FriendlyName);
                if (!syncInfoEntry.ExistsOnRemote) continue;

                System.Diagnostics.Debug.WriteLine(string.Format("Production Pull {0}", syncInfoEntry.RemoteProductionEntityName));
                SelectRequest selectRequestAction = new SelectRequest().WithSelectExpression(syncInfoEntry.GetLatestRemoteSelectString(pLastSyncDate, syncInfoEntry.RemoteProductionEntityName));

                SelectResponse selectResponse = null;

                do
                {
                    selectResponse = simpleDb.Select(selectRequestAction);

                    if (selectResponse.IsSetSelectResult())
                    {
                        SelectResult selectResult = selectResponse.SelectResult;
                        foreach (Item item in selectResult.Item)
                        {
                            Guid? entryGuid = null;
                            syncInfoEntry.RowsPulled++;
                            AttributeDictionary attributeDictionary = ItemAttributesToDictionary(item);
                            switch (syncInfoEntry.RemoteProductionEntityName)
                            {
                                case BDDeletion.AWS_PROD_DOMAIN:
                                    entryGuid = BDDeletion.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDLinkedNoteAssociation.AWS_PROD_DOMAIN:
                                    BDLinkedNoteAssociation.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDLinkedNote.AWS_PROD_DOMAIN:
                                    {

                                        //TODO: Get the existing and create a new note within the dev environment.
                                        // change all the association records.
                                        // NOTE: LinkedNoteAssociations MUST be loaded first.

                                        BDLinkedNote note = BDLinkedNote.CreateFromProdWithAttributes(pDataContext, attributeDictionary);

                                        if (null != note)
                                        {
                                            try
                                            {
                                                GetObjectRequest getObjectRequest = new GetObjectRequest()
                                                    .WithBucketName(BDLinkedNote.AWS_PROD_BUCKET)
                                                    .WithKey(note.storageKey); // This will be the "original" storage key

                                                using (GetObjectResponse response = S3.GetObject(getObjectRequest))
                                                {
                                                    if ((response.ContentType == @"text/html") || (response.ContentType == @"text/plain"))
                                                    {
                                                        using (StreamReader reader = new StreamReader(response.ResponseStream))
                                                        {
                                                            String encodedString = reader.ReadToEnd();
                                                            String unencodedString = System.Net.WebUtility.HtmlDecode(encodedString);
                                                            note.documentText = encodedString;
                                                        }
                                                    }
                                                }
                                            }
                                            catch (AmazonS3Exception amazonS3Exception)
                                            {
                                                if (amazonS3Exception.ErrorCode != null &&
                                                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                                                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                                                {
                                                    Console.WriteLine("Please check the provided AWS Credentials.");
                                                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("An error occurred with the message '{0}' when reading an object", amazonS3Exception.Message);
                                                }
                                            }

                                            note.storageKey = BDLinkedNote.GenerateStorageKey(note); // update the storage key with the new uuid.

                                            Guid originalLinkedNoteUuid = note.tempProductionUuid.Value;

                                            List<BDLinkedNoteAssociation> associationList = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForLinkedNoteId(pDataContext, originalLinkedNoteUuid);
                                            foreach (BDLinkedNoteAssociation association in associationList)
                                            {
                                                DateTime? modifiedDate = association.modifiedDate;
                                                association.linkedNoteId = note.Uuid;
                                                association.modifiedDate = modifiedDate; // reset the modified date because the above change will automatically update it
                                            }
                                            //pDataContext.ExecuteStoreCommand("UPDATE BDLinkedNoteAssociations SET linkedNoteId = {0} WHERE linkedNoteId = {1}", note.uuid, originalLinkedNoteUuid);
                                            pDataContext.SaveChanges();
                                            // This expects that the LinkedNoteAssociation data has already been loaded
                                        }

                                    }
                                    break;
                                case BDTherapy.AWS_PROD_DOMAIN:
                                    entryGuid = BDTherapy.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDTherapyGroup.AWS_PROD_DOMAIN:
                                    entryGuid = BDTherapyGroup.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDMetadata.AWS_PROD_DOMAIN:
                                    entryGuid = BDMetadata.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDSearchEntry.AWS_PROD_DOMAIN:
                                    entryGuid = BDSearchEntry.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDSearchEntryAssociation.AWS_PROD_DOMAIN:
                                    entryGuid = BDSearchEntryAssociation.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDNode.AWS_PROD_DOMAIN:
                                    entryGuid = BDNode.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDNodeAssociation.AWS_PROD_DOMAIN:
                                    entryGuid = BDNodeAssociation.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                            }
                            // The entry id will be null if a sync conflict prevented create/update so add it to the conflict list
                            if (null == entryGuid) syncInfoEntry.SyncConflictList.Add(attributeDictionary);
                        }
                        pDataContext.SaveChanges();
                    }

                    if (selectResponse.SelectResult.IsSetNextToken())
                    {
                        selectRequestAction.NextToken = selectResponse.SelectResult.NextToken;
                    }

                    System.Diagnostics.Debug.WriteLine("Pulled {0} Production Records for {1}", syncInfoEntry.RowsPulled, syncInfoEntry.RemoteProductionEntityName);

                } while (selectResponse.SelectResult.IsSetNextToken());
            }
            
            BDCommon.Settings.IsSyncLoad = false;
            
            #endregion

            #region Push to dev

            // reload the push lists
            syncDictionary = InitializeSyncDictionary(pDataContext, null, null, false, BDConstants.SyncType.Default);
            
            BDCommon.Settings.IsSyncLoad = true;
            foreach (SyncInfo syncInfoEntry in syncDictionary.Values)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Push {0}", syncInfoEntry.RemoteEntityName));
                foreach (IBDObject changeEntry in syncInfoEntry.PushList)
                {
                    SimpleDb.PutAttributes(changeEntry.PutAttributes());
                    syncInfoEntry.RowsPushed++;

                    if (changeEntry is BDLinkedNote)
                    {
                        BDLinkedNote linkedNote = changeEntry as BDLinkedNote;
                        PutObjectRequest putObjectRequest = new PutObjectRequest()
                                    .WithContentType(@"text/plain")
                                    .WithContentBody(linkedNote.documentText)
                                    .WithBucketName(BDLinkedNote.AWS_BUCKET)
                                    .WithKey(linkedNote.storageKey);

                        S3Response s3Response = S3.PutObject(putObjectRequest);
                        s3Response.Dispose();
                    }
                }

                System.Diagnostics.Debug.WriteLine("Pushed {0} Records for {1}", syncInfoEntry.RowsPushed, syncInfoEntry.RemoteEntityName);
            }

            BDCommon.Settings.IsSyncLoad = false;
            #endregion

            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(pDataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            systemSetting.settingDateTimeValue = DateTime.Now;
            pDataContext.SaveChanges();

           

            return syncDictionary;
        }

        public void DeleteLocalData(Entities pDataContext)
        {
            pDataContext.ExecuteStoreCommand("DELETE FROM BDNodes");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDNodeAssociations");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDDeletions");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDLinkedNoteAssociations");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDLinkedNotes");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDTherapies");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDTherapyGroups");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDSearchEntryAssociations");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDSearchEntries");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDMetata");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDHtmlPage");
        }

        #region Helper Methods

        private AttributeDictionary ItemAttributesToDictionary(Amazon.SimpleDB.Model.Item pItem)
        {
            AttributeDictionary attributeDictionary = new AttributeDictionary();

            if (null != pItem)
            {
                foreach (Amazon.SimpleDB.Model.Attribute attribute in pItem.Attribute)
                {
                    attributeDictionary.Add(attribute);
                }
            }

            return attributeDictionary;
        }
        #endregion

        public void DeleteRemoteForSearch()
        {
            DeleteDomainRequest saRequest = new DeleteDomainRequest().WithDomainName(BDSearchEntry.AWS_DOMAIN);
            SimpleDb.DeleteDomain(saRequest);
            DeleteDomainRequest seRequest = new DeleteDomainRequest().WithDomainName(BDSearchEntryAssociation.AWS_DOMAIN);
            SimpleDb.DeleteDomain(seRequest);
        }

        public void DeleteRemoteForPages(Entities pContext)
        {
            DeleteObjectsRequest s3DeleteHtmlPages = new DeleteObjectsRequest()
                .WithBucketName(BDHtmlPage.AWS_BUCKET);

            List<BDHtmlPage> allPages = BDHtmlPage.RetrieveAll(pContext);
            foreach (BDHtmlPage page in allPages)
            {
                s3DeleteHtmlPages.AddKey(page.storageKey);
            }
            if (s3DeleteHtmlPages.Keys.Count > 0)
            {
                try
                {
                    S3.DeleteObjects(s3DeleteHtmlPages);
                }
                catch (DeleteObjectsException e)
                {
                    var errorResponse = e.ErrorResponse;
                    Console.WriteLine("No. of objects successfully deleted = {0}", errorResponse.DeletedObjects.Count);
                    Console.WriteLine("No. of objects failed to delete = {0}", errorResponse.DeleteErrors.Count);
                    Console.WriteLine("Printing error data...");
                    foreach (DeleteError deleteError in errorResponse.DeleteErrors)
                    {
                        Console.WriteLine("Object Key: {0}\t{1}\t{2}", deleteError.Key, deleteError.Code, deleteError.Message);
                    }
                }
            }
            DeleteDomainRequest htDomain = new DeleteDomainRequest().WithDomainName(BDHtmlPage.AWS_DOMAIN);
            SimpleDb.DeleteDomain(htDomain);
        }
    }
}
