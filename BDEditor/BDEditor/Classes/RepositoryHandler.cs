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
            DateTime currentTimestamp = DateTime.Now;
            SyncInfo syncInfo;

            #region Initialize Sync
            SyncInfoDictionary syncDictionary = new SyncInfoDictionary();

            syncDictionary.Add(new SyncInfo(BDCategory.AWS_DOMAIN));

            // List the remote domains
            ListDomainsResponse sdbListDomainsResponse = SimpleDb.ListDomains(new ListDomainsRequest());
            if (sdbListDomainsResponse.IsSetListDomainsResult())
            {
                ListDomainsResult listDomainsResult = sdbListDomainsResponse.ListDomainsResult;
                foreach (String domain in listDomainsResult.DomainName)
                {
                    if (syncDictionary.ContainsKey(domain))
                        syncDictionary[domain].ExistsOnRemote = true;
                }
            }

            // Create missing domains
            foreach (SyncInfo entry in syncDictionary.Values)
            {
                if (!entry.ExistsOnRemote)
                {
                    CreateDomainRequest createDomain = (new CreateDomainRequest()).WithDomainName(entry.EntityName);
                    simpleDb.CreateDomain(createDomain);
                }
            }
           

            #endregion

            #region Pull

            #endregion

            #region Push

            // BDCategories
            List<BDCategory> categoryChanges = BDCategory.GetCategoriesUpdatedSince(pDataContext, pLastSyncDate);
            syncDictionary[BDCategory.AWS_DOMAIN].RowsPushed = categoryChanges.Count;

            foreach (BDCategory bdCategory in categoryChanges)
            {
                SimpleDb.PutAttributes(bdCategory.PutAttributes());  // Push to the repository
            }


            #endregion

            BDSystemSetting.SaveTimestamp(pDataContext, BDSystemSetting.LASTSYNC_TIMESTAMP, currentTimestamp);

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
