using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;

namespace BDEditor.DataModel
{
    public enum LinkedNoteType
    {
        All = -1,
        Default = 0,
        Footnote = 1,
        EndNote = 2
    }

    /// <summary>
    /// Extension of generated BDLinkedNoteAssociation
    /// </summary>
    public partial class BDLinkedNoteAssociation
    {
        /// <summary>
        /// Extended Create method that sets the created data and schema version. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>BDLinkedNoteAssociation</returns>
        public static BDLinkedNoteAssociation CreateLinkedNoteAssociation(Entities pContext)
        {
            BDLinkedNoteAssociation linkedNoteAssociation = CreateBDLinkedNoteAssociation(Guid.NewGuid());
            linkedNoteAssociation.createdBy = Guid.Empty;
            linkedNoteAssociation.schemaVersion = 0;
            linkedNoteAssociation.linkedNoteType = (int)LinkedNoteType.Default;

            pContext.AddObject(@"BDLinkedNoteAssociations", linkedNoteAssociation);
            return linkedNoteAssociation;
        }

        /// <summary>
        /// Extended create method that includes parent information. Saves the instance.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteType"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <param name="pParentEntityName"></param>
        /// <param name="pParentId"></param>
        /// <param name="pParentEntityPropertyName"></param>
        /// <returns>BDLinkedNoteAssociation</returns>
        public static BDLinkedNoteAssociation CreateLinkedNoteAssociation(Entities pContext, 
                                                                            LinkedNoteType pLinkedNoteType, 
                                                                            Guid pLinkedNoteId, 
                                                                            string pParentEntityName, 
                                                                            Guid pParentId, 
                                                                            string pParentEntityPropertyName)
        {
            BDLinkedNoteAssociation linkedNoteAssociation = CreateBDLinkedNoteAssociation(Guid.NewGuid());
            linkedNoteAssociation.createdBy = Guid.Empty;
            linkedNoteAssociation.schemaVersion = 0;
            linkedNoteAssociation.linkedNoteType = (int)pLinkedNoteType;

            linkedNoteAssociation.linkedNoteId = pLinkedNoteId;
            linkedNoteAssociation.parentId = pParentId;
            linkedNoteAssociation.parentEntityName = pParentEntityName;
            linkedNoteAssociation.parentEntityPropertyName = pParentEntityPropertyName;

            pContext.AddObject(@"BDLinkedNoteAssociations", linkedNoteAssociation);

            SaveLinkedNoteAssociation(pContext, linkedNoteAssociation);

            return linkedNoteAssociation;
        }

        public static void SaveLinkedNoteAssociation(Entities pContext, BDLinkedNoteAssociation pLinkedNoteAssociation)
        {
            if (pLinkedNoteAssociation.EntityState != EntityState.Unchanged)
            {
                pLinkedNoteAssociation.modifiedBy = Guid.Empty;
                pLinkedNoteAssociation.modifiedDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine(@"LinkedNoteAssociation Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Returns all the LinkedNoteAssociations for a parent uuid
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParentId"></param>
        /// <returns></returns>
        public static List<BDLinkedNoteAssociation> GetLinkedNoteAssociationsFromParentIdAndProperty(Entities pContext, Guid? pParentId, string pContextPropertyName)
        {
            List<BDLinkedNoteAssociation> resultList = new List<BDLinkedNoteAssociation>();

            BDLinkedNoteAssociation existingAssociation = GetLinkedNoteAssociationForParentIdAndProperty(pContext, pParentId, pContextPropertyName);
            if (null != existingAssociation)
            {
                resultList = GetLinkedNoteAssociationsForLinkedNoteId(pContext, existingAssociation.linkedNoteId.Value);
            }

            return resultList;
        }

        public static BDLinkedNoteAssociation GetLinkedNoteAssociationForParentIdAndProperty(Entities pContext, Guid? pParentId, string pContextPropertyName)
        {
            IQueryable<BDLinkedNoteAssociation> linkedNoteAssociations = (from bdLinkedNoteAssociations in pContext.BDLinkedNoteAssociations
                                                                          where bdLinkedNoteAssociations.parentId == pParentId && bdLinkedNoteAssociations.parentEntityPropertyName == pContextPropertyName
                                                                          select bdLinkedNoteAssociations);

            BDLinkedNoteAssociation result = null;
            if (linkedNoteAssociations.Count() > 0)
            {
                result = linkedNoteAssociations.First<BDLinkedNoteAssociation>();
            }
            return result;
        }

        /// <summary>
        /// Returns all the LinkedNoteAssociations for a linkedNote uuid 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns></returns>
        public static List<BDLinkedNoteAssociation> GetLinkedNoteAssociationsForLinkedNoteId(Entities pContext, Guid pLinkedNoteId)
        {
            IQueryable<BDLinkedNoteAssociation> linkedNoteAssociations = (from bdLinkedNoteAssociations in pContext.BDLinkedNoteAssociations
                                                                          where bdLinkedNoteAssociations.linkedNoteId == pLinkedNoteId
                                                                          orderby bdLinkedNoteAssociations.parentEntityName ascending, bdLinkedNoteAssociations.parentEntityPropertyName ascending
                                                                          select bdLinkedNoteAssociations);
            List<BDLinkedNoteAssociation> resultList = linkedNoteAssociations.ToList<BDLinkedNoteAssociation>();
            return resultList;
        }

        public static string GetDescription(Entities pDataContext, Guid? pParentId, string pParentEntityName, string pParentEntityPropertyName)
        {
            string result = string.Format("{0} [{1}]", pParentEntityName, pParentEntityPropertyName);

            if (null != pParentId)
            {
                switch (pParentEntityName)
                {
                    case BDTherapy.ENTITYNAME_FRIENDLY:
                        {
                            BDTherapy therapy = BDTherapy.GetTherapyWithId(pDataContext, pParentId.Value);
                            if (null != therapy)
                            {
                                result = string.Format("{0} [{1}]", therapy.DescriptionForLinkedNote, pParentEntityPropertyName);
                            }
                        }
                        break;
                    default:
                        result = string.Format("{0} [{1}]", @"Undescribed Entity", pParentEntityPropertyName);
                        break;
                }
            }
            return result;
        }

        public string GetDescription(Entities pDataContext)
        {
            return GetDescription(pDataContext, this.parentId, this.parentEntityName, this.parentEntityPropertyName);
        }
    }

}
