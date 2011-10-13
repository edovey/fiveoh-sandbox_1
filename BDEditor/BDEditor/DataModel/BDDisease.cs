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
    public partial class BDDisease
    {
        public const string ENTITYNAME_FRIENDLY = @"Disease";
        public const string PROPERTYNAME_OVERVIEW = @"Overview";
        public const string AWS_DOMAIN = @"bd_diseases_test";

        private const string UUID = @"di_uuid";
        private const string SCHEMAVERSION = @"di_schemaversion";
        private const string CREATEDBY = @"di_createdBy";
        private const string CREATEDDATE = @"di_createdDate";
        private const string MODIFIEDBY = @"di_modifiedBy";
        private const string MODIFIEDDATE = @"di_modifiedDate";
        private const string DISPLAYORDER = @"di_displayOrder";
        private const string SUBCATEGORYID = @"di_subcategoryId";
        private const string CATEGORYID = @"di_categoryId";
        private const string NAME = @"di_name";
        private const string DEPRECATED = @"di_deprecated";
        private const string OVERVIEW = @"di_overview";


        /// <summary>
        /// Extended Create method that sets the create date and the schema version
        /// </summary>
        /// <returns>BDDisease object</returns>
        public static BDDisease CreateDisease(Entities pContext)
        {
            BDDisease disease = CreateBDDisease(Guid.NewGuid(), false);
            disease.createdBy = Guid.Empty;
            disease.createdDate = DateTime.Now;
            disease.schemaVersion = 0;

            pContext.AddObject("BDDiseases", disease);

            return disease;
        }

        /// <summary>
        /// Extended Save method that sets the modification date
        /// </summary>
        /// <param name="pDisease"></param>
        public static void SaveDisease(Entities pContext, BDDisease pDisease)
        {
            if (pDisease.EntityState != EntityState.Unchanged)
            {
                pDisease.modifiedBy = Guid.Empty;
                pDisease.modifiedDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine(@"Disease Save");
                pContext.SaveChanges();
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
  
            IQueryable<BDDisease> diseases = (from bdDiseases in pContext.BDDiseases
                                        where bdDiseases.categoryId == pCategoryId
                                        select bdDiseases);

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

        IQueryable<BDDisease> diseases = (from bdDiseases in pContext.BDDiseases
                                                where bdDiseases.subcategoryId == pSubcategoryId
                                                select bdDiseases);

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
            BDDisease disease;

            IQueryable<BDDisease> diseases = (from bdDiseases in pContext.BDDiseases
                                                where bdDiseases.uuid == pDiseaseId
                                                select bdDiseases);
            disease = diseases.AsQueryable().First<BDDisease>();

            return disease;
        }

        #region Repository

        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<BDDisease> GetDiseasesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDDisease> entryList = new List<BDDisease>();
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
                entryList = entries.ToList<BDDisease>();
            return entryList;
        }

        public static SyncInfo SyncInfo()
        {
            return new SyncInfo(AWS_DOMAIN, MODIFIEDDATE);
        }

        /// <summary>
        /// Create or update an existing BDDisease from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDDisease entry = BDDisease.GetDiseaseWithId(pDataContext, uuid);
            if (null == entry)
                entry = BDDisease.CreateBDDisease(uuid, deprecated);

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            short displayOrder = short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.categoryId = Guid.Parse(pAttributeDictionary[CATEGORYID]);
            entry.subcategoryId = Guid.Parse(pAttributeDictionary[SUBCATEGORYID]);
            entry.name = pAttributeDictionary[NAME];
            entry.overview = pAttributeDictionary[OVERVIEW];

            BDDisease.SaveDisease(pDataContext, entry);
            
            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.UUID).WithValue(uuid.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.DEPRECATED).WithValue(deprecated.ToString()));

            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.CATEGORYID).WithValue((null == categoryId) ? Guid.Empty.ToString() : categoryId.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.SUBCATEGORYID).WithValue((null == subcategoryId) ? Guid.Empty.ToString() : subcategoryId.ToString().ToUpper())); 
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.NAME).WithValue(name));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDisease.OVERVIEW).WithValue(overview));

            return putAttributeRequest;
        }
        #endregion
    }
}
