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
        public const string DB_FILENAME = "BDDataStore.sdf";

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
            [Description("Subtopic")]
            BDSubtopic = 29,
            [Description("Antimicrobial Group")]
            BDAntimicrobialGroup = 30,
            [Description("Microorganism")]
            BDMicroorganism = 31,
            [Description("Microorganism Group")]
            BDMicroorganismGroup = 32,
            // BDAntimicribialRisk Class
            [Description("Antimicrobial Risk")]
            BDAntimicrobialRisk = 33,
            [Description("Condition")]
            BDCondition = 34,
            [Description("Immune Response")]
            BDResponse = 35,
            [Description("Infection Frequency")]
            BDFrequency = 36,
            [Description("Regimen")]
            BDRegimen = 37,
            //BDConfiguredEntry Class
            [Description("Configured Entry")]
            BDConfiguredEntry = 38,
            //BDCombinedEntry Class
            [Description("Combined Entry")]
            BDCombinedEntry = 39,
            [Description("Meta Decoration")]
            BDMetaDecoration = 888,
            [Description("Layout Column")]
            BDLayoutColumn = 999,
        }

        public enum LayoutVariantType
        {
            Undefined = -1,

            [Description("FrontMatter")]  // For Preface, Foreword 
            FrontMatter = 0,

            [Description("Treatment Recommendation Chapter")]
            TreatmentRecommendation00 = 100, // Chapter
            [Description("Treatment Recommendation")]
            TreatmentRecommendation01 = 101, // default layout chapter 2, selected infections
            [Description("Treatment Recommendations - Gastroenteritis")]
            TreatmentRecommendation01_Gastroenteritis = 10141,
            [Description("Treatment Recommendations - Gastroenteritis Culture Directed")]
            TreatmentRecommendation01_Gastroenteritis_CultureDirected = 101412,
            [Description("Treatment Recommendations - CNS Meningitis Table")]
            TreatmentRecommendation01_CNS_Meningitis_Table = 101500,
            [Description("Treatment Recommendations - Sepsis Without a Focus")]
            TreatmentRecommendation01_Sepsis_Without_Focus = 101600,
            [Description("Treatment Recommendations - Sepsis Without a Focus, with Risk")]
            TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk = 101601,

            [Description("Treatment Recommendations - Necrotizing Fasciitis")]
            TreatmentRecommendation02_NecrotizingFasciitis = 102, 

            [Description("Treatment Recommendations - Pneumonia Patient Characteristic")]
            TreatmentRecommendation04_Pneumonia_I = 104,  //  table 3 - top antimicrobialSection
            [Description("Treatment Recommendations - Pneumonia Patient Characteristic Content Row")]
            TreatmentRecommendation04_Pneumonia_I_ContentRow = 1041,  //  table 3 - top antimicrobialSection

            [Description("Treatment Recommendations - Pneumonia Risk and Recommendation")]
            TreatmentRecommendation04_Pneumonia_II = 105,  // table 3 - bottom antimicrobialSection
            [Description("Treatment Recommendations - Pneumonia Risk and Recommendation Header Row")]
            TreatmentRecommendation04_Pneumonia_II_HeaderRow = 1051,  // table 3 - bottom antimicrobialSection header tableRows
            [Description("Treatment Recommendations - Pneumonia Risk and Recommendation Content Row")]
            TreatmentRecommendation04_Pneumonia_II_ContentRow = 1052,  // table 3 - bottom antimicrobialSection content tableRows

            [Description("Treatment Recommendations - Culture-Proven Peritonitis")]
            TreatmentRecommendation05_CultureProvenPeritonitis = 106,  // table 4
            [Description("Treatment Recommendations - Culture-Proven Meningitis")]
            TreatmentRecommendation06_CultureProvenMeningitis = 107,  // table 5
            [Description("Treatment Recommendations - Culture-Proven Endocarditis")]
            TreatmentRecommendation07_CultureProvenEndocarditis = 108, // table 6
            [Description("Treatment Recommendations - Culture-Proven Endocarditis : Single Duration")]
            TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration = 1081, // table 6
            [Description("Treatment Recommendations - Culture-Proven Endocarditis : Viridans Group et al")]
            TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep = 1082,
            [Description("Treatment Recommendations - Culture-Proven Endocarditis:  Paediatric Doses")]
            TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics = 120,
            [Description("Treatment Recommendations - Opthalmic Infections")]
            TreatmentRecommendation08_Opthalmic = 109,
            [Description("Treatment Recommendations - Enteric Parasitic Infections I")]
            TreatmentRecommendation09_Parasitic_I = 110,
            [Description("Treatment Recommendations - Fungal Infections")]
            TreatmentRecommendation10_Fungal = 111,
            [Description("Treatment Recommendations - Fungal Infections : Amphotericin B")]
            TreatmentRecommendation10_Fungal_Amphotericin_B = 1111,
            [Description("Treatment Recommendations - Enteric Parasitic Infections II")]
            TreatmentRecommendation09_Parasitic_II = 112,

            [Description("Treatment Recommendations - Genital Ulcers")]
            TreatmentRecommendation11_GenitalUlcers = 113,
            [Description("Treatment Recommendations - Blood Culture Negative Endocarditis")]
            TreatmentRecommendation12_Endocarditis_BCNE = 114,
            [Description("Treatment Recommendations - Vesicular Lesions")]
            TreatmentRecommendation13_VesicularLesions = 115,
            [Description("Treatment Recommendations - Cellulitis : Extremities")]
            TreatmentRecommendation14_CellulitisExtremities = 116,
            [Description("Treatment Recommendations - Culture-Proven Pneumonia")]
            TreatmentRecommendation15_CultureProvenPneumonia = 117,  // table xx 
            [Description("Treatment Recommendations - Culture-Directed Infections")]
            TreatmentRecommendation16_CultureDirected = 118,
            [Description("Treatment Recommendations - Severity of Illness - Pneumonia")]
            TreatmentRecommendation17_Pneumonia = 119,
            [Description("Treatment Recommendations - PD Peritonitis - Adult")]
            TreatmentRecommendation19_Peritonitis_PD_Adult = 121,
            [Description("Treatment Recommendations - PD Peritonitis : Paediatric")]
            TreatmentRecommendation19_Peritonitis_PD_Paediatric = 122,
            [Description("Treatment Recommendations - Adult with Topic")]
            TreatmentRecommendation20_Adult_WithTopic = 123,
            [Description("Treatment Recommendations - Adult with Topic and Subtopic")]
            TreatmentRecommendation20_Adult_WithTopicAndSubtopic = 124,

            [Description("Antibiotics")]
            Antibiotics = 200, 
            [Description("Antibiotics - Clinical Antibiotic Guidelines")]
            Antibiotics_ClinicalGuidelines = 201,
            [Description("Antibiotics - Antimicrobial Spectrum")]
            Antibiotics_ClinicalGuidelines_Spectrum = 212,

            [Description("Antibiotics - Pharmacodynamics")]
            Antibiotics_Pharmacodynamics = 202,
            
            [Description("Antibiotics - Dosing and Costs")]
            Antibiotics_DosingAndCosts = 203,
            [Description("Antibiotics - Dosing and Costs: Adult")]
            Antibiotics_DosingAndCosts_Adult = 2031,
            [Description("Antibiotics - Dosing and Costs: Paediatric")]
            Antibiotics_DosingAndCosts_Paediatric = 2032,

            [Description("Antibiotics - Dosing and Monitoring")] // +Aminoglycoside 
            Antibiotics_DosingAndMonitoring = 204,
            [Description("Antibiotics - Dosing and Monitoring: Conventional")]
            Antibiotics_DosingAndMonitoring_Conventional = 2041,
            [Description("Antibiotics - Dosing and Monitoring: Vancomycin")]
            Antibiotics_DosingAndMonitoring_Vancomycin = 211,
            [Description("Antibiotics - Dosing in Renal Impairment")]
            Antibiotics_Dosing_RenalImpairment = 205,

            [Description("Antibiotics - Dosing in Hepatic Impairment")]
            Antibiotics_Dosing_HepaticImpairment = 206,
            [Description("Antibiotics - Child-Pugh Grading of Chronic Liver Disease")]
            Antibiotics_HepaticImpairment_Grading = 2061,

            [Description("Antibiotics Name Listing")]
            Antibiotics_NameListing = 207,
            [Description("Antibiotics Name Listing Header Row")]
            Antibiotics_NameListing_HeaderRow = 2071,
            [Description("Antibiotics Name Listing Content Row")]
            Antibiotics_NameListing_ContentRow = 2072,

            [Description("Antibiotics - IV to PO SWITCH Recommendations")]
            Antibiotics_Stepdown = 208,
            [Description("Antibiotics Stepdown Header Row")]
            Antibiotics_Stepdown_HeaderRow = 2081,
            [Description("Antibiotics Stepdown Content Row")]
            Antibiotics_Stepdown_ContentRow = 2082,

            [Description("Antibiotics - CSF Penetration")]
            Antibiotics_CSFPenetration = 209,
            [Description("Antibiotics - CSF Penetration : Intrathecal and/or Intraventricular Doses")]
            Antibiotics_CSFPenetration_Dosages = 2091,

            [Description("Antibiotics - B Lactam Allergy")]
            Antibiotics_BLactamAllergy = 210,
            [Description("Antibiotics - BLactam Allergy Cross Reactivity")]
            Antibiotics_BLactamAllergy_CrossReactivity = 2101,
            [Description("Antibiotics - BLactam Allergy Cross Reactivity Content Row")]
            Antibiotics_BLactamAllergy_CrossReactivity_ContentRow = 21011,

            [Description("Antibiotics - B Lactam Allergy Classifications")]
            Antibiotics_BLactamAllergy_Classifications = 2102,
            [Description("Antibiotics - B Lactam Allergy Classifications Header Row")]
            Antibiotics_BLactamAllergy_Classifications_HeaderRow = 21021,
            [Description("Antibiotics - B Lactam Allergy Classifications Content Row")]
            Antibiotics_BLactamAllergy_Classifications_ContentRow = 21022,
            
            [Description("Prophylaxis - Recommendations")]
            Prophylaxis = 300,
            [Description("Prophylaxis - Pre-Op Antibiotic Administration")]
            Prophylaxis_Surgical_PreOp = 301,

            [Description("Prophylaxis - Surgical")]
            Prophylaxis_Surgical = 302,
            [Description("Prophylaxis - Infective Endocarditis")]
            Prophylaxis_IE = 303,
            [Description("Prophylaxis - Infective Endocarditis Antibiotic Regimen")]
            Prophylaxis_IE_AntibioticRegimen = 304,
            [Description("Prophylaxis - Blood/Body Fluid Exposure")]
            Prophylaxis_FluidExposure = 305,
            [Description("Prophylaxis - Blood/Body Fluid Exposure - Risk of Infection")]
            Prophylaxis_FluidExposure_Risk = 3051,
            [Description("Prophylaxis - Blood/Body Fluid Exposure - Followup Protocol I")]
            Prophylaxis_FluidExposure_Followup_I = 3052,
            [Description("Prophylaxis - Blood/Body Fluid Exposure - Followup Protocol II")]
            Prophylaxis_FluidExposure_Followup_II = 3053,

            [Description("Prophylaxis - Sexual Assault")]
            Prophylaxis_SexualAssault = 306,
            [Description("Prophylaxis - Sexual Assault - Prophylaxis")]
            Prophylaxis_SexualAssault_Prophylaxis = 307,

            [Description("Prophylaxis - Immunization Recommendations")]
            Prophylaxis_Immunization = 314,

            [Description("Prophylaxis - Routine Immunization Programs")]
            Prophylaxis_Immunization_Routine = 308,
            [Description("Prophylaxis - Vaccines for High Risk")]
            Prophylaxis_Immunization_HighRisk = 309,
            [Description("Prophylaxis - Vaccine Details")]
            Prophylaxis_Immunization_VaccineDetail = 310,

            [Description("Prophylaxis - Communicable Diseases")]
            Prophylaxis_Communicable = 317,
            [Description("Prophylaxis - Invasive Diseases")]
            Prophylaxis_Communicable_Invasive = 311,
            [Description("Prophylaxis - Haemophilus influenaze")]
            Prophylaxis_Communicable_HaemophiliusInfluenzae = 315,
            [Description("Prophylaxis - Influenza")]
            Prophylaxis_Communicable_Influenza = 312,
            [Description("Prophylaxis - Influenza (Amantadine, No Renal Impairment)")]
            Prophylaxis_Communicable_Influenza_Amantadine_NoRenal = 3121,
            [Description("Prophylaxis - Influenza (Amantadine, Renal Impairment)")]
            Prophylaxis_Communicable_Influenza_Amantadine_Renal = 3122,
            [Description("Prophylaxis - Influenza (Oseltamivir)")]
            Prophylaxis_Communicable_Influenza_Oseltamivir = 3123,
            [Description("Prophylaxis - Influenza (Oseltamivir, Creatinine Clearance)")]
            Prophylaxis_Communicable_Influenza_Oseltamivir_Creatinine = 3124,
            [Description("Prophylaxis - Influenza (Oseltamivir, Weight)")]
            Prophylaxis_Communicable_Influenza_Oseltamivir_Weight = 3125,
            [Description("Prophylaxis - Influenza (Zanamivir)")]
            Prophylaxis_Communicable_Influenza_Zanamivir = 3126,
            [Description("Prophylaxis - Pertussis")]
            Prophylaxis_Communicable_Pertussis = 316,
            [Description("Prophylaxis - Infection Precautions")]
            Prophylaxis_InfectionPrecautions = 313,

            [Description("Dental")]
            Dental = 400,
            [Description("Dental - Prophylaxis")]
            Dental_Prophylaxis = 401,
            [Description("Dental - Prophylaxis DrugRegimens")]
            Dental_Prophylaxis_DrugRegimens = 405,
            [Description("Dental - Recommended Therapy Antimicrobial Activity")]
            Dental_RecommendedTherapy_AntimicrobialActivity = 406,
            [Description("Dental - Recommended Therapy Microorganisms")]
            Dental_RecommendedTherapy_Microorganisms = 407,
            [Description("Dental - Recommended Therapy")]
            Dental_RecommendedTherapy = 408,

            [Description(" Pregnancy / Lactation")]
            PregancyLactation = 500,
            [Description("P/L - Antimicrobials in Pregancy")]
            PregnancyLactation_Antimicrobials_Pregnancy = 501,
            [Description("P/L - Antimicrobials in Lactation")]
            PregnancyLactation_Antimicrobials_Lactation = 502,
            [Description("P/L - Exposure to Communicable Diseases during Pregnancy")]
            PregnancyLactation_Exposure_CommunicableDiseases = 503,
            [Description("P/L - Prevention of Perinatal Infection")]
            PregnancyLactation_Prevention_PerinatalInfection = 504,
            [Description("P/L - Perinatal HIV Protocol")]
            PregnancyLactation_Perinatal_HIVProtocol = 505,

            [Description("Organisms")]
            Organisms = 600,
            [Description("Organisms - Gram Stain Interpretation")]
            Organisms_GramStainInterpretation = 601,
            [Description("Organisms - Specific Body Sites")]
            Organisms_CommensalAndPathogenic = 602,
            [Description("Organisms - Therapy of Specific Organisms")]
            Organisms_Therapy = 603,
            [Description("Organisms - Therapy of Specific Organisms, with Subcategory")]
            Organisms_Therapy_with_Subcategory = 6031,
            [Description("Organisms - Antibiogram")]
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
            
            [Description("4-column Table")]
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

            [Description("Back Matter")]
            BackMatter = 9999,
        }

        public enum SyncType
        {
            Undefined = -1,
            Default = 0,
            All = 2,
            HtmlOnly = 3,
            SearchOnly = 4
        }

        public enum LinkedNoteType
        {
            [Description("Inline Overview")]
            Inline = 0,
            [Description("Unmarked Comment")]
            UnmarkedComment = 1,
            [Description("Marked Comment")]
            MarkedComment = 2,
            [Description("Endnote")]
            Endnote = 3,
            [Description("Footnote")]
            Footnote = 4,
            [Description("Reference")]
            Reference = 5,
            [Description("Internal Link")]
            InternalLink = 6,
            [Description("Reference Endnote")]
            ReferenceEndnote = 7,
            [Description("Legend")]
            Legend = 8,
            [Description("Immediate Inline")]
            Immediate = 9,
            [Description("External Link")]
            External = 10,
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

        public enum BDAttachmentMimeType
        {
            [Description("Unknown")]
            unknown = -1,
            [Description("Unsupported")]
            unsupported = 0,
            [Description("image/bmp")]
            bmp = 1,
            [Description("image/gif")]
            gif = 2,
            [Description("image/jpeg")]
            jpeg = 3,
            [Description("image/png")]
            png = 4,
            [Description("image/tiff")]
            tiff = 5,
            [Description("application/pdf")]
            pdf = 6
        }

        public enum BDJoinType
        {
            [Description("")]
            Next = 0,
            [Description("+")]
            AndWithNext = 1,
            [Description("or")]
            OrWithNext = 2,
            [Description("then")]
            ThenWithNext = 3,
            [Description("+/-")]
            WithOrWithoutWithNext = 4,
            [Description("other")]
            Other = 5,
            [Description("and/or")]
            AndOr = 6,
        }

        /// <summary>
        /// Type of content in HTML page.  Allows for lookups for same display parent ID
        /// </summary>
        public enum BDHtmlPageType
        {
            Undefined = -1,
            Data = 0,
            Overview = 1,
            Comments = 2,
            Footnote = 3,
            Reference = 4,
            Navigation = 5,
        }

        public const string DATETIMEFORMAT = @"yyyy-MM-dd'T'HH:mm:ss";
        public static readonly Color ACTIVELINK_COLOR = Color.LightSteelBlue;
        public static readonly Color INACTIVELINK_COLOR = SystemColors.Control;

        private BDConstants() { }

    }
}
