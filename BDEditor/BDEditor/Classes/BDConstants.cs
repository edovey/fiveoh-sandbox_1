﻿using System;
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
            BDTableRow = 14,
            [Description("Pathogen Resistance")]
            BDPathogenResistance = 15,
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
            [Description("Wound Content Row")]
            TreatmentRecommendation02_WoundMgmt_ContentRow = 1021, // table 1 - wound management

            [Description("Wound Classification")]
            TreatmentRecommendation03_WoundClass = 103, // table 2 - wound classification
            [Description("Wound Header Row")]
            TreatmentRecommendation03_WoundClass_HeaderRow = 1031, // table 2 - wound classification header row
            [Description("Wound Content Row")]
            TreatmentRecommendation03_WoundClass_ContentRow = 1032, // table 2 - wound classification content row

            [Description("Pneumonia Patient Characteristic")]
            TreatmentRecommendation04_Pneumonia_I = 104,  //  table 3 - top section
            [Description("Pneumonia Content Row")]
            TreatmentRecommendation04_Pneumonia_I_ContentRow = 1041,  //  table 3 - top section

            [Description("Pneumonia Risk and Recommendation")]
            TreatmentRecommendation04_Pneumonia_II = 105,  // table 3 - bottom section
            [Description("Pneumonia Header Row")]
            TreatmentRecommendation04_Pneumonia_II_HeaderRow = 1051,  // table 3 - bottom section header row
            [Description("Pneumonia Content Row")]
            TreatmentRecommendation04_Pneumonia_II_ContentRow = 1052,  // table 3 - bottom section content row

            [Description("Treatment of Culture-Proven Peritonitis")]
            TreatmentRecommendation05_Peritonitis = 106,  // table 4
            [Description("Peritonitis Header Row")]
            TreatmentRecommendation05_Peritonitis_HeaderRow = 1061,  // table 4 header row
            [Description("Peritonitis Content Row")]
            TreatmentRecommendation05_Peritonitis_ContentRow = 1062,  // table 4 content row

            [Description("Treatment of Culture-Proven Meningitis")]
            TreatmentRecommendation06_Meningitis = 107,  // table 5
            [Description("Meningitis Header Row")]
            TreatmentRecommendation06_Meningitis_HeaderRow = 1071,  // table 5 header row
            [Description("Meningitis Content Row")]
            TreatmentRecommendation06_Meningitis_ContentRow = 1072,  // table 5 content row

            [Description("Treatment of Culture-Proven Endocarditis")]
            TreatmentRecommendation07_Endocarditis = 108, // table 6
            [Description("Endocarditis Header Row")]
            TreatmentRecommendation07_Endocarditis_HeaderRow = 1081, // table 6
            [Description("Endocarditis Content Row")]
            TreatmentRecommendation07_Endocarditis_ContentRow = 1082, // table 6

            [Description("Treatment of Opthalmic Infections")]
            TreatmentRecommendation08_Opthalmic = 109,
            [Description("Treatment of Enteric Parasitic Infections I")]
            TreatmentRecommendation09_Parasitic_I = 110,
            [Description("Treatment of Fungal Infections")]
            TreatmentRecommendation10_Fungal = 111,
            [Description("Treatment of Enteric Parasitic Infections II")]
            TreatmentRecommendation09_Parasitic_II = 112,
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
