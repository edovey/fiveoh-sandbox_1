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
    /// Extension of generated class BDTherapy
    /// </summary>
    public partial class BDTherapy
    {
        public enum TherapyJoinType
        {
            None = 0,
            AndWithNext = 1,
            OrWithNext = 2
        }

        /// <summary>
        /// Extended Create method that sets created date and schema version
        /// </summary>
        /// <returns></returns>
        public static BDTherapy CreateTherapy(Entities pContext)
        {
            BDTherapy therapy = CreateBDTherapy(Guid.NewGuid(), false);
            therapy.createdBy = Guid.Empty;
            therapy.createdDate = DateTime.Now;
            therapy.schemaVersion = 0;
            therapy.therapyJoinType = (int)TherapyJoinType.None;

            pContext.AddObject("BDTherapies", therapy);

            return therapy;
        }

        /// <summary>
        /// Extended Save method that sets the modified date.
        /// </summary>
        /// <param name="pTherapy"></param>
        public static void SaveTherapy(Entities pContext, BDTherapy pTherapy)
        {
            if (pTherapy.EntityState != EntityState.Unchanged)
            {
                pTherapy.modifiedBy = Guid.Empty;
                pTherapy.modifiedDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine(@"Therapy Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all Therapies in the model with the specified Therapy Group ID
        /// </summary>
        /// <param name="pTherapyGroupId"></param>
        /// <returns>List of Therapies</returns>
        public static List<BDTherapy> GetTherapiesForTherapyGroupId(Entities pContext, Guid pTherapyGroupId)
        {
            List<BDTherapy> therapyList = new List<BDTherapy>();

            IQueryable<BDTherapy> therapies = (from bdTherapies in pContext.BDTherapies
                                                where bdTherapies.therapyGroupId == pTherapyGroupId
                                                select bdTherapies);
            foreach (BDTherapy therapy in therapies)
            {
                therapyList.Add(therapy);
            }
            return therapyList;
        }

        public static BDTherapy GetTherapyWithId(Entities pContext, Guid pTherapyId)
        {
            BDTherapy therapy;

            IQueryable<BDTherapy> therapies = (from bdTherapies in pContext.BDTherapies
                                                where bdTherapies.uuid == pTherapyId
                                                select bdTherapies);
            therapy = therapies.AsQueryable().First<BDTherapy>();

            return therapy;
        }
    }
}