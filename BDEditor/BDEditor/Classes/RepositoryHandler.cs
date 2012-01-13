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

        /// <summary>
        /// Synchronize with the Amazon SimpleDb
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pLastSyncDate"></param>
        /// <returns></returns>
        public SyncInfoDictionary Sync(Entities pDataContext, DateTime? pLastSyncDate)
        {
            DateTime currentSyncDate = DateTime.Now;
            #region Initialize Sync
            SyncInfoDictionary syncDictionary = new SyncInfoDictionary();

            // Create the SyncInfo instance and update the modified date of all the changed records to be the currentSyncDate
            syncDictionary.Add(BDCategory.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDChapter.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDDisease.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDLinkedNoteAssociation.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDLinkedNote.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDPathogenGroup.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDPathogen.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDPresentation.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDSection.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDSubcategory.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDTherapy.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDTherapyGroup.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDDeletion.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDSearchEntry.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDSearchEntryAssociation.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));
            syncDictionary.Add(BDMetadata.SyncInfo(pDataContext, pLastSyncDate, currentSyncDate));

            // prepare the push changes


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

            #endregion

            #region Pull
            Common.Settings.IsSyncLoad = true;

            foreach (SyncInfo syncInfoEntry in syncDictionary.Values)
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
                                case BDCategory.AWS_DOMAIN:
                                    entryGuid = BDCategory.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDChapter.AWS_DOMAIN:
                                    entryGuid = BDChapter.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDDisease.AWS_DOMAIN:
                                    entryGuid = BDDisease.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
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
                                case BDPathogenGroup.AWS_DOMAIN:
                                    entryGuid = BDPathogenGroup.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDPathogen.AWS_DOMAIN:
                                    entryGuid = BDPathogen.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDPresentation.AWS_DOMAIN:
                                    entryGuid = BDPresentation.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDSection.AWS_DOMAIN:
                                    BDSection.LoadFromAttributes(pDataContext, attributeDictionary, false);
                                    break;
                                case BDSubcategory.AWS_DOMAIN:
                                    entryGuid = BDSubcategory.LoadFromAttributes(pDataContext, attributeDictionary, false);
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

            Common.Settings.IsSyncLoad = false;

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
                        case BDCategory.KEY_NAME:
                            domainName = BDCategory.AWS_DOMAIN;
                            break;
                        case BDChapter.KEY_NAME:
                            domainName = BDChapter.AWS_DOMAIN;
                            break;
                        case BDDisease.KEY_NAME:
                            domainName = BDDisease.AWS_DOMAIN;
                            break;
                        case BDLinkedNote.KEY_NAME:
                            domainName = BDLinkedNote.AWS_DOMAIN;
                            break;
                        case BDLinkedNoteAssociation.KEY_NAME:
                            domainName = BDLinkedNoteAssociation.AWS_DOMAIN;
                            break;
                        case BDPathogen.KEY_NAME:
                            domainName = BDPathogen.AWS_DOMAIN;
                            break;
                        case BDPathogenGroup.KEY_NAME:
                            domainName = BDPathogenGroup.AWS_DOMAIN;
                            break;
                        case BDPresentation.KEY_NAME:
                            domainName = BDPresentation.AWS_DOMAIN;
                            break;
                        case BDSection.KEY_NAME:
                            domainName = BDSection.AWS_DOMAIN;
                            break;
                        case BDSubcategory.KEY_NAME:
                            domainName = BDSubcategory.AWS_DOMAIN;
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
                    }

                    DeleteAttributesRequest request = new DeleteAttributesRequest().WithDomainName(domainName).WithItemName(entry.targetId.Value.ToString().ToUpper());
                    SimpleDb.DeleteAttributes(request);
                    break;
                }
                #endregion

            }

            pLastSyncDate = currentSyncDate;

            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(pDataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            systemSetting.settingDateTimeValue = currentSyncDate;
            pDataContext.SaveChanges();

            return syncDictionary;
        }

        public void DeleteLocalData(Entities pDataContext)
        {
            pDataContext.ExecuteStoreCommand("DELETE FROM BDCategories");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDChapters");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDDiseases");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDDeletions");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDLinkedNoteAssociations");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDLinkedNotes");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDPathogenGroups");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDPathogens");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDPresentations");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDSections");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDSubcategories");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDTherapies");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDTherapyGroups");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDSearchEntryAssociations");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDSearchEntries");
            pDataContext.ExecuteStoreCommand("DELETE FROM BDMetata");
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
    }
}
