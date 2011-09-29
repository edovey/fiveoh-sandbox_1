using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated class BDTherapy
    /// </summary>
    public partial class BDTherapy
    {
        /// <summary>
        /// Extended Create method that sets created date and schema version
        /// </summary>
        /// <returns></returns>
        public static BDTherapy CreateTherapy()
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                BDTherapy therapy = CreateBDTherapy(Guid.NewGuid(), false);
                therapy.createdBy = Guid.Empty;
                therapy.createdDate = DateTime.Now;
                therapy.schemaVersion = 0;

                context.AddObject("BDTherapies", therapy);

                return therapy;
            }
        }

        /// <summary>
        /// Extended Save method that sets the modified date.
        /// </summary>
        /// <param name="pTherapy"></param>
        public static void SaveTherapy(BDTherapy pTherapy)
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                pTherapy.modifiedBy = Guid.Empty;
                pTherapy.modifiedDate = DateTime.Now;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all Therapies in the model with the specified Therapy Group ID
        /// </summary>
        /// <param name="pTherapyGroupId"></param>
        /// <returns>List of Therapies</returns>
        public static List<BDTherapy> GetTherapiesForTherapyGroupId(Guid pTherapyGroupId)
        {
            List<BDTherapy> therapyList = new List<BDTherapy>();
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDTherapy> therapies = (from bdTherapies in context.BDTherapies
                                                   where bdTherapies.therapyGroupId == pTherapyGroupId
                                                   select bdTherapies);
                foreach (BDTherapy therapy in therapies)
                {
                    therapyList.Add(therapy);
                }
            }
            return therapyList;
        }

        public static BDTherapy GetTherapyWithId(Guid pTherapyId)
        {
            BDTherapy therapy;
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDTherapy> therapies = (from bdTherapies in context.BDTherapies
                                                   where bdTherapies.uuid == pTherapyId
                                                   select bdTherapies);
                therapy = therapies.AsQueryable().First<BDTherapy>();
            }
            return therapy;
        }
    }
}
