using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BDEditor.Classes
{
    public class Constants
    {
        public enum BDNodeType
        {
            None = -1,
            //BDNode class
            BDChapter = 1,
            BDSection = 2,
            BDCategory = 3,
            BDSubCategory = 4,
            BDDisease = 5,
            BDPresentation = 6,
            BDPathogenGroup = 7,
            BDPathogen = 8,
            //BDTherapyGroup class
            BDTherapyGroup = 9,
            //BDTherapy class
            BDTherapy = 10
        }

        public enum LayoutVariantType
        {
            Undefined = -1,
            TreatmentRecommendation00 = 100, // Chapter
            TreatmentRecommendation01 = 101, // format specific section within chapter
            TreatmentRecommendation02 = 102,
            TreatmentRecommendation03 = 103
        }

        public const string DATETIMEFORMAT = @"yyyy-MM-dd'T'HH:mm:ss";
        public static readonly Color ACTIVELINK_COLOR = Color.LightSteelBlue;
        public static readonly Color INACTIVELINK_COLOR = SystemColors.Control;

        private Constants() { }

    }
}
