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
    /// Extension of generated BDPathogenGroup
    /// </summary>
    public partial class BDPathogenGroup
    {
        public const string AWS_DOMAIN = @"bd_1_pathogenGroups";
        public const string ENTITYNAME = @"BDPathogenBroups";
        public const string ENTITYNAME_FRIENDLY = @"Pathogen Group";
        
        private const string UUID = @"pg_uuid";
        private const string SCHEMAVERSION = @"pg_schemaVersion";
        private const string CREATEDBY = @"pg_createdBy";
        private const string CREATEDDATE = @"pg_createdDate";
        private const string MODIFIEDBY = @"pg_modifiedBy";
        private const string MODIFIEDDATE = @"pg_modifiedDate";
        private const string PRESENTATIONID = @"pg_presentationId";
        private const string DEPRECATED = @"pg_deprecated";
        private const string DISPLAYORDER = @"pg_displayOrder";

        /// <summary>
        /// Extended Create method that sets created date and schema version
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static BDPathogenGroup CreatePathogenGroup(Entities pContext, Guid pPresentationId)
        {
            BDPathogenGroup pathogenGroup = CreateBDPathogenGroup(Guid.NewGuid(), false);
            pathogenGroup.createdBy = Guid.Empty;
            pathogenGroup.createdDate = DateTime.Now;
            pathogenGroup.schemaVersion = 0;
            pathogenGroup.displayOrder = -1;
            pathogenGroup.presentationId = pPresentationId;

            pContext.AddObject("BDPathogenGroups", pathogenGroup);

            return pathogenGroup;
        }


        /// <summary>
        /// Extended Save method that sets the modified date.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPathogenGroup"></param>
        public static void SavePathogenGroup(Entities pContext, BDPathogenGroup pPathogenGroup)
        {
            if (pPathogenGroup.EntityState != EntityState.Unchanged)
            {
                //pPathogenGroup.modifiedBy = Guid.Empty;
                //pPathogenGroup.modifiedDate = DateTime.Now;

                System.Diagnostics.Debug.WriteLine(@"PathogenGroup Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all pathogen groups in the model with the specified presentation ID
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPresentationId"></param>
        /// <returns></returns>
        public static List<BDPathogenGroup> GetPathogenGroupsForPresentationId(Entities pContext, Guid pPresentationId)
        {
            List<BDPathogenGroup> pathogenGroupList = new List<BDPathogenGroup>();

            IQueryable<BDPathogenGroup> pathogenGroups = (from bdPathogenGroups in pContext.BDPathogenGroups
                                                          where bdPathogenGroups.presentationId == pPresentationId
                                                          select bdPathogenGroups);
            foreach (BDPathogenGroup pathogenGroup in pathogenGroups)
            {
                pathogenGroupList.Add(pathogenGroup);
            }
            return pathogenGroupList;
        }

        public static BDPathogenGroup GetPathogenGroupWithId(Entities pContext, Guid pPathogenGroupId)
        {
            BDPathogenGroup pathogenGroup = null;

            if (null != pPathogenGroupId)
            {
                IQueryable<BDPathogenGroup> entries = (from bdPathogenGroups in pContext.BDPathogenGroups
                                                              where bdPathogenGroups.uuid == pPathogenGroupId
                                                              select bdPathogenGroups);
                if (entries.Count<BDPathogenGroup>() > 0)
                    pathogenGroup = entries.AsQueryable().First<BDPathogenGroup>();
            }
            return pathogenGroup;
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
        public static List<BDPathogenGroup> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDPathogenGroup> entryList = new List<BDPathogenGroup>();
            IQueryable<BDPathogenGroup> pathogenGroups;

            if (null == pUpdateDateTime)
            {
                pathogenGroups = (from entry in pContext.BDPathogenGroups
                            select entry);
            }
            else
            {
                pathogenGroups = (from entry in pContext.BDPathogenGroups
                            where entry.modifiedDate > pUpdateDateTime.Value
                            select entry);
            }
            if (pathogenGroups.Count() > 0)
                entryList = pathogenGroups.ToList<BDPathogenGroup>();
            return entryList;
        }

        public static SyncInfo SyncInfo()
        {
            return new SyncInfo(AWS_DOMAIN, MODIFIEDDATE);
        }

        /// <summary>
        /// Create or update an existing BDPathogenGroup from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDPathogenGroup entry = BDPathogenGroup.GetPathogenGroupWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDPathogenGroup.CreateBDPathogenGroup(uuid, deprecated);
                pDataContext.AddObject("BDPathogenGroups", entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.presentationId = Guid.Parse(pAttributeDictionary[PRESENTATIONID]);
            short displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;

            if (pSaveChanges)
                pDataContext.SaveChanges();
            
            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.PRESENTATIONID).WithValue((null == presentationId) ? Guid.Empty.ToString() : presentationId.ToString().ToUpper()).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion
    }
}
