using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public class RepositoryHandler
    {
        private const string BD_ACCESS_KEY = @"AKIAJ6SRLQLH2ALT7ZBQ";
        private const string BD_SECRET_KEY = @"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";

        static private readonly RepositoryHandler aws = new RepositoryHandler();

        private AmazonSimpleDB simpleDb = null;

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

            foreach (SyncInfo syncInfoEntry in syncDictionary.Values)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Pull {0}", syncInfoEntry.EntityName));
                SelectRequest selectRequestAction = new SelectRequest().WithSelectExpression(syncInfoEntry.GetLatestSelectString(pLastSyncDate));
                SelectResponse selectResponse = simpleDb.Select(selectRequestAction);

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
                                entryGuid = BDCategory.LoadFromAttributes(pDataContext, attributeDictionary);
                                break;
                            case BDDisease.AWS_DOMAIN:
                                entryGuid = BDDisease.LoadFromAttributes(pDataContext, attributeDictionary);
                                break;
                            case BDLinkedNoteAssociation.AWS_DOMAIN:
                                BDLinkedNoteAssociation.LoadFromAttributes(pDataContext, attributeDictionary);
                                break;
                            case BDLinkedNote.AWS_DOMAIN:
                                entryGuid = BDLinkedNote.LoadFromAttributes(pDataContext, attributeDictionary);
                                break;
                            case BDPathogenGroup.AWS_DOMAIN:
                                entryGuid = BDPathogenGroup.LoadFromAttributes(pDataContext, attributeDictionary);
                                break;
                            case BDPathogen.AWS_DOMAIN:
                                entryGuid = BDPathogen.LoadFromAttributes(pDataContext, attributeDictionary);
                                break;
                            case BDPresentation.AWS_DOMAIN:
                                entryGuid = BDPresentation.LoadFromAttributes(pDataContext, attributeDictionary);
                                break;
                            case BDSection.AWS_DOMAIN:
                                BDSection.LoadFromAttributes(pDataContext, attributeDictionary);
                                break;
                            case BDSubcategory.AWS_DOMAIN:
                                entryGuid = BDSubcategory.LoadFromAttributes(pDataContext, attributeDictionary);
                                break;
                            case BDTherapy.AWS_DOMAIN:
                                entryGuid = BDTherapy.LoadFromAttributes(pDataContext, attributeDictionary);
                                break;
                            case BDTherapyGroup.AWS_DOMAIN:
                                entryGuid = BDTherapyGroup.LoadFromAttributes(pDataContext, attributeDictionary);
                                break;
                        }
                        // The entry id will be null if a sync conflict prevented create/updateso add it to the conflict list
                        if (null == entryGuid) syncInfoEntry.SyncConflictList.Add(attributeDictionary);
                    }
                }
                System.Diagnostics.Debug.WriteLine("Pulled {0} Records for {1}", syncInfoEntry.RowsPulled, syncInfoEntry.EntityName);
            }

            #endregion

            //if (null == pLastSyncDate)
            //{
            //    pLastSyncDate = DateTime.Now;
            //    BDSystemSetting.SaveTimestamp(pDataContext, BDSystemSetting.LASTSYNC_TIMESTAMP, pLastSyncDate.Value);
            //}

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

            pLastSyncDate = DateTime.Now;
            BDSystemSetting.SaveTimestamp(pDataContext, BDSystemSetting.LASTSYNC_TIMESTAMP, pLastSyncDate.Value);

            return syncDictionary;
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
