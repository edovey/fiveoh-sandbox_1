using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDDisease
    /// </summary>
    public partial class BDDisease: IBDObject
    {

        //public const string AWS_DOMAIN = @"bd_1_diseases";

        public const string AWS_PROD_DOMAIN = @"bd_1_diseases";
        public const string AWS_DEV_DOMAIN = @"bd_dev_1_diseases";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif
        public const string ENTITYNAME = @"BDDiseases";
        public const string ENTITYNAME_FRIENDLY = @"Disease";
        public const string PROPERTYNAME_OVERVIEW = @"Overview";
        public const string KEY_NAME = @"BDDisease";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"di_uuid";
        private const string SCHEMAVERSION = @"di_schemaVersion";
        private const string CREATEDBY = @"di_createdBy";
        private const string CREATEDDATE = @"di_createdDate";
        private const string MODIFIEDBY = @"di_modifiedBy";
        private const string MODIFIEDDATE = @"di_modifiedDate";
        private const string DISPLAYORDER = @"di_displayOrder";
        private const string SUBCATEGORYID = @"di_subcategoryId";
        private const string CATEGORYID = @"di_categoryId";
        private const string NAME = @"di_name";
        private const string DEPRECATED = @"di_deprecated";


        /// <summary>
        /// Extended Create method that sets the create date and the schema version
        /// </summary>
        /// <returns>BDDisease object</returns>
        public static BDDisease CreateDisease(Entities pContext)
        {
            BDDisease disease = CreateBDDisease(Guid.NewGuid(), false);
            disease.createdBy = Guid.Empty;
            disease.createdDate = DateTime.Now;
            disease.schemaVersion = ENTITY_SCHEMAVERSION;
            disease.displayOrder = -1;
            disease.subcategoryId = Guid.Empty;
            disease.categoryId = Guid.Empty;
            disease.name = string.Empty;
            
            pContext.AddObject(ENTITYNAME, disease);

            return disease;
        }

        /// <summary>
        /// Extended Save method that sets the modification date
        /// </summary>
        /// <param name="pDisease"></param>
        public static void Save(Entities pContext, BDDisease pDisease)
        {
            if (pDisease.EntityState != EntityState.Unchanged)
            {
                if (pDisease.schemaVersion != ENTITY_SCHEMAVERSION)
                    pDisease.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"Disease Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDDisease pEntity)
        {
            // delete linked notes
            List<BDLinkedNoteAssociation> notes = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pEntity.uuid);
            foreach (BDLinkedNoteAssociation a in notes)
            {
                BDLinkedNoteAssociation.Delete(pContext, a);
            }
            // delete children
            List<BDPresentation> children = BDPresentation.GetPresentationsForDiseaseId(pContext, pEntity.uuid);
            foreach(BDPresentation p in children)
            {
                BDPresentation.Delete(pContext, p);
            }

            // create BDDeletion record for the object to be deleted
            BDDeletion.CreateDeletion(pContext, KEY_NAME, pEntity.uuid);
            // delete record from local data store
            pContext.DeleteObject(pEntity);
            pContext.SaveChanges();
        }

        /// <summary>
        /// Get object to delete using provided uuid, call extended delete
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid">Guid of record to delete</param>
        /// <param name="pCreateDeletion"create entry in deletion table (bool)</param>
        public static void Delete(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            BDDisease entity = BDDisease.GetDiseaseWithId(pContext, pUuid);
            if (null != entity)
            {
                if (pCreateDeletion)
                {
                    BDDisease.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                }
            }
        }

        /// <summary>
        /// Gets all diseases in the model with the specified category ID
        /// </summary>
        /// <param name="pCategoryId"></param>
        /// <returns>List of Diseases</returns>
        public static List<BDDisease> GetDiseasesForCategoryId(Entities pContext, Guid pCategoryId)
        {
            List<BDDisease> diseaseList = new List<BDDisease>();
  
            IQueryable<BDDisease> diseases = (from entry in pContext.BDDiseases
                                        where entry.categoryId == pCategoryId
                                        orderby entry.displayOrder
                                        select entry);

            foreach (BDDisease disease in diseases)
            {
                diseaseList.Add(disease);
            }
            return diseaseList;
        }
     
        /// <summary>
        /// Gets all diseases in the model with the specified subcategory ID
        /// </summary>
        /// <param name="pSubcategoryId"></param>
        /// <returns>List of Diseases</returns>
        public static List<BDDisease> GetDiseasesForSubcategory(Entities pContext, Guid pSubcategoryId)
        {
            List<BDDisease> diseaseList = new List<BDDisease>();

        IQueryable<BDDisease> diseases = (from entry in pContext.BDDiseases
                                                where entry.subcategoryId == pSubcategoryId
                                                orderby entry.displayOrder
                                                select entry);

            foreach (BDDisease disease in diseases)
            {
                diseaseList.Add(disease);
            }

            return diseaseList;
        }

        /// <summary>
        /// Get Disease with the specified ID
        /// </summary>
        /// <param name="pDiseaseId"></param>
        /// <returns>Disease object</returns>
        public static BDDisease GetDiseaseWithId(Entities pContext, Guid pDiseaseId)
        {
            BDDisease disease = null;

            if (null != pDiseaseId)
            {
                IQueryable<BDDisease> entries = (from bdDiseases in pContext.BDDiseases
                                                 where bdDiseases.uuid == pDiseaseId
                                                 select bdDiseases);
                if (entries.Count<BDDisease>() > 0)
                    disease = entries.AsQueryable().First<BDDisease>();
            }
            return disease;
        }

        protected override void OnPropertyChanged(string property)
        {
            if (!Common.Settings.IsSyncLoad)
                switch (property)
                {
                    case "createdBy":
                    case "createdDate":
                    case "modifiedBy":
                    case "modifiedDate":
                        break;
                    default:
                        {
                            modifiedBy = Guid.Empty;
                            modifiedDate = DateTime.Now;
                        }
                        break;
                }

            base.OnPropertyChanged(property);
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
            IQueryable<BDDisease> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDDiseases
                            select entry);
            }
            else
            {
                entries = (from entry in pContext.BDDiseases
                            where entry.modifiedDate > pUpdateDateTime.Value
                            select entry);
            }

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDDisease>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDDisease.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
            {
                ((BDDisease)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
            }
            if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDDisease from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDDisease entry = BDDisease.GetDiseaseWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDDisease.CreateBDDisease(uuid, deprecated);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            short displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.categoryId = Guid.Parse(pAttributeDictionary[CATEGORYID]);
            entry.subcategoryId = Guid.Parse(pAttributeDictionary[SUBCATEGORYID]);
            entry.name = pAttributeDictionary[NAME];

            if (pSaveChanges)
                pDataContext.SaveChanges();
            
            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.CATEGORYID).WithValue((null == categoryId) ? Guid.Empty.ToString() : categoryId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.SUBCATEGORYID).WithValue((null == subcategoryId) ? Guid.Empty.ToString() : subcategoryId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

            return putAttributeRequest;
        }

        #endregion

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return this.name; }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("{0}: {1}", ENTITYNAME_FRIENDLY, this.name); }
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}
