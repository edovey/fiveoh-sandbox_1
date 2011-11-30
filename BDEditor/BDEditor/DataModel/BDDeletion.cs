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
    /// Extension of generated BDDeletion
    /// </summary>
    public partial class BDDeletion
    {
        public const string AWS_DOMAIN = @"bd_1_deletions";
        public const string ENTITYNAME = @"BDDeletions";
        public const string ENTITYNAME_FRIENDLY = @"Deletion";

        private const string UUID = @"de_uuid";
        private const string SCHEMAVERSION = @"de_schemaVersion";
        private const string CREATEDBY = @"de_createdBy";
        private const string CREATEDDATE = @"de_createdDate";
        private const string MODIFIEDBY = @"de_modifiedBy";
        private const string MODIFIEDDATE = @"de_modifiedDate";
        private const string ENTITYID = @"de_entityId";
        private const string ENITIYNAME = @"de_entityName";

        public static void CreateDeletion(Entities pContext, string pEntityName, Guid pEntityId)
        {
            BDDeletion deletion = CreateBDDeletion(Guid.NewGuid());
            deletion.createdBy = Guid.Empty;
            deletion.createdDate = DateTime.Now;
            deletion.modifiedBy = Guid.Empty;
            deletion.modifiedDate = DateTime.Now;
            deletion.schemaVersion = 0;
            deletion.entityName = pEntityName;
            deletion.entityId = pEntityId;

            pContext.AddObject("BDDeletions", deletion);
        }

        #region Repository
        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries.  List is empty if none found.</returns>
        public static List<BDDeletion> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDDeletion> entryList = new List<BDDeletion>();
            IQueryable<BDDeletion> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDDeletions
                           select entry);
            }
            else
            {   
                entries = (from entry in pContext.BDDeletions
                           where entry.modifiedDate > pUpdateDateTime.Value
                           select entry);
            }

            if (entries.Count() > 0)
                entryList = entries.ToList<BDDeletion>();
            return entryList;
        }

        public static BDDeletion GetDeletionsWithId(Entities pContext, Guid pDeletionId)
        {
            BDDeletion deletion = null;
            if(null != pDeletionId)
            {
                IQueryable<BDDeletion> entryList = (from entries in pContext.BDDeletions
                                                   where entries.uuid == pDeletionId
                                                   select entries);
                if (entryList.Count<BDDeletion>() > 0)
                    deletion = entryList.AsQueryable().First<BDDeletion>();
            }
            return deletion;
        }

        public static void DeleteLocalSinceDate(Entities pDataContext, DateTime? pLastSyncDate)
        {
            List<BDDeletion> newDeletionsForLocal = BDDeletion.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            foreach (BDDeletion deletion in newDeletionsForLocal)
            {
                switch (deletion.entityName)
                {
                    case BDCategory.ENTITYNAME_FRIENDLY:
                        BDCategory.Delete(pDataContext, deletion.entityId.Value,false);
                        break;
                    case BDChapter.ENTITYNAME_FRIENDLY:
                        BDChapter.Delete(pDataContext, deletion.entityId.Value, false);
                        break;
                    case BDDisease.ENTITYNAME_FRIENDLY:
                        BDDisease.Delete(pDataContext, deletion.entityId.Value, false);
                        break;
                    case BDLinkedNote.ENTITYNAME_FRIENDLY:
                        BDLinkedNote.Delete(pDataContext, deletion.entityId.Value, false);
                        break;
                    case BDLinkedNoteAssociation.ENTITYNAME_FRIENDLY:
                        BDLinkedNoteAssociation.Delete(pDataContext, deletion.entityId.Value, false);
                        break;
                    case BDPathogen.ENTITYNAME_FRIENDLY:
                        BDPathogen.Delete(pDataContext, deletion.entityId.Value, false);
                        break;
                    case BDPathogenGroup.ENTITYNAME_FRIENDLY:
                        BDPathogenGroup.Delete(pDataContext, deletion.entityId.Value, false);
                        break;
                    case BDPresentation.ENTITYNAME_FRIENDLY:
                        BDPresentation.Delete(pDataContext, deletion.entityId.Value, false);
                        break;
                    case BDSection.ENTITYNAME_FRIENDLY:
                        BDSection.Delete(pDataContext, deletion.entityId.Value, false);
                        break;
                    case BDSubcategory.ENTITYNAME_FRIENDLY:
                        BDSubcategory.Delete(pDataContext, deletion.entityId.Value, false);
                        break;
                    case BDTherapy.ENTITYNAME_FRIENDLY:
                        BDTherapy.Delete(pDataContext, deletion.entityId.Value, false);
                        break;
                    case BDTherapyGroup.ENTITYNAME_FRIENDLY:
                        BDTherapyGroup.Delete(pDataContext, deletion.entityId.Value, false);
                        break;
                }
                pDataContext.SaveChanges();

            }
        }

        public static SyncInfo SyncInfo()
        {
            return new SyncInfo(AWS_DOMAIN, MODIFIEDDATE);
        }

        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDDeletion entry = BDDeletion.GetDeletionsWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDDeletion.CreateBDDeletion(uuid);
                pDataContext.AddObject("BDDeletions", entry);
            }
            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.entityId = Guid.Parse(pAttributeDictionary[ENTITYID]);
            entry.entityName = pAttributeDictionary[ENTITYNAME];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.ENTITYID).WithValue((null == entityId) ? Guid.Empty.ToString() : entityId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.ENTITYNAME).WithValue((null == entityName) ? string.Empty : entityName).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion
    }
}
