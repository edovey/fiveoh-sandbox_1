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
    public partial class BDDeletion: IBDObject
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_deletions";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_deletions";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDDeletions";
        public const string ENTITYNAME_FRIENDLY = @"Deletion";
        public const string KEY_NAME = @"BDDeletion";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"de_uuid";
        private const string SCHEMAVERSION = @"de_schemaVersion";
        private const string CREATEDBY = @"de_createdBy";
        private const string CREATEDDATE = @"de_createdDate";
        private const string MODIFIEDBY = @"de_modifiedBy";
        private const string MODIFIEDDATE = @"de_modifiedDate";
        private const string TARGETID = @"de_targetId";
        private const string TARGETNAME = @"de_targetName";

        public static void CreateBDDeletion(Entities pContext, string pTargetName, IBDNode pTargetNode)
        {
            BDDeletion deletion = CreateBDDeletion(Guid.NewGuid());
            deletion.createdBy = Guid.Empty;
            deletion.createdDate = DateTime.Now;
            deletion.modifiedBy = Guid.Empty;
            deletion.modifiedDate = DateTime.Now;
            deletion.schemaVersion = ENTITY_SCHEMAVERSION;
            deletion.targetName = pTargetName;
            deletion.targetId = pTargetNode.Uuid;
            deletion.targetType = (int)pTargetNode.NodeType;
            deletion.targetLayoutVariant = (int)pTargetNode.LayoutVariant;

            pContext.AddObject(ENTITYNAME, deletion);
        }

        public static void CreateBDDeletion(Entities pContext, string pTargetName, IBDObject pTargetObject)
        {
            BDDeletion deletion = CreateBDDeletion(Guid.NewGuid());
            deletion.createdBy = Guid.Empty;
            deletion.createdDate = DateTime.Now;
            deletion.modifiedBy = Guid.Empty;
            deletion.modifiedDate = DateTime.Now;
            deletion.schemaVersion = ENTITY_SCHEMAVERSION;
            deletion.targetName = pTargetName;
            deletion.targetId = pTargetObject.Uuid;
            deletion.targetType = -1;
            deletion.targetLayoutVariant = -1;

            pContext.AddObject(ENTITYNAME, deletion);
        }

        #region Repository
        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries.  List is empty if none found.</returns>
        public static List<IBDObject> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<IBDObject> entryList = new List<IBDObject>();
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
                entryList = new List<IBDObject>(entries.ToList<BDDeletion>());
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
            List<IBDObject> newDeletionsForLocal = BDDeletion.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            foreach (IBDObject deletionEntry in newDeletionsForLocal)
            {
                BDDeletion entry = deletionEntry as BDDeletion;
                switch (entry.targetName)
                {
                    case BDNode.KEY_NAME:
                        BDNode.DeleteLocal(pDataContext, entry.targetId.Value);
                        break;
                    case BDLinkedNote.KEY_NAME:
                        BDLinkedNote.DeleteLocal(pDataContext, entry.targetId.Value);
                        break;
                    case BDLinkedNoteAssociation.KEY_NAME:
                        BDLinkedNoteAssociation.DeleteLocal(pDataContext, entry.targetId.Value);
                        break;

                    case BDTherapy.KEY_NAME:
                        BDTherapy.DeleteLocal(pDataContext, entry.targetId.Value);
                        break;
                    case BDTherapyGroup.KEY_NAME:
                        BDTherapyGroup.DeleteLocal(pDataContext, entry.targetId.Value);
                        break;

                    case BDMetadata.KEY_NAME:
                        BDMetadata.DeleteLocal(pDataContext, entry.targetId.Value);
                        break;

                    case BDSearchEntry.KEY_NAME:
                        BDSearchEntry.DeleteLocal(pDataContext, entry.targetId.Value);
                        break;
                    case BDSearchEntryAssociation.KEY_NAME:
                        BDSearchEntryAssociation.DeleteLocal(pDataContext, entry.targetId.Value);
                        break;
                }

                pDataContext.SaveChanges();

            }
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDDeletion.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if ((null != pCurrentSyncDate) && (!BDCommon.Settings.RepositoryOverwriteEnabled))
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDDeletion)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
            return syncInfo;
        }

        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDDeletion entry = BDDeletion.GetDeletionsWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDDeletion.CreateBDDeletion(uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }
            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.targetId = Guid.Parse(pAttributeDictionary [TARGETID]);
            entry.targetName = pAttributeDictionary[TARGETNAME];

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
            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.TARGETID).WithValue((null == targetId) ? Guid.Empty.ToString() : targetId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDeletion.TARGETNAME).WithValue((null == targetName) ? string.Empty : targetName).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return string.Format("{0} [{1}]", this.targetName, this.targetId); }
        }

        public string DescriptionForLinkedNote
        {
            get { throw new NotImplementedException(); }
        }

        public override string ToString()
        {
            return this.Description;
        }

    }
}
