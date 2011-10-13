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
        public const string ENTITYNAME_FRIENDLY = @"Presentation";
        public const string PROPERTYNAME_OVERVIEW = @"Overview";

        public const string AWS_DOMAIN = @"bd_presentations_test";

        private const string UUID = @"pr_uuid";
        private const string SCHEMAVERSION = @"pr_schemaversion";
        private const string CREATEDBY = @"pr_createdBy";
        private const string CREATEDDATE = @"pr_createdDate";
        private const string MODIFIEDBY = @"pr_createdBy";
        private const string MODIFIEDDATE = @"pr_modifiedDate";
        private const string DEPRECATED = @"pr_deprecated";
        private const string DISPLAYORDER = @"pr_displayOrder";
        private const string DISEASEID = @"pr_diseaseId";
        private const string NAME = @"pr_name";
        private const string OVERVIEW = @"pr_overview";

        /// <summary>
        /// Extended Create method that sets creation date and schema version.
        /// </summary>
        /// <returns></returns>
        public static BDPresentation CreatePresentation(Entities pContext)
        {
            BDPresentation presentation = CreateBDPresentation(Guid.NewGuid(), false);
            presentation.createdBy = Guid.Empty;
            presentation.createdDate = DateTime.Now;
            presentation.schemaVersion = 0;

            pContext.AddObject("BDPresentations", presentation);

            BDPathogenGroup pathogenGroup = BDPathogenGroup.CreatePathogenGroup(pContext);
            pathogenGroup.presentationId = presentation.uuid;
            BDPathogenGroup.SavePathogenGroup(pContext, pathogenGroup);

            return presentation;
        }

        /// <summary>
        /// Extended Save method that sets modifiedDate.
        /// </summary>
        /// <param name="pPresentation"></param>
        public static void SavePresentation(Entities pContext, BDPresentation pPresentation)
        {
            if (pPresentation.EntityState != EntityState.Unchanged)
            {
                pPresentation.modifiedBy = Guid.Empty;
                pPresentation.modifiedDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine(@"Presentation Save");
                pContext.SaveChanges();
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

                IQueryable<BDPresentation> presentations = (from bdPresentations in pContext.BDPresentations
                                                            where bdPresentations.diseaseId == pDiseaseId
                                                            select bdPresentations);
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
            BDPresentation presentation;
                IQueryable<BDPresentation> presentations = (from bdPresentations in pContext.BDPresentations
                                                            where bdPresentations.uuid == pPresentationId
                                                            select bdPresentations);
                presentation = presentations.AsQueryable().First<BDPresentation>();
            return presentation;
        }

        #region Repository

        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<BDPresentation> GetPresentationsUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
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
        public static Guid LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDPresentation entry = BDPresentation.GetPresentationWithId(pDataContext, uuid);
            if (null == entry)
                entry = BDPresentation.CreateBDPresentation(uuid, deprecated);

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            short displayorder = short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayorder;
            entry.diseaseId = Guid.Parse(pAttributeDictionary[DISEASEID]);
            entry.name = pAttributeDictionary[NAME];
            entry.overview = pAttributeDictionary[OVERVIEW];

            BDPresentation.SavePresentation(pDataContext, entry);
            
            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.UUID).WithValue(uuid.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.DEPRECATED).WithValue(deprecated.ToString()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)));

            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.DISEASEID).WithValue((null == diseaseId) ? Guid.Empty.ToString() : diseaseId.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.NAME).WithValue(name));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPresentation.OVERVIEW).WithValue(overview));

            return putAttributeRequest;
        }
        #endregion 
    }
}
