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
    /// Extension of generated BDLinkedNote
    /// </summary>
    public partial class BDLinkedNote
    {
        public const string AWS_DOMAIN = @"bd_linkedNotes";
        public const string AWS_BUCKET = @"bdDataStore";
        public const string AWS_S3_PREFIX = @"bd~";
        public const string AWS_S3_FILEEXTENSION = @".txt";

        public const string ENTITYNAME = @"BDLinkedNotes";
        public const string ENTITYNAME_FRIENDLY = @"Linked Note";

        private const string UUID = @"ln_uuid";
        private const string SCHEMAVERSION = @"ln_schemaversion";
        private const string CREATEDBY = @"ln_createdBy";
        private const string CREATEDDATE = @"ln_createdDate";
        private const string MODIFIEDBY = @"ln_modifieddBy";
        private const string MODIFIEDDATE = @"ln_modifiedDate";
        private const string LINKEDNOTEASSOCIATIONID = @"ln_linkedNoteAssociationId";
        private const string PREVIEWTEXT = @"ln_previewText";
        private const string SCOPEID = @"ln_scopeId";
        private const string DEPRECATED = @"ln_deprecated";
        private const string SINGLEUSE = @"ln_singleUse";
        private const string STORAGEKEY = @"ln_storageKey";
        private const string DOCUMENTTEXT = @"ln_documentText";

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <returns>BDLinkedNote</returns>
        public static BDLinkedNote CreateLinkedNote(Entities pContext)
        {
            BDLinkedNote linkedNote = CreateBDLinkedNote(Guid.NewGuid(), false);
            linkedNote.createdBy = Guid.Empty;
            linkedNote.createdDate = DateTime.Now;
            linkedNote.schemaVersion = 0;
            linkedNote.documentText = string.Empty;
            linkedNote.storageKey = string.Format("{0}{1}{2}", AWS_S3_PREFIX, linkedNote.uuid.ToString().ToUpper(), AWS_S3_FILEEXTENSION);
            linkedNote.singleUse = false;
            linkedNote.previewText = string.Empty;

            pContext.AddObject(@"BDLinkedNotes", linkedNote);
            return linkedNote;
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pLinkedNote"></param>
        public static void SaveLinkedNote(Entities pContext, BDLinkedNote pLinkedNote)
        {
            if (pLinkedNote.EntityState != EntityState.Unchanged)
            {
                pLinkedNote.modifiedBy = Guid.Empty;
                pLinkedNote.modifiedDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine(@"LinkedNote Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Return the LinkedNote for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns></returns>
        public static BDLinkedNote GetLinkedNoteWithId(Entities pContext, Guid? pLinkedNoteId)
        {
            BDLinkedNote result = null;

            if (null != pLinkedNoteId)
            {
                IQueryable<BDLinkedNote> entries = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                        where bdLinkedNotes.uuid == pLinkedNoteId
                                                        select bdLinkedNotes);

                if (entries.Count<BDLinkedNote>() > 0)
                    result = entries.AsQueryable().First<BDLinkedNote>();
            }

            return result;
        }

        /// <summary>
        /// Get all linked notes with the specified linkedNoteAssociation ID
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParentId"></param>
        /// <returns></returns>
        public static List<BDLinkedNote> GetLinkedNotesForLinkedNoteAssociationId(Entities pContext, Guid pLinkedNoteAssociationId)
        {
            IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                    where bdLinkedNotes.linkedNoteAssociationId == pLinkedNoteAssociationId
                                                    select bdLinkedNotes);

            List<BDLinkedNote> linkedNoteList = linkedNotes.ToList<BDLinkedNote>();

            return linkedNoteList;
        }

        public static List<BDLinkedNote> GetLinkedNotesForScopeId(Entities pContext, Guid? pScopeId)
        {
            IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                    where bdLinkedNotes.scopeId == pScopeId && bdLinkedNotes.singleUse == false
                                                    orderby bdLinkedNotes.previewText
                                                    select bdLinkedNotes);

            List<BDLinkedNote> resultList = linkedNotes.ToList<BDLinkedNote>();
            return resultList;
        }

        #region Repository

        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<BDLinkedNote> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDLinkedNote> entryList = new List<BDLinkedNote>();
            IQueryable<BDLinkedNote> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDLinkedNotes
                            select entry);
            }
            else
            {
                entries = (from entry in pContext.BDLinkedNotes
                            where entry.modifiedDate > pUpdateDateTime.Value
                            select entry);
            }
            if (entries.Count() > 0)
                entryList = entries.ToList<BDLinkedNote>();
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
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDLinkedNote entry = BDLinkedNote.GetLinkedNoteWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDLinkedNote.CreateBDLinkedNote(uuid, deprecated);
                pDataContext.AddObject("BDLinkedNotes", entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.linkedNoteAssociationId = Guid.Parse(pAttributeDictionary[LINKEDNOTEASSOCIATIONID]);
            entry.previewText = pAttributeDictionary[PREVIEWTEXT];
            entry.scopeId = Guid.Parse(pAttributeDictionary[SCOPEID]);
            entry.singleUse = bool.Parse(pAttributeDictionary[SINGLEUSE]);
            entry.storageKey = pAttributeDictionary[STORAGEKEY];
            entry.documentText = pAttributeDictionary[DOCUMENTTEXT];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.LINKEDNOTEASSOCIATIONID).WithValue((null == linkedNoteAssociationId) ? Guid.Empty.ToString() : linkedNoteAssociationId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.SCOPEID).WithValue((null == scopeId) ? Guid.Empty.ToString() : scopeId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.PREVIEWTEXT).WithValue(previewText).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.DOCUMENTTEXT).WithValue(documentText).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.STORAGEKEY).WithValue(storageKey).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.SINGLEUSE).WithValue(singleUse.ToString()).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

    }
}
