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

                pContext.AddObject(@"BDLinkedNotes", linkedNote);
                return linkedNote;
        }

        public static BDLinkedNote CreateLinkedNote(Entities pContext, Guid pParentId, string pContextPropertyName)
        {
            BDLinkedNote linkedNote = CreateBDLinkedNote(Guid.NewGuid(), false);
            linkedNote.createdBy = Guid.Empty;
            linkedNote.createdDate = DateTime.Now;
            linkedNote.schemaVersion = 0;
            linkedNote.storageKey = string.Format("bd~{0}.txt", linkedNote.uuid.ToString().ToUpper());

            linkedNote.parentId = pParentId;
            linkedNote.contextPropertyName = pContextPropertyName;

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

                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Get all Linked Notes with the specified parentID and property name
        /// </summary>
        /// <param name="pParentId"></param>
        /// <param name="pPropertyName"></param>
        /// <returns>List of Linked Notes</returns>
        public static List<BDLinkedNote> GetLinkedNotesForParentIdAndPropertyName(Entities pContext, Guid pParentId, string pPropertyName)
        {
            List<BDLinkedNote> linkedNoteList = new List<BDLinkedNote>();
                IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                        where bdLinkedNotes.parentId == pParentId
                                                        select bdLinkedNotes);
                foreach(BDLinkedNote linkedNote in linkedNotes)
                {
                    if (linkedNote.contextPropertyName == pPropertyName)
                        linkedNoteList.Add(linkedNote);
                }
                 return linkedNoteList;                                       
        }

        /// <summary>
        /// Get all linked notes with the specified parent ID
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParentId"></param>
        /// <returns></returns>
        public static List<BDLinkedNote> GetLinkedNotesForParentId(Entities pContext, Guid pParentId)
        {
            List<BDLinkedNote> linkedNoteList = new List<BDLinkedNote>();

            IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                    where bdLinkedNotes.parentId == pParentId
                                                    select bdLinkedNotes);
            foreach (BDLinkedNote linkedNote in linkedNotes)
            {
                linkedNoteList.Add(linkedNote);
            }
            return linkedNoteList;
        }
    }
}
