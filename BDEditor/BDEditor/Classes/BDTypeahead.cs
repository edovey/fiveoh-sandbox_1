﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public static class BDTypeahead
    {
        private static AutoCompleteStringCollection pathogenGroups;
        private static AutoCompleteStringCollection pathogens;
        private static AutoCompleteStringCollection therapyGroups;
        private static AutoCompleteStringCollection therapyNames;
        private static AutoCompleteStringCollection therapyDosages;
        private static AutoCompleteStringCollection therapyDurations;

        public static AutoCompleteStringCollection PathogenGroups
        {
            get
            {
                if (null == pathogenGroups)
                {
                    pathogenGroups = new AutoCompleteStringCollection();
                    string [] names = BDNode.RetrieveNodeNamesForType(new BDEditor.DataModel.Entities(), BDConstants.BDNodeType.BDPathogenGroup);
                    pathogenGroups.AddRange(names);
                }
                return pathogenGroups;
            }
        }

        public static AutoCompleteStringCollection Pathogens
        {
            get
            {
                if (null == pathogens)
                {
                    pathogens = new AutoCompleteStringCollection();
                    string[] names = BDNode.RetrieveNodeNamesForType(new BDEditor.DataModel.Entities(), BDConstants.BDNodeType.BDPathogen);
                    pathogens.AddRange(names);
                }
                return pathogens;
            }
        }

        public static AutoCompleteStringCollection TherapyGroups
        {
            get
            {
                if (null == therapyGroups)
                {
                    therapyGroups = new AutoCompleteStringCollection();
                    string[] names = BDTherapyGroup.RetrieveTherapyGroupNames(new BDEditor.DataModel.Entities());
                    therapyGroups.AddRange(names);
                }
                return therapyGroups;
            }
        }

        public static AutoCompleteStringCollection TherapyNames
        {
            get
            {
                if (null == therapyNames)
                {
                    therapyNames = new AutoCompleteStringCollection();
                    string[] names = BDTherapy.GetTherapyNames(new BDEditor.DataModel.Entities());
                    therapyNames.AddRange(names);
                }
                return therapyNames;
            }
        }

        public static AutoCompleteStringCollection TherapyDosages
        {
            get
            {
                if (null == therapyDosages)
                {
                    therapyDosages = new AutoCompleteStringCollection();
                    string[] names = BDTherapy.GetTherapyDosages(new BDEditor.DataModel.Entities());
                    therapyDosages.AddRange(names);
                }
                return therapyDosages;
            }
        }

        public static AutoCompleteStringCollection TherapyDurations
        {
            get
            {
                if (null == therapyDurations)
                {
                    therapyDurations = new AutoCompleteStringCollection();
                    string[] names = BDTherapy.GetTherapyDurations(new BDEditor.DataModel.Entities());
                    therapyDurations.AddRange(names);
                }
                return therapyDurations;
            }
        }

        public static void AddToCollection(BDConstants.BDNodeType pNodeType, string pEntityMember, string pValue)
        {
            if (!string.IsNullOrEmpty(pValue))
            {
                switch (pNodeType)
                {
                    case BDConstants.BDNodeType.BDPathogenGroup:
                        {
                            if(null == pathogenGroups)
                                pathogenGroups = new AutoCompleteStringCollection();
                            if (!pathogenGroups.Contains(pValue))
                                pathogenGroups.Add(pValue);
                        }
                        break;
                    case BDConstants.BDNodeType.BDPathogen:
                        {
                            if (null == pathogens)
                                pathogens = new AutoCompleteStringCollection();
                            if (!pathogens.Contains(pValue))
                                pathogens.Add(pValue);
                        }
                        break;
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        {
                            if (null == therapyGroups)
                                therapyGroups = new AutoCompleteStringCollection();
                            if (!therapyGroups.Contains(pValue))
                                therapyGroups.Add(pValue);
                        }
                        break;
                    case BDConstants.BDNodeType.BDTherapy:
                        {
                            if (null == therapyNames)
                                therapyNames = new AutoCompleteStringCollection();
                            if (null == therapyDosages)
                                therapyDosages = new AutoCompleteStringCollection();
                            if (null == therapyDurations)
                                therapyDurations = new AutoCompleteStringCollection();

                            if ((pEntityMember == string.Empty || pEntityMember == BDTherapy.PROPERTYNAME_THERAPY) && !therapyNames.Contains(pValue))
                                therapyNames.Add(pValue);
                            else if (pEntityMember == BDTherapy.PROPERTYNAME_DOSAGE && !therapyDosages.Contains(pValue))
                                therapyDosages.Add(pValue);
                            else if (pEntityMember == BDTherapy.PROPERTYNAME_DURATION && !therapyDurations.Contains(pValue))
                                therapyDurations.Add(pValue);
                            else if (pEntityMember == BDTherapy.PROPERTYNAME_DOSAGE_1 && !therapyDosages.Contains(pValue))
                                therapyDosages.Add(pValue);
                            else if (pEntityMember == BDTherapy.PROPERTYNAME_DOSAGE_2 && !therapyDosages.Contains(pValue))
                                therapyDosages.Add(pValue);
                            else if (pEntityMember == BDTherapy.PROPERTYNAME_DURATION_1 && !therapyDurations.Contains(pValue))
                                therapyDurations.Add(pValue);
                            else if (pEntityMember == BDTherapy.PROPERTYNAME_DURATION_2 && !therapyDurations.Contains(pValue))
                                therapyDurations.Add(pValue);
                        }
                        break;
                }
            }
        }
    }
}