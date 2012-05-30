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
            [Description("Subcategory")]
            BDSubcategory = 4,
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
            [Description("Table Subsection")]
            BDTableSubsection = 14,
            // BDTableRow class
            [Description("Table Row")]
            BDTableRow = 15,
            [Description("Pathogen Resistance")]
            BDPathogenResistance = 16,
            [Description("Dosage Group")]
            BDDosageGroup = 17,
            // BDDosage class
            [Description("Dosage")]
            BDDosage = 18,
            [Description("Antimicrobial")]
            BDAntimicrobial = 19,
            // BDPrecaution class
            [Description("Precaution")]
            BDPrecaution = 20,
            [Description("Subsection")]
            BDSubsection = 21,
            [Description("Topic")]
            BDTopic = 22,
            // BDTableCell class
            [Description("Table Cell")]
            BDTableCell = 23,
            [Description("Table Group")]
            BDTableGroup = 24,
            // BDAttachment class
            [Description("Attachment")]
            BDAttachment = 25,
            [Description("Surgery Group")]
            BDSurgeryGroup = 26,
            [Description("Surgery")]
            BDSurgery = 27,
            [Description("Surgery Classification")]
            BDSurgeryClassification = 28,
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

            [Description("Antibiotics")]
            Antibiotics = 200, 
            [Description("Clinical Antibiotic Guidelines")]
            Antibiotics_ClinicalGuidelines = 201,

            [Description("Pharmacodynamics")]
            Antibiotics_Pharmacodynamics = 202,
            [Description("Antibiotics Dosing and Costs")]
            Antibiotics_DosingAndCosts = 203,
            [Description("Antibiotics Dosing and Monitoring")]
            Antibiotics_DosingAndMonitoring = 204,
            [Description("Antibiotics Dosing in Renal Impairment")]
            Antibiotics_Dosing_RenalImpairment = 205,
            [Description("Antibiotics Dosing in Hepatic Impairment")]
            Antibiotics_Dosing_HepaticImpairment = 206,
            [Description("Antibiotics Name Listing")]
            Antibiotics_NameListing = 207,
            [Description("Antibiotics Stepdown Recommendations")]
            Antibiotics_Stepdown = 208,
            [Description("Antibiotics CSF Penetration")]
            Antibiotics_CSFPenetration = 209,
            [Description("Antibiotics B Lactam Allergy")]
            Antibiotics_BLactamAllergy = 210,
            
            [Description("Prophylaxis Recommendations")]
            Prophylaxis = 300, 

            [Description("Dental")]
            Dental = 400,
            [Description("Dental Prophylaxis")]
            Dental_Prophylaxis = 401,
            [Description("Dental Prophylaxis Header Row")]
            Dental_Prophylaxis_HeaderRow = 4011,
            [Description("Dental Prophylaxis Content Row")]
            Dental_Prophylaxis_ContentRow = 4012,
            [Description("Dental Prophylaxis Endocarditis")]
            Dental_Prophylaxis_Endocarditis = 402,
            [Description("Dental Prophylaxis Endocarditis Header Row")]
            Dental_Prophylaxis_Endocarditis_HeaderRow = 4021,
            [Description("Dental Prophylaxis Endocarditis Content Row")]
            Dental_Prophylaxis_Endocarditis_ContentRow = 4022,
            [Description("Dental Prophylaxis Endocarditis Antibiotic Regimen")]
            Dental_Prophylaxis_Endocarditis_AntibioticRegimen = 403,
            [Description("Dental Prophylaxis Endocarditis Antibiotic Regimen Header Row")]
            Dental_Prophylaxis_Endocarditis_AntibioticRegimen_HeaderRow = 4031,
            [Description("Dental Prophylaxis Endocarditis Antibiotic Regimen Content Row")]
            Dental_Prophylaxis_Endocarditis_AntibioticRegimen_ContentRow = 4032,
            [Description("Dental Prophylaxis Prosthetics")]
            Dental_Prophylaxis_Prosthetics = 404,
            [Description("Dental Prophylaxis Prosthetics Header Row")]
            Dental_Prophylaxis_Prosthetics_HeaderRow = 4041,
            [Description("Dental Prophylaxis Prosthetics Content Row")]
            Dental_Prophylaxis_Prosthetics_ContentRow = 4042,
            [Description("Dental Prophylaxis DrugRegimens")]
            Dental_Prophylaxis_DrugRegimens = 405,
            [Description("Dental Prophylaxis DrugRegimens Header Row")]
            Dental_Prophylaxis_DrugRegimens_HeaderRow = 4051,
            [Description("Dental Prophylaxis DrugRegimens Content Row")]
            Dental_Prophylaxis_DrugRegimens_ContentRow = 4052,
            [Description("Dental Recommended Therapy Antimicrobial Activity")]
            Dental_RecommendedTherapy_AntimicrobialActivity = 406,
            [Description("Dental Recommended Therapy Antimicrobial Activity Header Row")]
            Dental_RecommendedTherapy_AntimicrobialActivity_HeaderRow = 4061,
            [Description("Dental Recommended Therapy Antimicrobial Activity Content Row")]
            Dental_RecommendedTherapy_AntimicrobialActivity_ContentRow = 4062,
            [Description("Dental Recommended Therapy Microorganisms")]
            Dental_RecommendedTherapy_Microorganisms = 407,
            [Description("Dental Recommended Therapy Microorganisms Content Row")]
            Dental_RecommendedTherapy_Microorganisms_ContentRow = 4071,
            [Description("Dental Recommended Therapy")]
            Dental_RecommendedTherapy = 408,

            [Description(" Pregnancy / Lactation")]
            PregancyLactation = 500,
            [Description("Antimicrobials in Pregancy")]
            PregnancyLactation_Antimicrobials_Pregnancy = 501,
            [Description("Antimicrobials in Pregancy Header Row")]
            PregnancyLactation_Antimicrobials_Pregnancy_HeaderRow = 5011,
            [Description("Antimicrobials in Pregancy Content Row")]
            PregnancyLactation_Antimicrobials_Pregnancy_ContentRow = 5012,
            [Description("Antimicrobials in Lactation")]
            PregnancyLactation_Antimicrobials_Lactation = 502,
            [Description("Antimicrobials in Lactation Header Row")]
            PregnancyLactation_Antimicrobials_Lactation_HeaderRow = 5021,
            [Description("Antimicrobials in Lactation Content Row")]
            PregnancyLactation_Antimicrobials_Lactation_ContentRow = 5022,
            [Description("Exposure to Communicable Diseases during Pregnancy")]
            PregnancyLactation_Exposure_CommunicableDiseases = 503,
            [Description("Exposure to Communicable Diseases during Pregnancy Header Row")]
            PregnancyLactation_Exposure_CommunicableDiseases_HeaderRow = 5031,
            [Description("Exposure to Communicable Diseases during Pregnancy Content Row")]
            PregnancyLactation_Exposure_CommunicableDiseases_ContentRow = 5032,
            [Description("Prevention of Perinatal Infection")]
            PregnancyLactation_Prevention_PerinatalInfection = 504,

            [Description("Organisms")]
            Organisms = 600,
            [Description("Gram Stain Interpretation")]
            Organisms_GramStainInterpretation = 601,
            [Description("Gram Stain Interpretation Header Row")]
            Organisms_GramStainInterpretation_HeaderRow = 6011,
            [Description("Gram Stain Interpretation Content Row")]
            Organisms_GramStainInterpretation_ContentRow = 6012,
            [Description("Organisms for Specific Body Sites")]
            Organisms_CommensalAndPathogenic = 602,
            [Description("Organisms for Specific Body Sites Header Row")]
            Organisms_CommensalAndPathogenic_HeaderRow = 6021,
            [Description("Organisms for Specific Body Sites Content Row")]
            Organisms_CommensalAndPathogenic_ContentRow = 6022,
            [Description("Empiric Therapy of Specific Organisms")]
            Organisms_EmpiricTherapy = 603,
            [Description("Empiric Therapy of Specific Organisms Header Row")]
            Organisms_EmpiricTherapy_HeaderRow = 6031,
            [Description("Empiric Therapy of Specific Organisms Content Row")]
            Organisms_EmpiricTherapy_ContentRow = 6032,
            [Description("Antibiogram")]
            Organisms_Antibiogram = 604,

            [Description("2-column Table")]
            Table_2_Column = 900,
            [Description("2-column Table Header Row")]
            Table_2_Column_HeaderRow = 9001,
            [Description("2-column Table Content Row")]
            Table_2_Column_ContentRow = 9002,

            [Description("3-column Table")]
            Table_3_Column = 901,
            [Description("3-column Table Header Row")]
            Table_3_Column_HeaderRow = 9011,
            [Description("3-column Table Content Row")]
            Table_3_Column_ContentRow = 9012,
            
            [Description("4 column Table")]
            Table_4_Column = 902,
            [Description("4-column Table Header Row")]
            Table_4_Column_HeaderRow = 9021,
            [Description("4-column Table Content Row")]
            Table_4_Column_ContentRow = 9022,
            
            [Description("5-column Table")]
            Table_5_Column = 903,
            [Description("5-column Table Header Row")]
            Table_5_Column_HeaderRow = 9031,
            [Description("5-column Table Content Row")]
            Table_5_Column_ContentRow = 9032,
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

        public const string DATETIMEFORMAT = @"yyyy-MM-dd'T'HH:mm:ss";
        public static readonly Color ACTIVELINK_COLOR = Color.LightSteelBlue;
        public static readonly Color INACTIVELINK_COLOR = SystemColors.Control;

        private BDConstants() { }

    }
}
