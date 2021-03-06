﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDSearchEntryAssociation
    /// </summary>
    public partial class BDSearchEntryAssociation : IBDObject
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_searchEntryAssociations";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_searchEntryAssociations";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDSearchEntryAssociations";
        public const string ENTITYNAME_FRIENDLY = @"Search Entry Association";
        public const string KEY_NAME = @"BDSearchEntryAssociation";

        public const int ENTITY_SCHEMAVERSION = 1;

        private const string UUID = @"sa_uuid";
        private const string SCHEMAVERSION = @"sa_schemaVersion";
        private const string CREATEDBY = @"sa_createdBy";
        private const string CREATEDDATE = @"sa_createdDate";
        private const string DISPLAYORDER = @"sa_displayOrder";
        private const string SEARCHENTRYID = @"sa_searchEntryId";
        private const string SEARCHENTRYTYPE = @"sa_searchEntryType";
        private const string DISPLAYPARENTID = @"sa_displayParentId";
        private const string DISPLAYPARENTTYPE = @"sa_displayParentType";
        private const string LAYOUTVARIANT = @"sa_layoutVariant";
        private const string DISPLAYCONTEXT = @"sa_displayContext";
        private const string EDITORCONTEXT = @"sa_editorContext";
        private const string ANCHORNODEID = @"sa_anchorNodeId";

        /// <summary>
        /// Extended Create method that sets the created data and schema version. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>BDSearchEntryAssociation</returns>
        public static BDSearchEntryAssociation CreateBDSearchEntryAssociation(Entities pContext)
        {
            return CreateBDSearchEntryAssociation(pContext, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created data and schema version. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>BDSearchEntryAssociation</returns>
        public static BDSearchEntryAssociation CreateBDSearchEntryAssociation(Entities pContext, Guid pUuid)
        {
            BDSearchEntryAssociation entry = CreateBDSearchEntryAssociation(pUuid);
            entry.createdBy = Guid.Empty;
            entry.createdDate = DateTime.Now;
            entry.schemaVersion = ENTITY_SCHEMAVERSION;
            entry.displayOrder = -1;
            pContext.AddObject(ENTITYNAME, entry);
            return entry;
        }

        /// <summary>
        /// Extended create method that includes parent information.
        /// Initial create stores the EDITOR context, i.e., the context that was visible from the editor UI.  It is a more
        /// complete path as it goes to the endpoint, and its purpose is to help the user sort / order the entries in the UI.
        /// Display context for the purposes of the viewer is built & added later - during Publish / Build. 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pSearchEntryId"></param>
        /// <param name="pDisplayParentId"></param>
        /// <param name="pEditorContext"></param>
        /// <returns>BDSearchEntryAssociation</returns>
        public static BDSearchEntryAssociation CreateBDSearchEntryAssociation(Entities pDataContext,
                                                                            Guid pSearchEntryId,
                                                                            Guid pAnchorNodeId,
                                                                            string pEditorContext)
        {
            List<BDSearchEntryAssociation> associations = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryIdAndDisplayParentid(pDataContext, pSearchEntryId, pAnchorNodeId);
            if (associations.Count > 0)
                return associations[0];

            BDSearchEntryAssociation association = CreateBDSearchEntryAssociation(Guid.NewGuid());
            association.createdBy = Guid.Empty;
            association.schemaVersion = ENTITY_SCHEMAVERSION;
            association.displayOrder = -1;

            association.searchEntryId = pSearchEntryId;
            association.anchorNodeId = pAnchorNodeId;
            association.editorContext = pEditorContext;
            pDataContext.AddObject(ENTITYNAME, association);

            Save(pDataContext, association);

            return association;
        }

        public static void Save(Entities pContext, BDSearchEntryAssociation pAssociation)
        {
            if (pAssociation.EntityState != EntityState.Unchanged)
            {
                if (pAssociation.schemaVersion != ENTITY_SCHEMAVERSION)
                    pAssociation.schemaVersion = ENTITY_SCHEMAVERSION;
                
                // System.Diagnostics.Debug.WriteLine(@"SearchEntryAssociation Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Delete the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDSearchEntryAssociation pEntity)
        {
            Delete(pContext, pEntity, true);
        }

        /// <summary>
        /// Delete the record, save the context.
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        /// <param name="pCreateDeletion">IGNORED</param>
        public static void Delete(Entities pContext, BDSearchEntryAssociation pEntity, bool pCreateDeletion)
        {
            // Don't delete the iNote from here. Deletion of a iNote will delete all association entries
            if (null == pEntity) return;

            // delete record from local data store
            pContext.DeleteObject(pEntity);
            pContext.SaveChanges();
        }

        /// <summary>
        /// Get object to delete using provided uuid, call extended delete
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid">Guid of record to delete</param>
        /// <param name="pCreateDeletion">create entry in deletion table (bool)</param>
        public static void Delete(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            BDSearchEntryAssociation entity = BDSearchEntryAssociation.RetrieveSearchEntryAssociationWithId(pContext, pUuid);
            BDSearchEntryAssociation.Delete(pContext, entity, pCreateDeletion);
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
                BDSearchEntryAssociation entry = BDSearchEntryAssociation.RetrieveSearchEntryAssociationWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        public static void DeleteAll(Entities pContext)
        {
            pContext.ExecuteStoreCommand("DELETE FROM BDSearchEntryAssociations");
        }

        public static void DeleteForSearchEntryId(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            List<BDSearchEntryAssociation> children = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(pContext, pUuid);
            foreach (BDSearchEntryAssociation t in children)
            {
                BDSearchEntryAssociation.Delete(pContext, t, pCreateDeletion);
            }
        }

        public static void DeleteForAnchorNodeUuid(Entities pContext, Guid pAnchorNodeUuid)
        {
            List<BDSearchEntryAssociation> associationList = RetrieveForAnchorNodeUuid(pContext, pAnchorNodeUuid);
            foreach (BDSearchEntryAssociation association in associationList)
            {
                Delete(pContext, association, false);
            }
        }


        /// <summary>
        /// Retrieve all Search Entry Association Nodes
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>List of BDSearchEntryAssociation objects.</returns>
        public static List<IBDObject> RetrieveAll(Entities pContext)
        {
            List<IBDObject> entryList;
            IQueryable<BDSearchEntryAssociation> entries = (from bdNodes in pContext.BDSearchEntryAssociations
                                                    select bdNodes);
            entryList = new List<IBDObject>(entries.ToList<BDSearchEntryAssociation>());
            return entryList;
        }

        public static List<BDSearchEntryAssociation> RetrieveForAnchorNodeUuid(Entities pContext, Guid pAnchorNodeUuid)
        {
            List<BDSearchEntryAssociation> resultList = new List<BDSearchEntryAssociation>();
            IQueryable<BDSearchEntryAssociation> entries = (from entities in pContext.BDSearchEntryAssociations
                                                            where entities.anchorNodeId == pAnchorNodeUuid
                                                            orderby entities.displayOrder ascending
                                                            select entities);
            resultList.AddRange(entries.ToList<BDSearchEntryAssociation>());
            return resultList;
        }

        /// <summary>
        /// Returns all the SearchEntryAssociations for a searchEntry uuid, sorted in display order 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns></returns>
        public static List<BDSearchEntryAssociation> RetrieveSearchEntryAssociationsForSearchEntryId(Entities pContext, Guid pSearchEntryId)
        {
            List<BDSearchEntryAssociation> resultList = new List<BDSearchEntryAssociation>();
            IQueryable<BDSearchEntryAssociation> entries = (from entities in pContext.BDSearchEntryAssociations
                                                            where entities.searchEntryId == pSearchEntryId
                                                            orderby entities.displayOrder ascending
                                                            select entities);
            resultList.AddRange(entries.ToList<BDSearchEntryAssociation>());
            return resultList;
        }

        /// <summary>
        /// Returns all the SearchEntryAssociations for a searchEntry uuid and anchor node uuid
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pSearchEntryId"></param>
        /// <param name="pAnchorNodeId"></param>
        /// <returns>List of BDSearchEntryAssociation</returns>
        public static List<BDSearchEntryAssociation> RetrieveSearchEntryAssociationsForSearchEntryIdAndAnchorNodeId(Entities pContext, Guid pSearchEntryId, Guid pAnchorNodeId)
        {
            IQueryable<BDSearchEntryAssociation> entries = (from entities in pContext.BDSearchEntryAssociations
                                                            where entities.searchEntryId == pSearchEntryId
                                                            && entities.anchorNodeId == pAnchorNodeId
                                                            orderby entities.displayOrder ascending
                                                            select entities);
            List<BDSearchEntryAssociation> resultList = entries.ToList<BDSearchEntryAssociation>();
            return resultList;
        }

        /// <summary>
        /// Returns all the SearchEntryAssociations for a searchEntry uuid 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns></returns>
        public static List<BDSearchEntryAssociation> RetrieveSearchEntryAssociationsForSearchEntryIdAndDisplayParentid(Entities pContext, Guid pSearchEntryId, Guid pDisplayParentId)
        {
            IQueryable<BDSearchEntryAssociation> entries = (from entities in pContext.BDSearchEntryAssociations
                                                            where entities.searchEntryId == pSearchEntryId
                                                            && entities.displayParentId == pDisplayParentId
                                                            orderby entities.displayContext ascending
                                                            select entities);
            List<BDSearchEntryAssociation> resultList = entries.ToList<BDSearchEntryAssociation>();
            return resultList;
        }

        /// <summary>
        /// Return the LinkedNoteAssociation for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns></returns>
        public static BDSearchEntryAssociation RetrieveSearchEntryAssociationWithId(Entities pContext, Guid? pSearchEntryAssociationId)
        {
            BDSearchEntryAssociation result = null;

            if (null != pSearchEntryAssociationId)
            {
                IQueryable<BDSearchEntryAssociation> entries = (from entities in pContext.BDSearchEntryAssociations
                                                               where entities.uuid == pSearchEntryAssociationId
                                                               orderby entities.displayOrder
                                                               select entities);
                if (entries.Count<BDSearchEntryAssociation>() > 0)
                    result = entries.AsQueryable().First<BDSearchEntryAssociation>();
            }

            return result;
        }

 /// <summary>
 /// Reset the value of the displayParentId (the html page Uuid) during Publish
 /// </summary>
 /// <param name="pContext"></param>
        public static void ResetForRegeneration(Entities pContext)
        {
            List<BDSearchEntryAssociation> allEntries = new List<BDSearchEntryAssociation>();
            IQueryable<BDSearchEntryAssociation> entries = from entry in pContext.BDSearchEntryAssociations
                                                select entry;
            allEntries = entries.ToList<BDSearchEntryAssociation>();

            foreach (BDSearchEntryAssociation entry in allEntries)
                entry.displayParentId = null;

            pContext.SaveChanges();
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
            IQueryable<BDSearchEntryAssociation> entries;
            
            // retrieve ALL entries - this is a full refresh.
            entries = (from entry in pContext.BDSearchEntryAssociations
                      select entry);
            
            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDSearchEntryAssociation>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN,  AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDSearchEntryAssociation.RetrieveAll(pDataContext);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            return syncInfo;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.SEARCHENTRYID).WithValue((null == searchEntryId) ? Guid.Empty.ToString() : searchEntryId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.SEARCHENTRYTYPE).WithValue(string.Format(@"{0}", searchEntryType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.DISPLAYPARENTID).WithValue((null == displayParentId) ? Guid.Empty.ToString() : displayParentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.DISPLAYPARENTTYPE).WithValue(string.Format(@"{0}", displayParentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.DISPLAYCONTEXT).WithValue((null == displayContext) ? string.Empty : displayContext).WithReplace(true));
            //attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.EDITORCONTEXT).WithValue((null == editorContext) ? string.Empty : editorContext).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.LAYOUTVARIANT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.ANCHORNODEID).WithValue((null == anchorNodeId) ? Guid.Empty.ToString() : anchorNodeId.ToString().ToUpper()).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return this.uuid.ToString(); }
        }

        public override string ToString()
        {
            return this.displayContext;
        }

        public string DescriptionForLinkedNote
        {
            get { throw new NotImplementedException(); }
        }

        public BDConstants.BDNodeType NodeType
        {
            get { throw new NotImplementedException(); }
        }
    }

}
