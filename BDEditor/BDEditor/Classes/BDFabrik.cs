﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.DataModel;
using BDEditor.Views;

namespace BDEditor.Classes
{
    public class BDFabrik
    {
        private static volatile BDFabrik instance;
        private static object syncRoot = new object();

        private BDFabrik() { }

        #region Singleton
        public static BDFabrik Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (syncRoot)
                    {
                        if (null == instance)
                        {
                            instance = new BDFabrik();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        public static void SaveNode(Entities pDataContext, IBDNode pNode)
        {
            if (null != pNode)
            {
                switch (pNode.NodeType)
                {
                    case BDConstants.BDNodeType.BDAntimicrobialRisk:
                        BDAntimicrobialRisk risk = pNode as BDAntimicrobialRisk;
                        BDAntimicrobialRisk.Save(pDataContext, risk);
                        break;
                    case BDConstants.BDNodeType.BDAttachment:
                        BDAttachment attachment = pNode as BDAttachment;
                        BDAttachment.Save(pDataContext, attachment);
                        break;
                    case BDConstants.BDNodeType.BDCombinedEntry:
                        BDCombinedEntry combinedEntry = pNode as BDCombinedEntry;
                        BDCombinedEntry.Save(pDataContext, combinedEntry);
                        break;
                    case BDConstants.BDNodeType.BDConfiguredEntry:
                        BDConfiguredEntry configuredEntry = pNode as BDConfiguredEntry;
                        BDConfiguredEntry.Save(pDataContext, configuredEntry);
                        break;
                    case BDConstants.BDNodeType.BDDosage:
                        BDDosage dosage = pNode as BDDosage;
                        BDDosage.Save(pDataContext, dosage);
                        break;
                    case BDConstants.BDNodeType.BDPrecaution:
                        BDPrecaution precaution = pNode as BDPrecaution;
                        BDPrecaution.Save(pDataContext, precaution);
                        break;
                    case BDConstants.BDNodeType.BDRegimenGroup:
                        BDRegimenGroup regimenGroup = pNode as BDRegimenGroup;
                        BDRegimenGroup.Save(pDataContext, regimenGroup);
                        break;
                    case BDConstants.BDNodeType.BDRegimen:
                        BDRegimen regimen = pNode as BDRegimen;
                        BDRegimen.Save(pDataContext, regimen);
                        break;
                    case BDConstants.BDNodeType.BDTherapy:
                        BDTherapy therapy = pNode as BDTherapy;
                        BDTherapy.Save(pDataContext, therapy);
                        break;
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        BDTherapyGroup therapyGroup = pNode as BDTherapyGroup;
                        BDTherapyGroup.Save(pDataContext, therapyGroup);
                        break;
                    case BDConstants.BDNodeType.BDTableRow:
                        BDTableRow tableRow = pNode as BDTableRow;
                        BDTableRow.Save(pDataContext, tableRow);
                        break;

                    case BDConstants.BDNodeType.None:
                        // Do nothing
                        break;

                    case BDConstants.BDNodeType.BDCategory:
                    case BDConstants.BDNodeType.BDChapter:
                    case BDConstants.BDNodeType.BDDisease:
                    case BDConstants.BDNodeType.BDMicroorganismGroup:
                    case BDConstants.BDNodeType.BDMicroorganism:
                    case BDConstants.BDNodeType.BDOrganism:
                    case BDConstants.BDNodeType.BDPathogen:
                    case BDConstants.BDNodeType.BDPathogenGroup:
                    case BDConstants.BDNodeType.BDPresentation:
                    case BDConstants.BDNodeType.BDSection:
                    case BDConstants.BDNodeType.BDSubcategory:
                    case BDConstants.BDNodeType.BDTable:
                    case BDConstants.BDNodeType.BDTableSection:
                    default:
                        BDNode node = pNode as BDNode;
                        BDNode.Save(pDataContext, node);
                        break;
                }
            }
        }

        public static List<IBDNode> SearchNodesForText(Entities pDataContext, List<int> pNodes, string pText)
        {
            List<IBDNode> nodeList = new List<IBDNode>();

            foreach (int nodeType in pNodes)
            {
                switch (nodeType)
                {
                    case (int)BDConstants.BDNodeType.BDTherapyGroup:
                        nodeList.AddRange(BDTherapyGroup.RetrieveTherapyGroupsWithNameContainingString(pDataContext, pText));
                        break;
                    case (int)BDConstants.BDNodeType.BDTherapy:
                        {
                            nodeList.AddRange(BDTherapy.RetrieveTherapiesWithDosageContainingString(pDataContext, pText));
                            nodeList.AddRange(BDTherapy.RetrieveTherapiesWithNameContainingString(pDataContext, pText));
                            nodeList.AddRange(BDTherapy.RetrieveTherapiesWithDurationContainingString(pDataContext, pText));
                        }
                        break;
                    case (int)BDConstants.BDNodeType.BDTableRow:
                        // contains no text on its own - nothing to do for a table tableRows
                        break;
                    case (int)BDConstants.BDNodeType.BDDosage:
                        nodeList.AddRange(BDDosage.RetrieveDosagesWithDosageContainingString(pDataContext, pText));
                        break;
                    case (int)BDConstants.BDNodeType.BDPrecaution:
                        nodeList.AddRange(BDPrecaution.RetrievePrecautionsContainingString(pDataContext, pText));
                        break;
                    case (int)BDConstants.BDNodeType.BDTableCell:
                        nodeList.AddRange(BDTableCell.RetrieveTableCellsContainingString(pDataContext, pText));
                        break;
                    case (int)BDConstants.BDNodeType.BDAttachment:
                        nodeList.AddRange(BDAttachment.RetrieveAttachmentsWithNameContainingString(pDataContext, pText));
                        break;
                    case (int)BDConstants.BDNodeType.BDAntimicrobialRisk:
                        nodeList.AddRange(BDAntimicrobialRisk.RetrieveAntimicrobialRisksContainingString(pDataContext, pText));
                        break;
                    case (int)BDConstants.BDNodeType.BDConfiguredEntry:
                        nodeList.AddRange(BDConfiguredEntry.RetrieveConfiguredEntriesContainingString(pDataContext, pText));
                        break;
                    case (int)BDConstants.BDNodeType.BDCombinedEntry:
                        nodeList.AddRange(BDCombinedEntry.RetrieveCombinedEntryContainingString(pDataContext, pText));
                        break;
                    case (int)BDConstants.BDNodeType.BDRegimenGroup:
                        nodeList.AddRange(BDRegimenGroup.RetrieveRegimenGroupsWithNameContainingString(pDataContext, pText));
                        break;
                    case (int)BDConstants.BDNodeType.BDRegimen:
                        nodeList.AddRange(BDRegimen.RetrieveBDRegimensWithNameContainingString(pDataContext, pText));
                        nodeList.AddRange(BDRegimen.RetrieveBDRegimensWithDosageContainingString(pDataContext, pText));
                        break;
                    default:
                        {
                            BDConstants.BDNodeType nType = (BDConstants.BDNodeType)nodeType;
                            nodeList.AddRange(BDNode.RetrieveNodesWithNameContainingStringOfType(pDataContext, pText, nType));
                        }
                        break;
                }
            }

            return nodeList;
        }

        public static List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>> ChildTypeDefinitionListForNode(IBDNode pNode)
        {
            if (null == pNode)
                return null;

            if (pNode.LayoutVariant == BDConstants.LayoutVariantType.Undefined)
            {
                string message = string.Format("Undefined Layout Variant for node parameter in ChildTypeDefinitionListForNode method call. [{0}]", BDUtilities.GetEnumDescription(pNode.NodeType));
                throw new BDException(message, pNode);
            }

            return ChildTypeDefinitionListForNode(pNode.NodeType, pNode.LayoutVariant);
        }

        //TODO: Return 2 dimensional array: index = 0:childtype index = 1:list of possible layoutvariants
        //TODO: optional display order for childtypes
        //public static List<BDConstants.BDNodeType> ChildTypeDefinitionListForNode(BDConstants.BDNodeType pNodeType, BDConstants.LayoutVariantType pLayoutVariant)
        public static List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>> ChildTypeDefinitionListForNode(BDConstants.BDNodeType pNodeType, BDConstants.LayoutVariantType pLayoutVariant)
        {
            //Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]> result = new Tuple<BDConstants.BDNodeType,BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] {BDConstants.LayoutVariantType.TreatmentRecommendation01, BDConstants.LayoutVariantType.TreatmentRecommendation02_WoundMgmt});

            //BDConstants.BDNodeType nodeType = result.Item1;
            //BDConstants.LayoutVariantType[] variants = result.Item2;

            List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>> childDefinitionList = new List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>>();

            BDConstants.LayoutVariantType layoutVariant = pLayoutVariant;
            switch (pNodeType)
            {
                case BDConstants.BDNodeType.BDAntimicrobial:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosageGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosageGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobialRisk, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDAntimicrobialGroup:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                            // no children for this variant
                            break;
                        default:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDCategory:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            //DELTA: 1
                            //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis, BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics,
                            BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis,BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis, 
                            BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis, BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia}));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubcategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia, BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                        case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                        case BDConstants.LayoutVariantType.Prophylaxis_IE:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubcategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubcategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAttachment, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubcategory, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgery, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgeryGroup, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification }));
                            break;
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobialGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAttachment, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDChapter:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.FrontMatter:
                        case BDConstants.LayoutVariantType.BackMatter:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                        case BDConstants.LayoutVariantType.Antibiotics:
                        case BDConstants.LayoutVariantType.Dental: // Catch all for dental
                        case BDConstants.LayoutVariantType.PregancyLactation:
                        case BDConstants.LayoutVariantType.Organisms:
                        case BDConstants.LayoutVariantType.References:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSection, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.Prophylaxis_IE }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDCondition:
                    switch (layoutVariant)
                    {
                        default:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDDisease:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis, BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities, BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation01_CNS_Meningitis_Table }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities, BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;

                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir, BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Zanamivir }));
                            break;

                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDDosageGroup:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDFrequency:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDMicroorganism:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPrecaution, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDMicroorganismGroup:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                        case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganism, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDConfiguredEntry, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDOrganism:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDConfiguredEntry, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogen:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenResistance, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogenGroup:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogenResistance:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPresentation:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            //DELTA: 1
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDResponse, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDRegimenGroup:
                    switch (layoutVariant)
                    {
                        default:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDRegimen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDRegimen:
                    switch (layoutVariant)
                    {
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDResponse:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDFrequency, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSection:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                        case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                        case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Intraoperative }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Surgical_Topic }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation16_CultureDirected:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia, BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis, BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubsection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine, BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Immunization_VaccineDetail }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk, BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_I, BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_SexualAssault, BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive, BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza, BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis:
                        case BDConstants.LayoutVariantType.Prophylaxis_IE:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.PregnancyLactation_Perinatal_HIVProtocol }));
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Organisms_Antibiogram:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAttachment, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDOrganism, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubsection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.FrontMatter:
                        case BDConstants.LayoutVariantType.BackMatter:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubsection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubcategory:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I, BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II }));
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobialGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgery, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganism, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgeryClassification, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;

                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubsection:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.FrontMatter:
                        case BDConstants.LayoutVariantType.BackMatter:
                            //case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDOrganism, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubtopic:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_2_Column, BDConstants.LayoutVariantType.Table_3_Column, BDConstants.LayoutVariantType.Table_4_Column, BDConstants.LayoutVariantType.Table_5_Column }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;

                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgery:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgeryClassification, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgeryClassification, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDRegimenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDRegimenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                        default:
                            break;
                    }
                    break;

                case BDConstants.BDNodeType.BDSurgeryGroup:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgery, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDRegimenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgeryClassification, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgery, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgeryClassification:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDRegimenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTable:
                    switch (layoutVariant)
                    {

                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_NameListing_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_Stepdown_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I:
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;

                        case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCondition, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Table_2_Column:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_2_Column_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Table_3_Column:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_3_Column_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Table_4_Column:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_4_Column_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Table_5_Column:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_5_Column_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages:
                        case BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_I:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk:
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine:
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Intraoperative:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDConfiguredEntry, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCombinedEntry, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCombinedEntry, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_NoRenal }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_Renal }));
                            break;
                        default:
                            break;
                    }
                    break;
                //case BDConstants.BDNodeType.BDTableRow:
                //    childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableCell, new BDConstants.LayoutVariantType[] { layoutVariant }));
                //    break;
                case BDConstants.BDNodeType.BDTableSection:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSubsection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSubsection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_NameListing_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_Stepdown_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.Table_2_Column:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_2_Column_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.Table_3_Column:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSubsection, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_3_Column }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_3_Column_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.Table_4_Column:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_4_Column_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.Table_5_Column:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_5_Column_ContentRow }));
                            break;

                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTableSubsection:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_NameListing_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.Table_3_Column:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_3_Column_ContentRow }));
                            break;

                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTableGroup:
                    switch (layoutVariant)
                    {
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTherapyGroup:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                        case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapy, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTopic:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosageGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization:
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine:
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_2_Column, BDConstants.LayoutVariantType.Table_3_Column, BDConstants.LayoutVariantType.Table_4_Column, BDConstants.LayoutVariantType.Table_5_Column }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubtopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAttachment, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubtopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_Renal:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDConfiguredEntry, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_Renal }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCombinedEntry, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir_Creatinine, BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir_Weight }));
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Zanamivir:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCombinedEntry, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Zanamivir }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_CNS_Meningitis_Table:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDConfiguredEntry, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubtopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubtopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubtopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAttachment, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgery, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDAttachment:
                    switch (layoutVariant)
                    {
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            return childDefinitionList;
        }

        public static List<IBDNode> GetAllForNodeType(Entities pDataContext, BDConstants.BDNodeType pNodeType)
        {
            List<IBDNode> entryList = new List<IBDNode>();

            if (null == pDataContext) return entryList;

            switch (pNodeType)
            {
                case BDConstants.BDNodeType.None:
                    // do nothing
                    break;
                case BDConstants.BDNodeType.BDAttachment:
                    IQueryable<BDAttachment> taEntries = (from entry in pDataContext.BDAttachments
                                                          orderby entry.displayOrder
                                                          select entry);
                    if (taEntries.Count() > 0)
                    {
                        List<IBDNode> workingList = new List<IBDNode>(taEntries.ToList<BDAttachment>());
                        entryList.AddRange(workingList);
                    }
                    break;
                case BDConstants.BDNodeType.BDConfiguredEntry:
                    IQueryable<BDConfiguredEntry> cEntries = (from entry in pDataContext.BDConfiguredEntries
                                                              orderby entry.displayOrder
                                                              select entry);
                    if (cEntries.Count() > 0)
                    {
                        List<BDConfiguredEntry> workingList = new List<BDConfiguredEntry>(cEntries.ToList<BDConfiguredEntry>());
                        entryList.AddRange(workingList);
                    }
                    break;
                case BDConstants.BDNodeType.BDTherapy:
                    IQueryable<BDTherapy> tEntries = (from entry in pDataContext.BDTherapies
                                                      orderby entry.displayOrder
                                                      select entry);
                    if (tEntries.Count() > 0)
                    {
                        List<IBDNode> workingList = new List<IBDNode>(tEntries.ToList<BDTherapy>());
                        entryList.AddRange(workingList);
                    }
                    break;
                case BDConstants.BDNodeType.BDTherapyGroup:
                    IQueryable<BDTherapyGroup> tgEntries = (from entry in pDataContext.BDTherapyGroups
                                                            orderby entry.displayOrder
                                                            select entry);
                    if (tgEntries.Count() > 0)
                    {
                        List<IBDNode> workingList = new List<IBDNode>(tgEntries.ToList<BDTherapyGroup>());
                        entryList.AddRange(workingList);
                    }
                    break;
                case BDConstants.BDNodeType.BDRegimen:
                    IQueryable<BDRegimen> regimens = (from entry in pDataContext.BDRegimens
                                                      orderby entry.displayOrder
                                                      select entry);
                    if (regimens.Count() > 0)
                    {
                        List<IBDNode> workingList = new List<IBDNode>(regimens.ToList<BDRegimen>());
                        entryList.AddRange(workingList);
                    }
                    break;
                case BDConstants.BDNodeType.BDRegimenGroup:
                    IQueryable<BDRegimenGroup> regimenGroups = (from entry in pDataContext.BDRegimenGroups
                                                                orderby entry.displayOrder
                                                                select entry);
                    if (regimenGroups.Count() > 0)
                    {
                        List<IBDNode> workingList = new List<IBDNode>(regimenGroups.ToList<BDRegimenGroup>());
                        entryList.AddRange(workingList);
                    }
                    break;
                default:
                    IQueryable<BDNode> nodeEntries = null;
                    nodeEntries = (from entry in pDataContext.BDNodes
                                   where entry.nodeType == (int)pNodeType
                                   orderby entry.displayOrder
                                   select entry);

                    if (null != nodeEntries && nodeEntries.Count() > 0)
                    {
                        List<IBDNode> workingList = new List<IBDNode>(nodeEntries.ToList<BDNode>());
                        entryList.AddRange(workingList);
                    }
                    break;
            }

            return entryList;
        }

        public static List<IBDNode> GetChildrenForParent(Entities pContext, IBDNode pParent)
        {
            List<IBDNode> entryList = new List<IBDNode>();

            if (null != pParent)
            {
                List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>> childTypeInfoList = ChildTypeDefinitionListForNode(pParent);
                if (null != childTypeInfoList)
                {
                    foreach (Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]> childTypeInfo in childTypeInfoList)
                    {
                        entryList.AddRange(GetChildrenForParentIdAndChildType(pContext, pParent.Uuid, childTypeInfo.Item1));
                    }
                }


                bool performPostSort = true;

                switch (pParent.NodeType)
                {
                    case BDConstants.BDNodeType.BDPathogenGroup:
                        performPostSort = false;
                        break;
                    default:
                        performPostSort = true;
                        break;
                }

                if (performPostSort)
                {
                    entryList.Sort(delegate(IBDNode n1, IBDNode n2)
                    {
                        if (n1.DisplayOrder == null && n2.DisplayOrder == null) return 0;

                        else if (n1.DisplayOrder != null && n2.DisplayOrder == null) return -1;

                        else if (n1.DisplayOrder == null && n2.DisplayOrder != null) return 1;

                        else
                            return (n1.DisplayOrder as IComparable).CompareTo(n2.DisplayOrder as IComparable);
                    });
                }
            }

            return entryList;
        }

        private static List<IBDNode> GetChildrenForParentIdAndChildType(Entities pContext, Guid pParentId, BDConstants.BDNodeType pChildNodeType)
        {
            List<IBDNode> entryList = new List<IBDNode>();

            if (null != pParentId)
            {
                BDConstants.BDNodeType childNodeType = pChildNodeType;

                switch (childNodeType)
                {
                    case BDConstants.BDNodeType.BDAttachment:
                        entryList.AddRange(new List<IBDNode>(BDAttachment.RetrieveAttachmentForParentId(pContext, pParentId)));
                        break;
                    case BDConstants.BDNodeType.BDRegimenGroup:
                        entryList.AddRange(new List<IBDNode>(BDRegimenGroup.RetrieveRegimenGroupsForParentUuid(pContext, pParentId)));
                        break;
                    case BDConstants.BDNodeType.BDRegimen:
                        entryList.AddRange(new List<IBDNode>(BDRegimen.RetrieveBDRegimensForParentUuid(pContext, pParentId)));
                        break;
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        entryList.AddRange(new List<IBDNode>(BDTherapyGroup.RetrieveTherapyGroupsForParentId(pContext, pParentId)));
                        break;

                    case BDConstants.BDNodeType.BDTherapy:
                        entryList.AddRange(new List<IBDNode>(BDTherapy.RetrieveTherapiesForParentId(pContext, pParentId)));
                        break;

                    case BDConstants.BDNodeType.BDTableRow:
                        entryList.AddRange(new List<IBDNode>(BDTableRow.RetrieveTableRowsForParentId(pContext, pParentId)));
                        break;

                    case BDConstants.BDNodeType.BDTableCell:
                        entryList.AddRange(new List<IBDNode>(BDTableCell.RetrieveTableCellsForParentId(pContext, pParentId)));
                        break;

                    case BDConstants.BDNodeType.BDDosage:
                        entryList.AddRange(new List<IBDNode>(BDDosage.RetrieveDosagesForParentId(pContext, pParentId)));
                        break;

                    case BDConstants.BDNodeType.BDPrecaution:
                        entryList.AddRange(new List<IBDNode>(BDPrecaution.RetrievePrecautionsForParentId(pContext, pParentId)));
                        break;

                    case BDConstants.BDNodeType.BDAntimicrobialRisk:
                        entryList.AddRange(new List<IBDNode>(BDAntimicrobialRisk.RetrieveAntimicrobialRiskForParentId(pContext, pParentId)));
                        break;
                    case BDConstants.BDNodeType.BDConfiguredEntry:
                        entryList.AddRange(new List<IBDNode>(BDConfiguredEntry.RetrieveListForParentId(pContext, pParentId)));
                        break;
                    case BDConstants.BDNodeType.BDCombinedEntry:
                        entryList.AddRange(new List<IBDNode>(BDCombinedEntry.RetrieveListForParentId(pContext, pParentId)));
                        break;
                    default:
                        List<IBDNode> workingList = new List<IBDNode>(BDNode.RetrieveNodesForParentIdAndChildNodeType(pContext, pParentId, pChildNodeType));
                        entryList.AddRange(workingList);
                        break;
                }
            }
            return entryList;
        }

        public static List<IBDNode> RepairSiblingNodeDisplayOrder(Entities pContext, IBDNode pParentNode, BDConstants.BDNodeType pNodeType)
        {
            List<IBDNode> siblingList = GetChildrenForParent(pContext, pParentNode);
            for (int idx = 0; idx < siblingList.Count; idx++)
            {
                if (siblingList[idx].DisplayOrder != idx)
                {
                    siblingList[idx].DisplayOrder = idx;
                    SaveNode(pContext, siblingList[idx]);
                }
            }

            return siblingList;
        }

        public static void ReorderNode(Entities pContext, IBDNode pNode, int pOffset)
        {
            if (!((null != pNode) && (null != pNode.ParentId) && (null != pNode.DisplayOrder))) return;

            IBDNode parentNode = RetrieveNode(pContext, pNode.ParentType, pNode.ParentId);
            List<IBDNode> siblingList = RepairSiblingNodeDisplayOrder(pContext, parentNode, pNode.NodeType);

            int currentIndex = siblingList.FindIndex(t => t == pNode);
            if (currentIndex != pNode.DisplayOrder)
            {
                throw new BDException("Attempt to reorder a node without a valid display order", pNode);
            }

            if (currentIndex >= 0)
            {
                int requestedIndex = currentIndex + pOffset;
                if ((requestedIndex >= 0) && (requestedIndex < siblingList.Count))
                {
                    siblingList[requestedIndex].DisplayOrder = currentIndex;
                    SaveNode(pContext, siblingList[requestedIndex]);

                    siblingList[currentIndex].DisplayOrder = requestedIndex;
                    SaveNode(pContext, siblingList[currentIndex]);
                }
            }
        }

        //TODO: Pass child layoutvariant type. Multiple types possible.
        public static IBDNode CreateChildNode(Entities pContext, IBDNode pParentNode, BDConstants.BDNodeType pChildType, BDConstants.LayoutVariantType pLayoutVariant)
        {
            IBDNode result = null;

            // get the existing siblings to the new node
            //Ensure that the display orders for the sibling list is sequential so the new node is 
            List<IBDNode> siblingList = RepairSiblingNodeDisplayOrder(pContext, pParentNode, pChildType);

            switch (pChildType)
            {
                case BDConstants.BDNodeType.BDAntimicrobialRisk:
                    BDAntimicrobialRisk risk = BDAntimicrobialRisk.CreateBDAntimicrobialRisk(pContext);
                    risk.DisplayOrder = siblingList.Count;
                    risk.SetParent(pParentNode);
                    risk.LayoutVariant = pLayoutVariant;
                    BDAntimicrobialRisk.Save(pContext, risk);
                    result = risk;
                    break;
                case BDConstants.BDNodeType.BDAttachment:
                    BDAttachment attachment = BDAttachment.CreateBDAttachment(pContext);
                    attachment.DisplayOrder = siblingList.Count;
                    attachment.SetParent(pParentNode);
                    attachment.LayoutVariant = pLayoutVariant;
                    BDAttachment.Save(pContext, attachment);
                    result = attachment;
                    break;
                case BDConstants.BDNodeType.BDConfiguredEntry:
                    string configuredEntryName = string.Format(@"Entry-{0}", siblingList.Count);
                    BDConfiguredEntry configuredEntry = BDConfiguredEntry.Create(pContext, pLayoutVariant, pParentNode.Uuid, pParentNode.NodeType, configuredEntryName);
                    configuredEntry.DisplayOrder = siblingList.Count;
                    BDConfiguredEntry.Save(pContext, configuredEntry);
                    result = configuredEntry;
                    break;
                case BDConstants.BDNodeType.BDCombinedEntry:
                    string combinedEntryName = string.Format(@"Entry-{0}", siblingList.Count);
                    BDCombinedEntry combinedEntry = BDCombinedEntry.Create(pContext, pLayoutVariant, pParentNode.Uuid, pParentNode.NodeType, combinedEntryName);
                    BDCombinedEntry.Save(pContext, combinedEntry);
                    result = combinedEntry;
                    break;
                case BDConstants.BDNodeType.BDDosage:
                    BDDosage dosage = BDDosage.CreateBDDosage(pContext, pParentNode.Uuid);
                    dosage.DisplayOrder = siblingList.Count;
                    dosage.SetParent(pParentNode);
                    dosage.LayoutVariant = pLayoutVariant;
                    BDDosage.Save(pContext, dosage);
                    result = dosage;
                    break;
                case BDConstants.BDNodeType.BDPrecaution:
                    BDPrecaution precaution = BDPrecaution.CreateBDPrecaution(pContext, pParentNode.Uuid);
                    precaution.DisplayOrder = siblingList.Count;
                    precaution.SetParent(pParentNode);
                    precaution.LayoutVariant = pLayoutVariant;
                    BDPrecaution.Save(pContext, precaution);
                    result = precaution;
                    break;
                case BDConstants.BDNodeType.BDRegimenGroup:
                    BDRegimenGroup regimenGroup = BDRegimenGroup.CreateBDRegimenGroup(pContext, pParentNode.Uuid);
                    regimenGroup.DisplayOrder = siblingList.Count;
                    regimenGroup.SetParent(pParentNode);
                    regimenGroup.LayoutVariant = pLayoutVariant;
                    BDRegimenGroup.Save(pContext, regimenGroup);
                    result = regimenGroup;
                    break;
                case BDConstants.BDNodeType.BDRegimen:
                    BDRegimen regimen = BDRegimen.CreateBDRegimen(pContext, pParentNode.Uuid);
                    regimen.DisplayOrder = siblingList.Count;
                    regimen.SetParent(pParentNode);
                    regimen.LayoutVariant = pLayoutVariant;
                    BDRegimen.Save(pContext, regimen);
                    result = regimen;
                    break;
                case BDConstants.BDNodeType.BDTherapyGroup:
                    BDTherapyGroup therapyGroup = BDTherapyGroup.CreateBDTherapyGroup(pContext, pParentNode.Uuid);
                    therapyGroup.displayOrder = siblingList.Count;
                    therapyGroup.SetParent(pParentNode);
                    therapyGroup.LayoutVariant = pLayoutVariant;
                    therapyGroup.Name = String.Format(@"New Therapy Group {0}", therapyGroup.displayOrder + 1);
                    BDTherapyGroup.Save(pContext, therapyGroup);
                    result = therapyGroup;
                    break;

                case BDConstants.BDNodeType.BDTherapy:
                    BDTherapy therapy = BDTherapy.CreateBDTherapy(pContext, pParentNode.Uuid);
                    therapy.displayOrder = siblingList.Count;
                    therapy.SetParent(pParentNode);
                    therapy.LayoutVariant = pLayoutVariant;
                    therapy.Name = String.Format(@"New Therapy {0}", therapy.displayOrder + 1);
                    BDTherapy.Save(pContext, therapy);
                    result = therapy;
                    break;

                case BDConstants.BDNodeType.BDTableRow:
                    BDTableRow tableRow = BDTableRow.CreateBDTableRow(pContext, pChildType);
                    tableRow.displayOrder = siblingList.Count;
                    tableRow.SetParent(pParentNode);
                    tableRow.LayoutVariant = pLayoutVariant;
                    tableRow.Name = String.Format(@"New {0}-{1}", BDUtilities.GetEnumDescription(pChildType), tableRow.displayOrder + 1);
                    tableRow.rowType = -1;
                    BDTableRow.Save(pContext, tableRow);
                    result = tableRow;

                    // add BDTableCells for the tableRows
                    int cellCount = GetTableColumnCount(pLayoutVariant);
                    for (int i = 0; i < cellCount; i++)
                    {
                        BDTableCell tableCell = BDTableCell.CreateBDTableCell(pContext);
                        tableCell.displayOrder = i + 1;
                        tableCell.SetParent(tableRow);
                        tableCell.alignment = 0;
                        BDTableCell.Save(pContext, tableCell);
                    }

                    break;

                case BDConstants.BDNodeType.BDTable:
                default:
                    BDNode node = BDNode.CreateBDNode(pContext, pChildType);
                    node.displayOrder = siblingList.Count;
                    node.SetParent(pParentNode);
                    node.LayoutVariant = pLayoutVariant;
                    node.Name = String.Format(@"New {0}-{1}", BDUtilities.GetEnumDescription(pChildType), node.displayOrder + 1);
                    BDNode.Save(pContext, node);
                    result = node;
                    break;
            }
            return result;
        }

        public static void DeleteNode(Entities pContext, IBDNode pNode)
        {
            DeleteNode(pContext, pNode, true);
        }

        public static void DeleteNode(Entities pContext, IBDNode pNode, bool pCreateDeletionRecord)
        {
            IBDNode parentNode = RetrieveNode(pContext, pNode.ParentType, pNode.ParentId);
            BDConstants.BDNodeType nodeType = pNode.NodeType;

            switch (nodeType)
            {
                case BDConstants.BDNodeType.BDAntimicrobialRisk:
                    BDAntimicrobialRisk risk = pNode as BDAntimicrobialRisk;
                    BDAntimicrobialRisk.Delete(pContext, risk, pCreateDeletionRecord);
                    break;
                case BDConstants.BDNodeType.BDAttachment:
                    BDAttachment attachment = pNode as BDAttachment;
                    BDAttachment.Delete(pContext, attachment, pCreateDeletionRecord);
                    break;
                case BDConstants.BDNodeType.BDDosage:
                    BDDosage dosage = pNode as BDDosage;
                    BDDosage.Delete(pContext, dosage, pCreateDeletionRecord);
                    break;
                case BDConstants.BDNodeType.BDRegimenGroup:
                    BDRegimenGroup regimenGroup = pNode as BDRegimenGroup;
                    BDRegimenGroup.Delete(pContext, regimenGroup, pCreateDeletionRecord);
                    break;
                case BDConstants.BDNodeType.BDRegimen:
                    BDRegimen regimen = pNode as BDRegimen;
                    BDRegimen.Delete(pContext, regimen, pCreateDeletionRecord);
                    break;
                case BDConstants.BDNodeType.BDTherapyGroup:
                    BDTherapyGroup therapyGroup = pNode as BDTherapyGroup;
                    BDTherapyGroup.Delete(pContext, therapyGroup, pCreateDeletionRecord);
                    break;
                case BDConstants.BDNodeType.BDPrecaution:
                    BDPrecaution precaution = pNode as BDPrecaution;
                    BDPrecaution.Delete(pContext, precaution, pCreateDeletionRecord);
                    break;
                case BDConstants.BDNodeType.BDTherapy:

                    BDTherapy therapy = pNode as BDTherapy;
                    BDTherapy.Delete(pContext, therapy, pCreateDeletionRecord);
                    break;

                case BDConstants.BDNodeType.BDTableRow:
                    BDTableRow row = pNode as BDTableRow;
                    List<BDTableCell> cells = BDTableCell.RetrieveTableCellsForParentId(pContext, pNode.Uuid);
                    foreach (BDTableCell cell in cells)
                        BDTableCell.Delete(pContext, cell, pCreateDeletionRecord);
                    BDTableRow.Delete(pContext, row, pCreateDeletionRecord);
                    break;

                default:
                    BDNode node = pNode as BDNode;
                    BDNode.Delete(pContext, node, pCreateDeletionRecord);
                    break;
            }

            List<IBDNode> siblingList = RepairSiblingNodeDisplayOrder(pContext, parentNode, nodeType);
        }

        public static IBDNode RetrieveNode(Entities pContext, BDConstants.BDNodeType pNodeType, Guid? pUuid)
        {
            IBDNode result = null;

            if (null != pUuid)
            {
                switch (pNodeType)
                {
                    case BDConstants.BDNodeType.BDAntimicrobialRisk:
                        result = BDAntimicrobialRisk.RetrieveAntimicrobialRiskWithId(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDAttachment:
                        result = BDAttachment.RetrieveWithId(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDDosage:
                        result = BDDosage.RetrieveDosageWithId(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDPrecaution:
                        result = BDPrecaution.RetrievePrecautionWithId(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDRegimen:
                        result = BDRegimen.RetrieveBDRegimenWithUuid(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDRegimenGroup:
                        result = BDRegimenGroup.RetrieveRegimenGroupWithUuid(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        result = BDTherapyGroup.RetrieveTherapyGroupWithId(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDTherapy:
                        result = BDTherapy.RetrieveTherapyWithId(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDTableRow:
                        result = BDTableRow.RetrieveTableRowWithId(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDConfiguredEntry:
                        result = BDConfiguredEntry.RetrieveConfiguredEntryWithId(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDCombinedEntry:
                        result = BDCombinedEntry.RetrieveCombinedEntryWithId(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDTableCell:
                        result = BDTableCell.RetrieveWithId(pContext, pUuid.Value);
                        break;
                    case BDConstants.BDNodeType.BDLinkedNote:
                        // not a BDNode
                        break;
                    default:
                        result = BDNode.RetrieveNodeWithId(pContext, pUuid.Value);
                        break;
                }
            }
            return result;
        }

        public static IBDNode RetrieveNode(Entities pContext, Guid? pUuid)
        {
            IBDNode result = null;
            result = RetrieveNode(pContext, BDConstants.BDNodeType.BDChapter, pUuid);
            if (null != result) return result;
            else
            {
                result = RetrieveNode(pContext, BDConstants.BDNodeType.BDTherapy, pUuid);
                if (result != null) return result;
                else
                {
                    result = RetrieveNode(pContext, BDConstants.BDNodeType.BDTherapyGroup, pUuid);
                    if (result != null) return result;
                    else
                    {
                        result = RetrieveNode(pContext, BDConstants.BDNodeType.BDRegimen, pUuid);
                        if (result != null) return result;
                        else
                        {
                            result = RetrieveNode(pContext, BDConstants.BDNodeType.BDRegimenGroup, pUuid);
                            if (result != null) return result;
                            else
                            {
                                result = RetrieveNode(pContext, BDConstants.BDNodeType.BDLinkedNote, pUuid);
                                if (result != null) return result;
                                else
                                {
                                    result = RetrieveNode(pContext, BDConstants.BDNodeType.BDAntimicrobialRisk, pUuid);
                                    if (result != null) return result;
                                    else
                                    {
                                        result = RetrieveNode(pContext, BDConstants.BDNodeType.BDDosage, pUuid);
                                        if (result != null) return result;
                                        else
                                        {
                                            result = RetrieveNode(pContext, BDConstants.BDNodeType.BDDosageGroup, pUuid);
                                            if (result != null) return result;
                                            else
                                            {
                                                result = RetrieveNode(pContext, BDConstants.BDNodeType.BDTableRow, pUuid);
                                                if (result != null) return result;
                                                else
                                                {
                                                    result = RetrieveNode(pContext, BDConstants.BDNodeType.BDTableCell, pUuid);
                                                    if (result != null) return result;
                                                    else
                                                    {
                                                        result = RetrieveNode(pContext, BDConstants.BDNodeType.BDPrecaution, pUuid);
                                                        if (result != null) return result;
                                                        else
                                                        {
                                                            result = RetrieveNode(pContext, BDConstants.BDNodeType.BDConfiguredEntry, pUuid);
                                                            if (result != null) return result;
                                                            else
                                                            {
                                                                result = RetrieveNode(pContext, BDConstants.BDNodeType.BDCombinedEntry, pUuid);
                                                                if (result != null) return result;
                                                                else
                                                                {
                                                                    result = RetrieveNode(pContext, BDConstants.BDNodeType.BDAttachment, pUuid);
                                                                    return result;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static IBDNode CreateNode(Entities pContext, BDConstants.BDNodeType pNodeType, Guid? pParentUuid, BDConstants.BDNodeType pParentNodeType)
        {
            IBDNode entry = null;
            switch (pNodeType)
            {
                case BDConstants.BDNodeType.None:
                    break;
                case BDConstants.BDNodeType.BDConfiguredEntry:
                    entry = BDConfiguredEntry.Create(pContext, BDConstants.LayoutVariantType.Undefined, pParentUuid.Value, pParentNodeType, null);
                    break;
                case BDConstants.BDNodeType.BDTherapy:
                    entry = BDTherapy.CreateBDTherapy(pContext, pParentUuid.Value);
                    break;
                case BDConstants.BDNodeType.BDTherapyGroup:
                    entry = BDTherapyGroup.CreateBDTherapyGroup(pContext, pParentUuid.Value);
                    break;
                case BDConstants.BDNodeType.BDTableRow:
                    entry = BDTableRow.CreateBDTableRow(pContext, pParentNodeType, pParentUuid.Value);
                    break;
                case BDConstants.BDNodeType.BDAttachment:
                    entry = BDAttachment.CreateBDAttachment(pContext);
                    break;
                case BDConstants.BDNodeType.BDAntimicrobialRisk:
                    entry = BDAntimicrobialRisk.CreateBDAntimicrobialRisk(pContext);
                    break;
                case BDConstants.BDNodeType.BDDosage:
                    entry = BDDosage.CreateBDDosage(pContext, pParentUuid.Value);
                    break;
                case BDConstants.BDNodeType.BDPrecaution:
                    entry = BDPrecaution.CreateBDPrecaution(pContext, pParentUuid.Value);
                    break;
                case BDConstants.BDNodeType.BDRegimenGroup:
                    entry = BDRegimenGroup.CreateBDRegimenGroup(pContext, pParentUuid.Value);
                    break;
                case BDConstants.BDNodeType.BDRegimen:
                    entry = BDRegimen.CreateBDRegimen(pContext, pParentUuid.Value);
                    break;
                default:
                    entry = BDNode.CreateBDNode(pContext, pNodeType);
                    break;
            }

            if (null != entry)
            {
                entry.SetParent(pParentNodeType, pParentUuid);
            }

            return entry;
        }

        public static IBDControl CreateControlForNode(Entities pContext, IBDNode pNode)
        {
            IBDControl nodeControl = null;
            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDAntimicrobial:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            {
                                nodeControl = new BDNodeOverviewControl(pContext, pNode);
                                BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                                newOverviewControl.ShowAsChild = true;
                                newOverviewControl.ShowSiblingAdd = true;
                            }
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.AssignTypeaheadSource(BDTypeahead.Antimicrobials, BDNode.PROPERTYNAME_NAME);
                                newControl.ShowAsChild = false;
                                newControl.ShowSiblingAdd = false;
                            }
                            break;
                        default:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.AssignTypeaheadSource(BDTypeahead.Antimicrobials, BDNode.PROPERTYNAME_NAME);
                                newControl.ShowAsChild = true;
                                newControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDAntimicrobialRisk:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            {
                                nodeControl = new BDAntimicrobialRiskControl(pContext, pNode);
                                nodeControl.ShowAsChild = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDAntimicrobialGroup:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.ShowAsChild = false;
                                newControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDCategory:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_IE:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            break;

                        default:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDChapter:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDCondition:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDCombinedEntry:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            BDCombinedEntry combinedEntry = pNode as BDCombinedEntry;
                            nodeControl = new BDCombinedEntryControl(pContext, combinedEntry, null);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDConfiguredEntry:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_I:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II:
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                            BDConfiguredEntry configuredWithOverview = pNode as BDConfiguredEntry;
                            nodeControl = new BDConfiguredEntryControl(pContext, configuredWithOverview, null);
                            ((BDConfiguredEntryControl)nodeControl).showOverview = true;
                            break;
                        default:
                            BDConfiguredEntry configuredEntry = pNode as BDConfiguredEntry;
                            nodeControl = new BDConfiguredEntryControl(pContext, configuredEntry, null);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDDisease:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl newControl = nodeControl as BDNodeControl;
                            newControl.ShowAsChild = false;
                            newControl.ShowSiblingAdd = false;
                            break;
                        default:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDDosage:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            nodeControl = new BDDosageControl(pContext, pNode);
                            break;
                        default:
                            {
                                nodeControl = new BDDosageAndCostControl(pContext, pNode);
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDDosageGroup:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.ShowAsChild = true;
                                newControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDFrequency:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl newControl = nodeControl as BDNodeControl;
                            newControl.ShowAsChild = true;
                            newControl.ShowSiblingAdd = true;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDMicroorganism:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.ShowAsChild = false;
                                newControl.ShowSiblingAdd = false;
                            }
                            break;
                        default:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.ShowAsChild = true;
                                newControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDMicroorganismGroup:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.ShowAsChild = false;
                                newControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDOrganism:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            {
                                nodeControl = new BDNodeOverviewControl(pContext, pNode);
                                BDNodeOverviewControl newControl = nodeControl as BDNodeOverviewControl;
                                newControl.ShowAsChild = false;
                                newControl.ShowSiblingAdd = false;
                            }
                            break;
                        default:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.ShowAsChild = false;
                                newControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogen:
                    switch (pNode.LayoutVariant)
                    {

                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                            newOverviewControl.ShowAsChild = true;
                            newOverviewControl.ShowSiblingAdd = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl control = nodeControl as BDNodeControl;
                            control.AssignTypeaheadSource(BDTypeahead.Pathogens, BDNode.PROPERTYNAME_NAME);
                            control.ShowAsChild = false;
                            control.ShowSiblingAdd = false;
                            break;
                        default:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.AssignTypeaheadSource(BDTypeahead.Pathogens, BDNode.PROPERTYNAME_NAME);
                                newControl.ShowAsChild = true;
                                newControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogenGroup:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.AssignTypeaheadSource(BDTypeahead.PathogenGroups, BDNode.PROPERTYNAME_NAME);
                                newControl.ShowAsChild = true;
                                newControl.ShowSiblingAdd = true;
                            }
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                            {
                                nodeControl = new BDNodeOverviewControl(pContext, pNode);
                                BDNodeOverviewControl newControl = nodeControl as BDNodeOverviewControl;
                                newControl.AssignTypeaheadSource(BDTypeahead.PathogenGroups, BDNode.PROPERTYNAME_NAME);
                                newControl.ShowAsChild = true;
                                newControl.ShowSiblingAdd = true;
                            }
                            break;
                        default:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.AssignTypeaheadSource(BDTypeahead.PathogenGroups, BDNode.PROPERTYNAME_NAME);
                                newControl.ShowAsChild = false;
                                newControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogenResistance:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl newControl = nodeControl as BDNodeControl;
                            newControl.ShowAsChild = true;
                            newControl.ShowSiblingAdd = true;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPresentation:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl newControl = nodeControl as BDNodeOverviewControl;
                            newControl.ShowAsChild = true;
                            newControl.ShowSiblingAdd = true;
                            break;
                        default:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPrecaution:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDPrecautionControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDRegimen:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            {
                                nodeControl = new BDRegimenControl();
                                BDRegimenControl regimenControl = nodeControl as BDRegimenControl;
                                regimenControl.AssignTypeaheadSource(BDTypeahead.TherapyNames, BDRegimen.PROPERTYNAME_NAME);
                                regimenControl.AssignTypeaheadSource(BDTypeahead.TherapyDosages, BDRegimen.PROPERTYNAME_DOSAGE);
                                regimenControl.AssignTypeaheadSource(BDTypeahead.TherapyDurations, BDRegimen.PROPERTYNAME_DURATION);
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDRegimenGroup:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            {
                                nodeControl = new BDRegimenGroupControl();
                                BDRegimenGroupControl newControl = nodeControl as BDRegimenGroupControl;
                                newControl.AssignTypeaheadSource(BDTypeahead.TherapyGroups);
                                newControl.CurrentRegimenGroup = pNode as BDRegimenGroup;
                                newControl.AssignDataContext(pContext);
                                newControl.DefaultLayoutVariantType = pNode.LayoutVariant;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDResponse:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl newControl = nodeControl as BDNodeOverviewControl;
                            newControl.ShowAsChild = true;
                            newControl.ShowSiblingAdd = true;
                            break;
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSection:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Organisms_Antibiogram:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            break;
                        default:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubcategory:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                        case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl newNodeControl = nodeControl as BDNodeControl;
                            newNodeControl.ShowAsChild = false;
                            newNodeControl.ShowSiblingAdd = false;
                            break;
                        default:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubsection:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                        case BDConstants.LayoutVariantType.FrontMatter:
                        case BDConstants.LayoutVariantType.BackMatter:
                            {
                                nodeControl = new BDNodeOverviewControl(pContext, pNode);
                                BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                                newOverviewControl.ShowAsChild = false;
                                newOverviewControl.ShowSiblingAdd = false;
                            }
                            break;
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubtopic:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl overviewControl = nodeControl as BDNodeOverviewControl;
                            overviewControl.ShowAsChild = false;
                            overviewControl.ShowSiblingAdd = false;
                            break;
                        default:
                            {
                                nodeControl = new BDNodeOverviewControl(pContext, pNode);
                                BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                                newOverviewControl.ShowAsChild = true;
                                newOverviewControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgeryGroup:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newNodeControl = nodeControl as BDNodeControl;
                                newNodeControl.ShowAsChild = false;
                                newNodeControl.ShowSiblingAdd = false;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgery:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                            {
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newNodeControl = nodeControl as BDNodeControl;
                                newNodeControl.ShowAsChild = true;
                                newNodeControl.ShowSiblingAdd = true;
                            }
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl ovControl = nodeControl as BDNodeOverviewControl;
                            ovControl.ShowAsChild = false;
                            ovControl.ShowSiblingAdd = false;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl newControl = nodeControl as BDNodeControl;
                            newControl.ShowAsChild = false;
                            newControl.ShowSiblingAdd = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgeryClassification:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl newControl = nodeControl as BDNodeOverviewControl;
                            newControl.ShowAsChild = false;
                            newControl.ShowSiblingAdd = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTable:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_I:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                            newOverviewControl.ShowAsChild = true;
                            newOverviewControl.ShowSiblingAdd = false;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl dmControl = nodeControl as BDNodeControl;
                            dmControl.ShowAsChild = true;
                            dmControl.ShowSiblingAdd = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Intraoperative:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl pxControl = nodeControl as BDNodeOverviewControl;
                            pxControl.ShowAsChild = false;
                            pxControl.ShowSiblingAdd = false;
                            break;
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl newControl = nodeControl as BDNodeControl;
                            newControl.ShowAsChild = false;
                            newControl.ShowSiblingAdd = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTableCell:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDTableCellControl();
                            BDTableCellControl cellControl = nodeControl as BDTableCellControl;

                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTableRow:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDTableRowControl();
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTableSection:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                            newOverviewControl.ShowAsChild = true;
                            newOverviewControl.ShowSiblingAdd = true;
                            break;
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl newControl = nodeControl as BDNodeControl;
                            newControl.ShowAsChild = true;
                            newControl.ShowSiblingAdd = true;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTableGroup:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl newControl = nodeControl as BDNodeControl;
                            newControl.ShowAsChild = true;
                            newControl.ShowSiblingAdd = true;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTableSubsection:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl newControl = nodeControl as BDNodeControl;
                            newControl.ShowAsChild = true;
                            newControl.ShowSiblingAdd = true;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTherapy:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            {
                                nodeControl = new BDTherapyControl();
                                BDTherapyControl therapyControl = nodeControl as BDTherapyControl;
                                therapyControl.AssignTypeaheadSource(BDTypeahead.TherapyNames, BDTherapy.PROPERTYNAME_THERAPY);
                                therapyControl.AssignTypeaheadSource(BDTypeahead.TherapyDosages, BDTherapy.PROPERTYNAME_DOSAGE);
                                therapyControl.AssignTypeaheadSource(BDTypeahead.TherapyDurations, BDTherapy.PROPERTYNAME_DURATION);
                                therapyControl.AssignTypeaheadSource(BDTypeahead.TherapyDosages, BDTherapy.PROPERTYNAME_DOSAGE_1);
                                therapyControl.AssignTypeaheadSource(BDTypeahead.TherapyDosages, BDTherapy.PROPERTYNAME_DOSAGE_2);
                                therapyControl.AssignTypeaheadSource(BDTypeahead.TherapyDurations, BDTherapy.PROPERTYNAME_DURATION_1);
                                therapyControl.AssignTypeaheadSource(BDTypeahead.TherapyDurations, BDTherapy.PROPERTYNAME_DURATION_2);

                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTherapyGroup:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            {
                                nodeControl = new BDTherapyGroupControl();
                                BDTherapyGroupControl newControl = nodeControl as BDTherapyGroupControl;
                                newControl.AssignTypeaheadSource(BDTypeahead.TherapyGroups);
                                newControl.CurrentTherapyGroup = pNode as BDTherapyGroup;
                                newControl.AssignDataContext(pContext);
                                newControl.DefaultLayoutVariantType = pNode.LayoutVariant;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTopic:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_VaccineDetail:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl dmOverview = nodeControl as BDNodeOverviewControl;
                            dmOverview.ShowAsChild = false;
                            dmOverview.ShowSiblingAdd = false;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl tmpControl = nodeControl as BDNodeControl;
                            tmpControl.ShowAsChild = false;
                            tmpControl.ShowSiblingAdd = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_Renal:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl tControl = nodeControl as BDNodeControl;
                            tControl.ShowAsChild = false;
                            tControl.ShowSiblingAdd = false;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl txControl = nodeControl as BDNodeControl;
                            txControl.ShowAsChild = true;
                            txControl.ShowSiblingAdd = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Topic:
                        default:
                            {
                                nodeControl = new BDNodeOverviewControl(pContext, pNode);
                                BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                                newOverviewControl.ShowAsChild = true;
                                newOverviewControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDAttachment:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDAttachmentControl(pContext, pNode);
                            BDAttachmentControl attachmentControl = nodeControl as BDAttachmentControl;
                            attachmentControl.ShowAsChild = true;
                            attachmentControl.ShowSiblingAdd = false;
                            break;
                    }
                    break;
                default:
                    // Require explicit handling for given child types
                    // i.e. disease does not currently display children within this control
                    // Don't load children if not explicitly supported here
                    break;
            }
            return nodeControl;

        }

        public static bool RowIsHeaderRow(IBDNode pNode)
        {
            // functions correctly for a BDTableRow or BDTableCell.
            bool isHeader = false;
            if (pNode != null)
            {
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Antibiotics_NameListing_HeaderRow:
                    case BDConstants.LayoutVariantType.Antibiotics_Stepdown_HeaderRow:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional_HeaderRow:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin_HeaderRow:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_2_Column_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_3_Column_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_4_Column_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_5_Column_HeaderRow:
                        isHeader = true;
                        break;
                    default:
                        isHeader = false;
                        break;
                }
            }

            return isHeader;
        }

        public static int GetTableColumnCount(BDConstants.LayoutVariantType pLayoutVariant)
        {
            int maxColumns = 0;
            switch (pLayoutVariant)
            {
                case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                case BDConstants.LayoutVariantType.Antibiotics_NameListing_ContentRow:
                case BDConstants.LayoutVariantType.Antibiotics_NameListing_HeaderRow:
                case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin_ContentRow:
                case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin_HeaderRow:
                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I:
                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I_ContentRow:
                case BDConstants.LayoutVariantType.Table_2_Column:
                case BDConstants.LayoutVariantType.Table_2_Column_HeaderRow:
                case BDConstants.LayoutVariantType.Table_2_Column_ContentRow:
                case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                    maxColumns = 2;
                    break;
                case BDConstants.LayoutVariantType.Table_3_Column:
                case BDConstants.LayoutVariantType.Table_3_Column_HeaderRow:
                case BDConstants.LayoutVariantType.Table_3_Column_ContentRow:
                case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II:
                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_HeaderRow:
                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_ContentRow:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity:
                case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity_ContentRow:
                case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional_ContentRow:
                case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional_HeaderRow:
                    maxColumns = 3;
                    break;

                case BDConstants.LayoutVariantType.Table_4_Column:
                case BDConstants.LayoutVariantType.Table_4_Column_HeaderRow:
                case BDConstants.LayoutVariantType.Table_4_Column_ContentRow:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep:
                case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                    maxColumns = 4;
                    break;
                case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                case BDConstants.LayoutVariantType.Antibiotics_Stepdown_ContentRow:
                case BDConstants.LayoutVariantType.Antibiotics_Stepdown_HeaderRow:
                case BDConstants.LayoutVariantType.Table_5_Column:
                case BDConstants.LayoutVariantType.Table_5_Column_HeaderRow:
                case BDConstants.LayoutVariantType.Table_5_Column_ContentRow:
                case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                    maxColumns = 5;
                    break;
                default:
                    maxColumns = 4;
                    break;
            }
            return maxColumns;
        }

        static public List<String> GetPropertyNamesForNodeType(BDConstants.BDNodeType pNodeType)
        {
            List<String> propertyList = new List<string>();
            switch (pNodeType)
            {
                case BDConstants.BDNodeType.BDAntimicrobialRisk:
                    propertyList.Add(BDAntimicrobialRisk.PROPERTYNAME_APPRATING);
                    propertyList.Add(BDAntimicrobialRisk.PROPERTYNAME_LACTATIONRISK);
                    propertyList.Add(BDAntimicrobialRisk.PROPERTYNAME_NAME);
                    propertyList.Add(BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK);
                    propertyList.Add(BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION);
                    propertyList.Add(BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE);
                    break;
                case BDConstants.BDNodeType.BDAttachment:
                    propertyList.Add(BDAttachment.PROPERTYNAME_DATA);
                    break;
                case BDConstants.BDNodeType.BDConfiguredEntry:
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_NAME);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD01);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD02);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD03);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD04);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD05);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD06);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD07);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD08);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD09);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD10);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD11);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD12);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD13);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD14);
                    propertyList.Add(BDConfiguredEntry.PROPERTYNAME_FIELD15);
                    break;
                case BDConstants.BDNodeType.BDCombinedEntry:
                    propertyList.Add(BDCombinedEntry.PROPERTYNAME_NAME);
                    propertyList.Add(BDCombinedEntry.PROPERTYNAME_GROUPTITLE);
                    propertyList.Add(BDCombinedEntry.VIRTUALPROPERTYNAME_ENTRYTITLE);
                    propertyList.Add(BDCombinedEntry.VIRTUALPROPERTYNAME_ENTRYDETAIL);
                    propertyList.Add(BDCombinedEntry.VIRTUALCOLUMNNAME_01);
                    propertyList.Add(BDCombinedEntry.VIRTUALCOLUMNNAME_02);
                    break;
                case BDConstants.BDNodeType.BDDosage:
                    propertyList.Add(BDDosage.PROPERTYNAME_COST);
                    propertyList.Add(BDDosage.PROPERTYNAME_DOSAGE);
                    propertyList.Add(BDDosage.PROPERTYNAME_DOSAGE2);
                    propertyList.Add(BDDosage.PROPERTYNAME_DOSAGE3);
                    propertyList.Add(BDDosage.PROPERTYNAME_DOSAGE4);
                    propertyList.Add(BDDosage.PROPERTYNAME_NAME);
                    break;
                case BDConstants.BDNodeType.BDPrecaution:
                    propertyList.Add(BDPrecaution.PROPERTYNAME_DURATION);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_GLOVESACUTE);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_GLOVESLONGTERM);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_GOWNSACUTE);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_GOWNSLONGTERM);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_INFECTIVEMATERIAL);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_MASKACUTE);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_MASKLONGTERM);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_MODEOFTRANSMISSION);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_ORGANISM_1);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_ORGANISM_2);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_SINGLEROOMACUTE);
                    propertyList.Add(BDPrecaution.PROPERTYNAME_SINGLEROOMLONGTERM);
                    break;
                case BDConstants.BDNodeType.BDTableCell:
                    propertyList.Add(BDTableCell.PROPERTYNAME_CONTENTS);
                    break;
                case BDConstants.BDNodeType.BDTableRow:
                    propertyList.Add(BDTableRow.PROPERTYNAME_CONTENTS);
                    break;
                case BDConstants.BDNodeType.BDTherapy:
                    propertyList.Add(BDTherapy.PROPERTYNAME_DOSAGE);
                    propertyList.Add(BDTherapy.PROPERTYNAME_DOSAGE_1);
                    propertyList.Add(BDTherapy.PROPERTYNAME_DOSAGE_2);
                    propertyList.Add(BDTherapy.PROPERTYNAME_DURATION);
                    propertyList.Add(BDTherapy.PROPERTYNAME_DURATION_1);
                    propertyList.Add(BDTherapy.PROPERTYNAME_DURATION_2);
                    propertyList.Add(BDTherapy.PROPERTYNAME_THERAPY);
                    break;
                case BDConstants.BDNodeType.BDTherapyGroup:
                    propertyList.Add(BDTherapyGroup.PROPERTYNAME_NAME);
                    break;
                default:
                    propertyList.Add(BDNode.PROPERTYNAME_NAME);
                    break;
            }

            return propertyList;
        }

        public static bool TherapyLayoutHasFirstDosage(BDConstants.LayoutVariantType pLayoutVariant)
        {
            bool returnValue = true;
            switch (pLayoutVariant)
            {
                case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                    returnValue = false;
                    break;
                default:
                    break;
            }

            return returnValue;
        }

        public static bool TherapyLayoutHasFirstDuration(BDConstants.LayoutVariantType pLayoutVariant)
        {
            bool returnValue = true;

            switch (pLayoutVariant)
            {
                // layouts with no duration
                case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
                case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }
        public static bool TherapyLayoutHasSecondDosage(BDConstants.LayoutVariantType pLayoutVariant)
        {
            bool returnValue = true;

            switch (pLayoutVariant)
            {
                // layouts without second dosage
                case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }

        public static bool TherapyLayoutHasSecondDuration(BDConstants.LayoutVariantType pLayoutVariant)
        {
            bool returnValue = true;

            switch (pLayoutVariant)
            {
                // layouts wihout second duration
                case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
                case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }
        public static bool TherapyLayoutHasThirdDosage(BDConstants.LayoutVariantType pLayoutVariant)
        {
            bool returnValue = true;

            switch (pLayoutVariant)
            {
                // layouts without third dosage
                case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
                case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep:
                case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }
        public static bool TherapyLayoutHasThirdDuration(BDConstants.LayoutVariantType pLayoutVariant)
        {
            bool returnValue = true;

            switch (pLayoutVariant)
            {
                // layouts without third duration
                case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
                case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep:
                case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }

        public static bool TherapyLayoutHasCombinedDoseAndDuration(BDConstants.LayoutVariantType pLayoutVariant)
        {
            //NB:  the logic here is the REVERSE of the other like methods in this class, where the default is to return FALSE
            //      and only the TRUE conditions are listed here.
            bool returnValue = false;

            switch (pLayoutVariant)
            {
                case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                    returnValue = true;
                    break;
                default:
                    break;
            }
            return returnValue;
        }
    }
}
