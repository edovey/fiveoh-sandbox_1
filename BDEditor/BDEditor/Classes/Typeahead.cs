using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public static class Typeahead
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
                    string [] names = BDPathogenGroup.GetPathogenGroupNames(new BDEditor.DataModel.Entities());
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
                    string[] names = BDPathogen.GetPathogenNames(new BDEditor.DataModel.Entities());
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
                    string[] names = BDTherapyGroup.GetTherapyGroupNames(new BDEditor.DataModel.Entities());
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
                    string[] names = BDTherapy.GetTherapyDoseStrings(new BDEditor.DataModel.Entities());
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
                    string[] names = BDTherapy.GetTherapyDurationStrings(new BDEditor.DataModel.Entities());
                    therapyDurations.AddRange(names);
                }
                return therapyDurations;
            }
        }

        public static void AddToCollection(string pEntityName, string pEntityMember, string pEntityValue)
        {
            if (!string.IsNullOrEmpty(pEntityValue))
            {
                switch (pEntityName)
                {
                    case BDPathogenGroup.ENTITYNAME:
                        {
                            if (!pathogenGroups.Contains(pEntityValue))
                                pathogenGroups.Add(pEntityValue);
                        }
                        break;
                    case BDPathogen.ENTITYNAME:
                        {
                            if (!pathogens.Contains(pEntityValue))
                                pathogens.Add(pEntityValue);
                        }
                        break;
                    case BDTherapyGroup.ENTITYNAME:
                        {
                            if (!therapyGroups.Contains(pEntityValue))
                                therapyGroups.Add(pEntityValue);
                        }
                        break;
                    case BDTherapy.ENTITYNAME:
                        {
                            if ((pEntityMember == string.Empty || pEntityMember == BDTherapy.PROPERTYNAME_THERAPY) && !therapyNames.Contains(pEntityValue))
                                therapyNames.Add(pEntityValue);
                            else if (pEntityMember == BDTherapy.PROPERTYNAME_DOSAGE && !therapyDosages.Contains(pEntityValue))
                                therapyDosages.Add(pEntityValue);
                            else if (pEntityMember == BDTherapy.PROPERTYNAME_DURATION && !therapyDurations.Contains(pEntityValue))
                                therapyDurations.Add(pEntityValue);
                        }
                        break;
                }
            }
        }
    }
}
