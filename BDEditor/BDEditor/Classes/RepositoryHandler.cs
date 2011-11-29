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
            #region Initialize Sync
            SyncInfoDictionary syncDictionary = new SyncInfoDictionary();

            syncDictionary.Add(BDCategory.SyncInfo());
            syncDictionary.Add(BDChapter.SyncInfo());
            syncDictionary.Add(BDDisease.SyncInfo());
            syncDictionary.Add(BDLinkedNoteAssociation.SyncInfo());
            syncDictionary.Add(BDLinkedNote.SyncInfo());
            syncDictionary.Add(BDPathogenGroup.SyncInfo());
            syncDictionary.Add(BDPathogen.SyncInfo());
            syncDictionary.Add(BDPresentation.SyncInfo());
            syncDictionary.Add(BDSection.SyncInfo());
            syncDictionary.Add(BDSubcategory.SyncInfo());
            syncDictionary.Add(BDTherapy.SyncInfo());
            syncDictionary.Add(BDTherapyGroup.SyncInfo());
            syncDictionary.Add(BDDeletion.SyncInfo());

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
                        CreateDomainRequest createDomainRequest = (new CreateDomainRequest()).WithDomainName(syncInfoEntry.EntityName);
                        CreateDomainResponse createResponse = simpleDb.CreateDomain(createDomainRequest);
                        syncInfoEntry.ExistsOnRemote = true;
                        System.Diagnostics.Debug.WriteLine(string.Format("Created domain for {0}", syncInfoEntry.EntityName));
                    }
                    catch (AmazonSimpleDBException ex)
                    {
                        syncInfoEntry.Exception = ex;
                        System.Diagnostics.Debug.WriteLine(string.Format("Failed to created domain for {0}", syncInfoEntry.EntityName));
                    }
                }
            }

            #endregion

            #region Pull
            Common.Settings.IsSyncLoad = true;

            foreach (SyncInfo syncInfoEntry in syncDictionary.Values)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Pull {0}", syncInfoEntry.EntityName));
                SelectRequest selectRequestAction = new SelectRequest().WithSelectExpression(syncInfoEntry.GetLatestSelectString(pLastSyncDate));

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
                            switch (syncInfoEntry.EntityName)
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

                    System.Diagnostics.Debug.WriteLine("Pulled {0} Records for {1}", syncInfoEntry.RowsPulled, syncInfoEntry.EntityName);

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
                    System.Diagnostics.Debug.WriteLine(string.Format("Push {0}", syncInfoEntry.EntityName));
                    switch (syncInfoEntry.EntityName)
                    {
                        case BDCategory.AWS_DOMAIN:
                            {
                                List<BDCategory> changeList = BDCategory.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDCategory entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes());  // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDChapter.AWS_DOMAIN:
                            {
                                List<BDChapter> changeList = BDChapter.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDChapter entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes()); // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDDeletion.AWS_DOMAIN:
                            {
                                List<BDDeletion> changeList = BDDeletion.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDDeletion entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes()); // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDDisease.AWS_DOMAIN:
                            {
                                List<BDDisease> changeList = BDDisease.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDDisease entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes());  // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDLinkedNoteAssociation.AWS_DOMAIN:
                            {
                                List<BDLinkedNoteAssociation> changeList = BDLinkedNoteAssociation.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDLinkedNoteAssociation entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes());  // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDLinkedNote.AWS_DOMAIN:
                            {
                                List<BDLinkedNote> changeList = BDLinkedNote.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDLinkedNote entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes());  // Push to the repository

                                    string encodedText = System.Net.WebUtility.HtmlEncode(entry.documentText);

                                    PutObjectRequest putObjectRequest = new PutObjectRequest()
                                        .WithContentType(@"text/plain")
                                        //.WithContentBody(encodedText)
                                        .WithContentBody(entry.documentText)
                                        .WithBucketName(BDLinkedNote.AWS_BUCKET)
                                        .WithKey(entry.storageKey);

                                    S3Response s3Response = S3.PutObject(putObjectRequest);
                                    s3Response.Dispose();

                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDPathogenGroup.AWS_DOMAIN:
                            {
                                List<BDPathogenGroup> changeList = BDPathogenGroup.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDPathogenGroup entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes());  // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDPathogen.AWS_DOMAIN:
                            {
                                List<BDPathogen> changeList = BDPathogen.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDPathogen entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes());  // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDPresentation.AWS_DOMAIN:
                            {
                                List<BDPresentation> changeList = BDPresentation.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDPresentation entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes());  // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDSection.AWS_DOMAIN:
                            {
                                List<BDSection> changeList = BDSection.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDSection entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes());  // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDSubcategory.AWS_DOMAIN:
                            {
                                List<BDSubcategory> changeList = BDSubcategory.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDSubcategory entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes());  // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDTherapy.AWS_DOMAIN:
                            {
                                List<BDTherapy> changeList = BDTherapy.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDTherapy entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes());  // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                        case BDTherapyGroup.AWS_DOMAIN:
                            {
                                List<BDTherapyGroup> changeList = BDTherapyGroup.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                                foreach (BDTherapyGroup entry in changeList)
                                {
                                    SimpleDb.PutAttributes(entry.PutAttributes());  // Push to the repository
                                    syncInfoEntry.RowsPushed++;
                                }
                            }
                            break;
                    }
                    System.Diagnostics.Debug.WriteLine("Pushed {0} Records for {1}", syncInfoEntry.RowsPushed, syncInfoEntry.EntityName);
                }
            #endregion

                #region Delete Remote
                // Process all deletion records in database since last sync: will include records received in last pull
                List<BDDeletion> newDeletionsForRemote = BDDeletion.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
                string domainName = "";
                foreach (BDDeletion entry in newDeletionsForRemote)
                {
                    switch(entry.entityName)
                    {
                        case BDCategory.ENTITYNAME_FRIENDLY:
                            domainName = BDCategory.AWS_DOMAIN;
                            break;
                        case BDChapter.ENTITYNAME_FRIENDLY:
                            domainName = BDChapter.AWS_DOMAIN;
                            break;
                        case BDDisease.ENTITYNAME_FRIENDLY:
                            domainName = BDDisease.AWS_DOMAIN;
                            break;
                        case BDLinkedNote.ENTITYNAME_FRIENDLY:
                            domainName = BDLinkedNote.AWS_DOMAIN;
                            break;
                        case BDLinkedNoteAssociation.ENTITYNAME_FRIENDLY:
                            domainName = BDLinkedNoteAssociation.AWS_DOMAIN;
                            break;
                        case BDPathogen.ENTITYNAME_FRIENDLY:
                            domainName = BDPathogen.AWS_DOMAIN;
                            break;
                        case BDPathogenGroup.ENTITYNAME_FRIENDLY:
                            domainName = BDPathogenGroup.AWS_DOMAIN;
                            break;
                        case BDPresentation.ENTITYNAME_FRIENDLY:
                            domainName = BDPresentation.AWS_DOMAIN;
                            break;
                        case BDSection.ENTITYNAME_FRIENDLY:
                            domainName = BDSection.AWS_DOMAIN;
                            break;
                        case BDSubcategory.ENTITYNAME_FRIENDLY:
                            domainName = BDSubcategory.AWS_DOMAIN;
                            break;
                        case BDTherapy.ENTITYNAME_FRIENDLY:
                            domainName = BDTherapy.AWS_DOMAIN;
                            break;
                        case BDTherapyGroup.ENTITYNAME_FRIENDLY:
                            domainName = BDTherapyGroup.AWS_DOMAIN;
                            break;
                    }

                    DeleteAttributesRequest request = new DeleteAttributesRequest().WithDomainName(domainName).WithItemName(entry.entityId.Value.ToString().ToUpper());
                    SimpleDb.DeleteAttributes(request);
                    break;
                }
                #endregion

            }

            pLastSyncDate = DateTime.Now;
            BDSystemSetting.SaveTimestamp(pDataContext, BDSystemSetting.LASTSYNC_TIMESTAMP, pLastSyncDate.Value);

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
