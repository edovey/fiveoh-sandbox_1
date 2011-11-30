using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDPresentation
    /// </summary>
    public partial class BDPresentation
    {
        public const string AWS_DOMAIN = @"bd_1_presentations";
        public const string ENTITYNAME = @"BDPresentations";
        public const string ENTITYNAME_FRIENDLY = @"Presentation";
        public const string PROPERTYNAME_OVERVIEW = @"Overview";
        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"pr_uuid";
        private const string SCHEMAVERSION = @"pr_schemaVersion";
        private const string CREATEDBY = @"pr_createdBy";
        private const string CREATEDDATE = @"pr_createdDate";
        private const string MODIFIEDBY = @"pr_modifiedBy";
        private const string MODIFIEDDATE = @"pr_modifiedDate";
        private const string DEPRECATED = @"pr_deprecated";
        private const string DISPLAYORDER = @"pr_displayOrder";
        private const string DISEASEID = @"pr_diseaseId";
        private const string NAME = @"pr_name";

        /// <summary>
        /// Extended Create method that sets creation date and schema version.
        /// </summary>
        /// <returns></returns>
        public static BDPresentation CreatePresentation(Entities pContext, Guid pDiseaseId)
        {
            BDPresentation presentation = CreateBDPresentation(Guid.NewGuid(), false);
            presentation.createdBy = Guid.Empty;
            presentation.createdDate = DateTime.Now;
            presentation.schemaVersion = ENTITY_SCHEMAVERSION;
            presentation.displayOrder = -1;
            presentation.name = string.Empty;
            presentation.diseaseId = pDiseaseId;

            pContext.AddObject("BDPresentations", presentation);

            BDPathogenGroup pathogenGroup = BDPathogenGroup.CreatePathogenGroup(pContext, presentation.uuid);
            pathogenGroup.displayOrder = 0;
            BDPathogenGroup.Save(pContext, pathogenGroup);

            return presentation;
        }

        /// <summary>
        /// Extended Save method that sets modifiedDate.
        /// </summary>
        /// <param name="pPresentation"></param>
        public static void Save(Entities pContext, BDPresentation pPresentation)
        {
            if (pPresentation.EntityState != EntityState.Unchanged)
            {
                if (pPresentation.schemaVersion != ENTITY_SCHEMAVERSION)
                    pPresentation.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"Presentation Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDPresentation pEntity)
        {
            // find linked notes and delete
            List<BDLinkedNoteAssociation> notes = BDLinkedNoteAssociation.GetLinkedNoteAssociationsFromParentIdAndProperty(pContext, pEntity.uuid, PROPERTYNAME_OVERVIEW);
            foreach (BDLinkedNoteAssociation a in notes)
            {
                BDLinkedNoteAssociation.Delete(pContext, a);
            }

            // find child objects and delete
            List<BDPathogenGroup> pGroups = BDPathogenGroup.GetPathogenGroupsForPresentationId(pContext, pEntity.uuid);
            foreach (BDPathogenGroup pg in pGroups)
            {
                BDPathogenGroup.Delete(pContext, pg);
            }

            // create BDDeletion record for the object to be deleted
            BDDeletion.CreateDeletion(pContext, ENTITYNAME_FRIENDLY, pEntity.uuid);
            // delete record from local data store
            pContext.DeleteObject(pEntity);
            pContext.SaveChanges();
        }

        /// <summary>
        /// Get object to delete using provided uuid, call extended delete
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid">Guid of record to delete</param>
        /// <param name="pCreateDeletion"create entry in deletion table (bool)</param>
        public static void Delete(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            BDPresentation entity = BDPresentation.GetPresentationWithId(pContext, pUuid);
            if (null != entity)
            {
                if (pCreateDeletion)
                {
                    BDPresentation.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                }
            }
        }

        /// <summary>
        /// Gets all Presentations in the model with the specified disease ID
        /// </summary>
        /// <param name="pDiseaseId"></param>
        /// <returns>List of Presentations</returns>
        public static List<BDPresentation> GetPresentationsForDiseaseId(Entities pContext, Guid pDiseaseId)
        {
            List<BDPresentation> presentationList = new List<BDPresentation>();

            IQueryable<BDPresentation> presentations = (from entry in pContext.BDPresentations
                                                        where entry.diseaseId == pDiseaseId
                                                        orderby entry.displayOrder
                                                        select entry);
            foreach (BDPresentation presentation in presentations)
            {
                presentationList.Add(presentation);
            }
            return presentationList;
        }

        /// <summary>
        /// Get Presentation with the specified ID
        /// </summary>
        /// <param name="pPresentationId"></param>
        /// <returns>BDPresentation object</returns>
        public static BDPresentation GetPresentationWithId(Entities pContext, Guid pPresentationId)
        {
            BDPresentation presentation = null;
            if (null != pPresentationId)
            {
                IQueryable<BDPresentation> entries = (from bdPresentations in pContext.BDPresentations
                                                            where bdPresentations.uuid == pPresentationId
                                                            select bdPresentations);
                if (entries.Count<BDPresentation>() > 0)
                    presentation = entries.AsQueryable().First<BDPresentation>();
            }
            return presentation;
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
        public static List<BDPresentation> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDPresentation> entryList = new List<BDPresentation>();
            IQueryable<BDPresentation> presentations;

            if (null == pUpdateDateTime)
            {
                presentations = (from entry in pContext.BDPresentations
                            select entry);
            }
            else
            {
                presentations = (from entry in pContext.BDPresentations
                            where entry.modifiedDate > pUpdateDateTime.Value
                            select entry);
            }
            if (presentations.Count() > 0)
                entryList = presentations.ToList<BDPresentation>();
            return entryList;
        }

        public static SyncInfo SyncInfo()
        {
            return new SyncInfo(AWS_DOMAIN, MODIFIEDDATE);
        }

        /// <summary>
        /// Create or update an existing BDPresentation from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDPresentation entry = BDPresentation.GetPresentationWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDPresentation.CreateBDPresentation(uuid, deprecated);
                pDataContext.AddObject("BDPresentations", entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            short displayorder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayorder;
            entry.diseaseId = Guid.Parse(pAttributeDictionary[DISEASEID]);
            entry.name = pAttributeDictionary[NAME];

            if (pSaveChanges)
                pDataContext.SaveChanges();
            
            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.DISEASEID).WithValue((null == diseaseId) ? Guid.Empty.ToString() : diseaseId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion 
    }
}
