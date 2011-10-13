using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

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
                linkedNote.storageKey = string.Format("{0}{1}{2}", AWS_S3_PREFIX, linkedNote.uuid.ToString().ToUpper(), AWS_S3_FILEEXTENSION);
                linkedNote.singleUse = false;
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
        public static BDLinkedNote GetLinkedNoteForId(Entities pContext, Guid? pLinkedNoteId)
        {
            BDLinkedNote result = null;

            if (null != pLinkedNoteId)
            {
                IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                        where bdLinkedNotes.uuid == pLinkedNoteId
                                                        select bdLinkedNotes);

                result = linkedNotes.AsQueryable().First<BDLinkedNote>();
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
    }
}
