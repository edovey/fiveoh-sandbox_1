using System;
using System.Collections.Generic;
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
        public static BDLinkedNote CreateLinkedNote()
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                BDLinkedNote linkedNote = CreateBDLinkedNote(Guid.NewGuid(), false);
                linkedNote.createdBy = Guid.Empty;
                linkedNote.createdDate = DateTime.Now;
                linkedNote.schemaVersion = 0;

                context.AddObject(@"BDLinkedNote", linkedNote);
                return linkedNote;
            }
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pLinkedNote"></param>
        public static void SaveLinkedNote(BDLinkedNote pLinkedNote)
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                pLinkedNote.modifiedBy = Guid.Empty;
                pLinkedNote.modifiedDate = DateTime.Now;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Get all Linked Notes with the specified parentID and property name
        /// </summary>
        /// <param name="pParentId"></param>
        /// <param name="pPropertyName"></param>
        /// <returns>List of Linked Notes</returns>
        public static List<BDLinkedNote> GetLinkedNotesForParentIdAndPropertyName(Guid pParentId, string pPropertyName)
        {
            List<BDLinkedNote> linkedNoteList = new List<BDLinkedNote>();
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in context.BDLinkedNotes
                                                        where bdLinkedNotes.parentId == pParentId
                                                        select bdLinkedNotes);
                foreach(BDLinkedNote linkedNote in linkedNotes)
                {
                    if (linkedNote.contextPropertyName == pPropertyName)
                        linkedNoteList.Add(linkedNote);
                }
                 return linkedNoteList;                                       
            }
        }
    }
}
