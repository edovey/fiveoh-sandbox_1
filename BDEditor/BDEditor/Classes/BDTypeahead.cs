using System;
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
        private static AutoCompleteStringCollection antimicrobials;

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
                    string[] regimenGroups = BDRegimenGroup.RetrieveRegimenGroupNames(new BDEditor.DataModel.Entities());
                    therapyGroups.AddRange(names);
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
                    string[] names = BDTherapy.RetrieveTherapyNames(new BDEditor.DataModel.Entities());
                    string[] regimens = BDRegimen.RetrieveBDRegimenNames(new BDEditor.DataModel.Entities());
                    therapyNames.AddRange(names);
                    therapyNames.AddRange(regimens);
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
                    string[] names = BDTherapy.RetrieveTherapyDosages(new BDEditor.DataModel.Entities());
                    string[] regimens = BDRegimen.RetrieveBDRegimenDosages(new BDEditor.DataModel.Entities());
                    therapyDosages.AddRange(names);
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
                    string[] names = BDTherapy.RetrieveTherapyDurations(new BDEditor.DataModel.Entities());
                    therapyDurations.AddRange(names);
                }
                return therapyDurations;
            }
        }

        public static AutoCompleteStringCollection Antimicrobials
        {
            get
            {
                if (null == antimicrobials)
                {
                    antimicrobials = new AutoCompleteStringCollection();
                    string[] names = BDNode.RetrieveNodeNamesForType(new BDEditor.DataModel.Entities(), BDConstants.BDNodeType.BDAntimicrobial);
                    antimicrobials.AddRange(names);
                }
                return antimicrobials;
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
                    case BDConstants.BDNodeType.BDAntimicrobial:
                        {
                            if (null == antimicrobials)
                                antimicrobials = new AutoCompleteStringCollection();
                            if (!antimicrobials.Contains(pValue))
                                antimicrobials.Add(pValue);
                        }
                        break;
                    case BDConstants.BDNodeType.BDTherapyGroup:
                    case BDConstants.BDNodeType.BDRegimenGroup:
                        {
                            if (null == therapyGroups)
                                therapyGroups = new AutoCompleteStringCollection();
                            if (!therapyGroups.Contains(pValue))
                                therapyGroups.Add(pValue);
                        }
                        break;
                    case BDConstants.BDNodeType.BDTherapy:
                    case BDConstants.BDNodeType.BDRegimen:
                        {
                            if (null == therapyNames)
                                therapyNames = new AutoCompleteStringCollection();
                            if (null == therapyDosages)
                                therapyDosages = new AutoCompleteStringCollection();
                            if (null == therapyDurations)
                                therapyDurations = new AutoCompleteStringCollection();

                            if ((pEntityMember == string.Empty || pEntityMember == BDTherapy.PROPERTYNAME_THERAPY || pEntityMember == BDRegimen.PROPERTYNAME_NAME)
                                && !therapyNames.Contains(pValue))
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
                            else if (pEntityMember == BDRegimen.PROPERTYNAME_DOSAGE && !therapyDosages.Contains(pValue))
                                therapyDosages.Add(pValue);
                            else if (pEntityMember == BDRegimen.PROPERTYNAME_DURATION && !therapyDurations.Contains(pValue))
                                therapyDurations.Add(pValue);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
