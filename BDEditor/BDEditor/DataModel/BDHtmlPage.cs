using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDHtmlPage
    /// </summary>
    public partial class BDHtmlPage : IBDObject
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_htmlPages";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_htmlPages";

        public const string AWS_PROD_BUCKET = @"bdProdStore";
        public const string AWS_DEV_BUCKET = @"bdDevStore";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
        public const string AWS_BUCKET = AWS_DEV_BUCKET;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
        public const string AWS_BUCKET = AWS_PROD_BUCKET;
#endif

        public const string AWS_S3_PREFIX = @"bdhp~";
        public const string AWS_S3_FILEEXTENSION = @".html";

        public const string ENTITYNAME = @"BDHtmlPages";
        public const string ENTITYNAME_FRIENDLY = @"HTML Pages";
        public const string KEY_NAME = @"BDHtmlPage";

        public const int ENTITY_SCHEMAVERSION = 1;

        private const string UUID = @"ht_uuid";
        private const string SCHEMAVERSION = @"ht_schemaVersion";
        private const string CREATEDBY = @"ht_createdBy";
        private const string CREATEDDATE = @"ht_createdDate";
        private const string DISPLAYPARENTID = @"ht_displayParentId";
        private const string DISPLAYPARENTTYPE = @"ht_displayParentType";
        private const string STORAGEKEY = @"ht_storageKey";
        private const string DOCUMENTTEXT = @"ht_documentText";
        private const string HTMLPAGETYPE = @"ht_htmlPageType";
        private const string HTMLPAGELAYOUT = @"ht_layout";
        private const string HTMLPAGETITLE = @"ht_pageTitle";

        public Guid? tempProductionUuid { get; set; }

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid"></param>
        /// <returns></returns>
        public static BDHtmlPage CreateBDHtmlPage(Entities pContext, Guid pUuid)
        {
            BDHtmlPage page = CreateBDHtmlPage(pUuid);
            page.createdBy = Guid.Empty;
            page.createdDate = DateTime.Now;
            page.schemaVersion = ENTITY_SCHEMAVERSION;
            page.documentText = string.Empty;
            page.storageKey = GenerateStorageKey(page);
            page.htmlPageType = (int)BDConstants.BDHtmlPageType.Undefined;
            page.layoutVariant = (int)BDConstants.LayoutVariantType.Undefined;
            page.pageTitle = string.Empty;

            pContext.AddObject(ENTITYNAME, page);
            Save(pContext, page);
            return page;
        }

        public static void Save(Entities pContext, BDHtmlPage pPage)
        {
            if (pPage.EntityState != EntityState.Unchanged)
            {
                if (pPage.schemaVersion != ENTITY_SCHEMAVERSION)
                    pPage.schemaVersion = ENTITY_SCHEMAVERSION;

                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Delete from the local datastore without creating a deletion record nor deleting any children. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid"></param>
        public static void DeleteLocal(Entities pContext, Guid? pUuid)
        {
            if (null != pUuid)
            {
                BDHtmlPage entry = BDHtmlPage.RetrieveWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        /// <summary>
        /// Delete all records from local store
        /// </summary>
        public static void DeleteAll(Entities pContext)
        {
            // check if table exists:
            int result = pContext.ExecuteStoreQuery<int> (@"SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME ='BDHtmlPages'").SingleOrDefault();
            
            if(result == 1) 
                pContext.ExecuteStoreCommand("DELETE FROM BDHtmlPages");
        }

        /// <summary>
        /// Return the HtmlPage for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pEntryId"></param>
        /// <returns></returns>
        public static BDHtmlPage RetrieveWithId(Entities pContext, Guid? pEntryId)
        {
            BDHtmlPage result = null;

            if (null != pEntryId)
            {
                IQueryable<BDHtmlPage> entries = (from entry in pContext.BDHtmlPages
                                                  where entry.uuid == pEntryId
                                                    select entry);

                if (entries.Count<BDHtmlPage>() > 0)
                    result = entries.AsQueryable().First<BDHtmlPage>();
            }

            return result;
        }

        /// <summary>
        /// Returns the HTML pages for the display parent where the type is 'Data'
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentId"></param>
        /// <returns></returns>
        public static List<BDHtmlPage> RetrieveHtmlPageForDisplayParentId(Entities pContext, Guid? pDisplayParentId)
        {
            IQueryable<BDHtmlPage> entries = (from entry in pContext.BDHtmlPages
                                              where entry.displayParentId == pDisplayParentId
                                              select entry);
            List<BDHtmlPage> resultList = entries.ToList<BDHtmlPage>();
            return resultList;
        }

        public static List<BDHtmlPage> RetrieveHtmlPageForDisplayParentIdOfPageType(Entities pContext, Guid? pDisplayParentId, BDConstants.BDHtmlPageType pPageType)
        {
            IQueryable<BDHtmlPage> entries = (from entry in pContext.BDHtmlPages
                                              where entry.displayParentId == pDisplayParentId
                                                    && entry.htmlPageType == (int)pPageType
                                              select entry);
            List<BDHtmlPage> resultList = entries.ToList<BDHtmlPage>();
            return resultList;
        }

        public static List<BDHtmlPage> RetrieveAll(Entities pContext)
        {
            List<BDHtmlPage> entryList = new List<BDHtmlPage>();
            IQueryable<BDHtmlPage> entries;

            entries = (from entry in pContext.BDHtmlPages
                   select entry);
            if (entries.Count() > 0)
                return entries.ToList<BDHtmlPage>();

            return entryList;
        }

        public static List<IBDObject> RetrieveAllAsIBDObject(Entities pContext)
        {
            return RetrieveAll(pContext).ToList<IBDObject>();

        }

        public static List<Guid> RetrieveAllIds(Entities pContext)
        {
            IQueryable<Guid> pages = (from entry in pContext.BDHtmlPages
                                      select entry.uuid);
            return pages.ToList<Guid>();
        }

        public static List<Guid> RetrieveAllDisplayParentIDs(Entities pContext)
        {
            IQueryable<Guid> pages = (from entry in pContext.BDHtmlPages
                                            select entry.displayParentId.Value);
            return pages.ToList<Guid>();
        }

        #region Repository

        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<IBDObject> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<IBDObject> entryList = new List<IBDObject>();
            IQueryable<BDHtmlPage> entries;

            // get ALL entries - this is a full refresh.
            entries = (from entry in pContext.BDHtmlPages
                       select entry);

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDHtmlPage>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, CREATEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDHtmlPage.RetrieveAllAsIBDObject(pDataContext);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            return syncInfo;
        }

        public static string GenerateStorageKey(BDHtmlPage pNote)
        {
            string result = GenerateStorageKey(pNote.Uuid);
            return result;
        }

        public static string GenerateStorageKey(Guid pUuid)
        {
            string result = string.Format("{0}{1}{2}", AWS_S3_PREFIX, pUuid.ToString().ToUpper(), AWS_S3_FILEEXTENSION);
            return result;
        }

         #endregion

        #region IBDObject implementation
        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.DISPLAYPARENTID).WithValue((null == displayParentId) ? Guid.Empty.ToString() : displayParentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.STORAGEKEY).WithValue((null == storageKey) ? string.Empty : storageKey).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.DISPLAYPARENTTYPE).WithValue(string.Format(@"{0}", displayParentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.HTMLPAGETYPE).WithValue(string.Format(@"{0}", htmlPageType)).WithReplace(true));

            // schema version 1
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.HTMLPAGELAYOUT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.HTMLPAGETITLE).WithValue((null == pageTitle) ? string.Empty : pageTitle).WithReplace(true));

            return putAttributeRequest;
        }

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public string DescriptionForLinkedNote
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
        public override string ToString()
        {
            return this.uuid.ToString();
        }

        public BDConstants.BDHtmlPageType HtmlPageType
        {
            get
            {
                BDConstants.BDHtmlPageType result = BDConstants.BDHtmlPageType.Undefined;

                if (Enum.IsDefined(typeof(BDConstants.BDHtmlPageType), htmlPageType))
                {
                    result = (BDConstants.BDHtmlPageType)htmlPageType;
                }
                return result;
            }
        }
    }
}
