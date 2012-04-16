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
            [Description("Undefined")]
            Undefined = -1,
            [Description("None")]
            None = 0,
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
            BDTherapy = 10,
            // BDLinkedNote class
            [Description("Linked Note")]
            BDLinkedNote = 11,
            [Description("Table")]
            BDTable = 12,
            [Description("Table Section")]
            BDTableSection = 13,
            // BDTableRow class
            [Description("Table Row")]
            BDTableRow = 14
        }

        public enum LayoutVariantType
        {
            Undefined = -1,
            [Description("Treatment Recommendation Chapter")]
            TreatmentRecommendation00 = 100, // Chapter
            [Description("Treatment Recommendation")]
            TreatmentRecommendation01 = 101, // default layout chapter 2, selected infections
            [Description("Wound Management")]
            TreatmentRecommendation02_WoundMgmt = 102, // table 1 - wound management
            [Description("Wound Classification")]
            TreatmentRecommendation03_WoundClass = 103, // table 2 - wound classification
            [Description("Pneumonia Severity")]
            TreatmentRecommendation04 = 104,
            [Description("Treatment of Culture-Proven Peritonitis")]
            TreatmentRecommendation05 = 105,
            [Description("Treatment of Culture-Proven Meningitis")]
            TreatmentRecommendation06 = 106,
            [Description("Treatment of Culture-Proven Endocarditis")]
            TreatmentRecommendation07 = 107
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

        public enum TableCellAlignment
        {
            [Description("Undefined")]
            Undefined = -1,
            [Description("Left Justified")]
            LeftJustified = 0,
            [Description("Centred")]
            Centred = 1,
            [Description("Right Justified")]
            RightJustified = 2
        }

        public enum TableRowLayoutVariant
        {
            [Description("Undefined")]
            Undefined = -1,
            [Description("Header")]
            Header = 0,
            [Description("Section")]
            Section = 1,
            [Description("Content")]
            Content = 2
        }

        public const string DATETIMEFORMAT = @"yyyy-MM-dd'T'HH:mm:ss";
        public static readonly Color ACTIVELINK_COLOR = Color.LightSteelBlue;
        public static readonly Color INACTIVELINK_COLOR = SystemColors.Control;

        private BDConstants() { }

    }
}
