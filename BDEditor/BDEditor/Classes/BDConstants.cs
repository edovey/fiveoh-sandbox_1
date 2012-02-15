using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace BDEditor.Classes
{
    public class BDConstants
    {
        public enum BDNodeType
        {
            [Description("None")]
            None = -1,
            //BDNode class
            [Description("Chapter")]
            BDChapter = 1,
            [Description("Section")]
            BDSection = 2,
            [Description("Category")]
            BDCategory = 3,
            [Description("Sub Category")]
            BDSubCategory = 4,
            [Description("Disease")]
            BDDisease = 5,
            [Description("Presentation")]
            BDPresentation = 6,
            [Description("Pathogen Group")]
            BDPathogenGroup = 7,
            [Description("Pathogen")]
            BDPathogen = 8,
            //BDTherapyGroup class
            [Description("Therapy Group")]
            BDTherapyGroup = 9,
            //BDTherapy class
            [Description("Therapy")]
            BDTherapy = 10
        }

        public enum LayoutVariantType
        {
            Undefined = -1,
            TreatmentRecommendation00 = 100, // Chapter
            TreatmentRecommendation01 = 101, // format specific section within chapter
            TreatmentRecommendation02 = 102,
            TreatmentRecommendation03 = 103,
            TreatmentRecommendation04 = 104,
            TreatmentRecommendation05 = 105
        }

        public enum SyncType
        {
            Undefined = -1,
            Default = 0,
            Publish = 2
        }

        public enum LinkedNoteType
        {
            [Description("Inline")]
            Inline = 0,
            [Description("Unmarked Comment")]
            UnmarkedComment = 1,
            [Description("Marked Comment")]
            MarkedComment = 2,
            [Description("Endnote")]
            Endnote = 3,
            [Description("Footnote")]
            Footnote = 4
        }

        public const string DATETIMEFORMAT = @"yyyy-MM-dd'T'HH:mm:ss";
        public static readonly Color ACTIVELINK_COLOR = Color.LightSteelBlue;
        public static readonly Color INACTIVELINK_COLOR = SystemColors.Control;

        private BDConstants() { }

    }
}
