using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDSection
    /// </summary>
    public partial class BDSection
    {
        public const string AWS_DOMAIN = @"bd_sections_test";
        public const string ENTITYNAME = @"BDSections";
        public const string ENTITYNAME_FRIENDLY = @"Section";

        private const string UUID = @"sn_uuid";
        private const string SCHEMAVERSION = @"sn_schemaversion";
        private const string CREATEDBY = @"sn_createdby";
        private const string CREATEDDATE = @"sn_createddate";
        private const string MODIFIEDBY = @"sn_modifiedby";
        private const string MODIFIEDDATE = @"sn_modifieddate";
        private const string NAME = @"sn_name";
        private const string DEPRECATED = @"sn_deprecated";
        private const string DISPLAYORDER = @"sn_displayorder";

        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <returns></returns>
        public static BDSection CreateSection(Entities pContext)
        {
            //using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            //{
                BDSection section = CreateBDSection(Guid.NewGuid(), false);
                section.createdBy = Guid.Empty;
                section.createdDate = DateTime.Now;
                section.schemaVersion = 0;

                pContext.AddObject("BDSections", section);

                return section;
            //}
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pSection"></param>
        public static void SaveSection(Entities pContext, BDSection pSection)
        {
            if (pSection.EntityState != EntityState.Unchanged)
            {
                pSection.modifiedBy = Guid.Empty;
                pSection.modifiedDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine(@"Section Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Get Section with the specified ID
        /// </summary>
        /// <param name="pSectionId"></param>
        /// <returns>BDSection object.</returns>
        public static BDSection GetSectionWithId(Entities pContext, Guid pSectionId)
        {
            BDSection section;
            
                IQueryable<BDSection> sections = (from bdSections in pContext.BDSections
                                                     where bdSections.uuid == pSectionId
                                                     select bdSections);
                section = sections.AsQueryable().First<BDSection>();
            return section;
        }

        #region Repository

        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<BDSection> GetSectionsUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDSection> entryList = new List<BDSection>();
            IQueryable<BDSection> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDSections
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDSections
                           where entry.modifiedDate > pUpdateDateTime.Value
                           select entry);
            }
            if (entries.Count() > 0)
                entryList = entries.ToList<BDSection>();
            return entryList;
        }

        public static SyncInfo SyncInfo()
        {
            return new SyncInfo(AWS_DOMAIN, MODIFIEDDATE);
        }

        /// <summary>
        /// Create or update an existing BDSection from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDSection entry = BDSection.GetSectionWithId(pDataContext, uuid);
            if (null == entry)
                entry = BDSection.CreateBDSection(uuid, deprecated);

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            short displayOrder = short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.name = pAttributeDictionary[NAME];

            BDSection.SaveSection(pDataContext, entry);

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.UUID).WithValue(uuid.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.DEPRECATED).WithValue(deprecated.ToString()));

            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.NAME).WithValue(name));

            return putAttributeRequest;
        }
        #endregion

    }
}
