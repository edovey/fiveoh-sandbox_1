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
    /// Extension of generated BDPathogen
    /// </summary>
    public partial class BDPathogen: IBDObject
    {
        public const string AWS_DOMAIN = @"bd_pathogens_test";
        public const string ENTITYNAME = @"BDPathogen";
        public const string ENTITYNAME_FRIENDLY = @"Pathogen";
        public const string PROPERTYNAME_NAME = @"Name";


        private const string UUID = @"pa_uuid";
        private const string SCHEMAVERSION = @"pa_schemaversion";
        private const string CREATEDBY = @"pa_createdBy";
        private const string CREATEDDATE = @"pa_createdDate";
        private const string MODIFIEDBY = @"pa_modifiedBy";
        private const string MODIFIEDDATE = @"pa_modifiedDate";
        private const string PATHOGENGROUPID = @"di_pathogenGroupId";
        private const string NAME = @"pa_name";
        private const string DEPRECATED = @"pa_deprecated";
        private const string DISPLAYORDER = @"pa_displayOrder";

        public static BDPathogen CreatePathogen(Entities pContext)
        {
            BDPathogen pathogen = CreateBDPathogen(Guid.NewGuid(), false);
            pathogen.createdBy = Guid.Empty;
            pathogen.createdDate = DateTime.Now;
            pathogen.schemaVersion = 0;
            pathogen.displayOrder = -1;
            pathogen.pathogenGroupId = Guid.Empty;
            pathogen.name = string.Empty;

            pContext.AddObject("BDPathogens", pathogen);
            return pathogen;
        }

        /// <summary>
        /// Extend Save method that sets modified date
        /// </summary>
        /// <param name="pPathogen"></param>
        public static void SavePathogen(Entities pContext, BDPathogen pPathogen)
        {
            if (pPathogen.EntityState != EntityState.Unchanged)
            {
                pPathogen.modifiedBy = Guid.Empty;
                pPathogen.modifiedDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine(@"Pathogen Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Get all pathogens in the model with the specified presentation ID
        /// </summary>
        /// <param name="pPresentationId"></param>
        /// <returns>List of Pathogens</returns>
        public static List<BDPathogen> GetPathogensForPathogenGroup(Entities pContext, Guid pPathogenGroupId)
        {
            List<BDPathogen> pathogenList = new List<BDPathogen>();
                IQueryable<BDPathogen> pathogens = (from bdPathogens in pContext.BDPathogens
                                                    where bdPathogens.pathogenGroupId == pPathogenGroupId
                                                    select bdPathogens);
                foreach (BDPathogen pathogen in pathogens)
                {
                    pathogenList.Add(pathogen);
                }
            return pathogenList;
        }

        /// <summary>
        /// Get Pathogen with the specified ID
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPathogenId"></param>
        /// <returns></returns>
        public static BDPathogen GetPathogenWithId(Entities pContext, Guid pPathogenId)
        {
            BDPathogen pathogen = null;
            if (null != pPathogenId)
            {
                IQueryable<BDPathogen> entries = (from bdPathogens in pContext.BDPathogens
                                                    where bdPathogens.uuid == pPathogenId
                                                    select bdPathogens);
                if (entries.Count<BDPathogen>() > 0)
                    pathogen = entries.AsQueryable().First<BDPathogen>();
            }
            return pathogen;
        }

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return string.Format("[{0}]", this.name); }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("Pathogen - {0}", this.name); }
        }

        #region Repository

        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<BDPathogen> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDPathogen> entryList = new List<BDPathogen>();
            IQueryable<BDPathogen> pathogens;

            if (null == pUpdateDateTime)
            {
                pathogens = (from entry in pContext.BDPathogens
                            select entry);
            }
            else
            {
                pathogens = (from entry in pContext.BDPathogens
                            where entry.modifiedDate > pUpdateDateTime.Value
                            select entry);
            }
            if (pathogens.Count() > 0)
                entryList = pathogens.ToList<BDPathogen>();
            return entryList;
        }

        public static SyncInfo SyncInfo()
        {
            return new SyncInfo(AWS_DOMAIN, MODIFIEDDATE);
        }

        /// <summary>
        /// Create or update an existing BDPathogen from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDPathogen entry = BDPathogen.GetPathogenWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDPathogen.CreateBDPathogen(uuid, deprecated);
                pDataContext.AddObject("BDPathogens", entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.pathogenGroupId = Guid.Parse(pAttributeDictionary[PATHOGENGROUPID]);
            entry.name = pAttributeDictionary[NAME];
            short displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;

            pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogen.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogen.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogen.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogen.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogen.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogen.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogen.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogen.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogen.PATHOGENGROUPID).WithValue((null == pathogenGroupId) ? Guid.Empty.ToString() : pathogenGroupId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogen.NAME).WithValue(name).WithReplace(true));

            return putAttributeRequest;
        }

        #endregion
    }
}
