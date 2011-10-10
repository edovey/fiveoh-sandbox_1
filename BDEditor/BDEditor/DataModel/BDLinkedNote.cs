﻿using System;
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
                linkedNote.storageKey = string.Format("bd~{0}.txt", linkedNote.uuid.ToString().ToUpper());

                pContext.AddObject(@"BDLinkedNotes", linkedNote);
                return linkedNote;
        }

        public static BDLinkedNote CreateLinkedNote(Entities pContext, LinkedNoteType pLinkedNoteType, string pParentEntityName, Guid pParentId, string pParentEntityPropertyName)
        {
            BDLinkedNote linkedNote = CreateLinkedNote(pContext);

            BDLinkedNoteAssociation linkedNoteAssociation = BDLinkedNoteAssociation.CreateLinkedNoteAssociation(pContext, pLinkedNoteType, linkedNote.uuid, pParentEntityName, pParentId, pParentEntityPropertyName);

            linkedNote.linkedNoteAssociationId = linkedNoteAssociation.uuid;

            SaveLinkedNote(pContext, linkedNote);
            
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

        /*
        /// <summary>
        /// Get all Linked Notes with the specified parentID and property name
        /// </summary>
        /// <param name="pParentId"></param>
        /// <param name="pPropertyName"></param>
        /// <returns>List of Linked Notes</returns>
        public static BDLinkedNote GetLinkedNoteForParentIdAndPropertyName(Entities pContext, Guid pParentId, string pPropertyName)
        {
            IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                    where bdLinkedNotes.parentId == pParentId
                                                    select bdLinkedNotes);

            if (linkedNotes.Count() == 0)
                return null;
            else
            {
                foreach(BDLinkedNote note in linkedNotes)
                {
                    if (note.contextPropertyName == pPropertyName)
                        return note;
                }
            }
            return null;                                       
        }

        /// <summary>
        /// Get linked note with the specified parent ID
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParentId"></param>
        /// <returns></returns>
        public static BDLinkedNote GetLinkedNoteForParentId(Entities pContext, Guid pParentId)
        {
            BDLinkedNote linkedNote;

            IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                    where bdLinkedNotes.parentId == pParentId
                                                    select bdLinkedNotes);
            if (linkedNotes.Count() == 0)
                return null;
            else
            {
                linkedNote = linkedNotes.AsQueryable().First<BDLinkedNote>();
            }
            return linkedNote;                                       
        }
        */

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

                if (linkedNotes.Count() > 0)
                {
                    result = linkedNotes.First();
                }
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
            //List<BDLinkedNote> linkedNoteList = new List<BDLinkedNote>();

            IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                    where bdLinkedNotes.linkedNoteAssociationId == pLinkedNoteAssociationId
                                                    select bdLinkedNotes);

            List<BDLinkedNote> linkedNoteList = linkedNotes.ToList<BDLinkedNote>();
            //foreach (BDLinkedNote linkedNote in linkedNotes)
            //{
            //    linkedNoteList.Add(linkedNote);
            //}
            return linkedNoteList;
        }

        public static List<BDLinkedNote> GetLinkedNotesForScopeId(Entities pContext, Guid? pScopeId)
        {
            IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                    where bdLinkedNotes.scopeId == pScopeId
                                                    orderby bdLinkedNotes.previewText
                                                    select bdLinkedNotes);

            List<BDLinkedNote> resultList = linkedNotes.ToList<BDLinkedNote>();
            return resultList;
        }
    }
}
