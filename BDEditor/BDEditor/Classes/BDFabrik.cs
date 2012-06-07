using System;
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
                    case (int)BDConstants.BDNodeType.BDChapter:
                    case (int)BDConstants.BDNodeType.BDSection:
                    case (int)BDConstants.BDNodeType.BDCategory:
                    case (int)BDConstants.BDNodeType.BDSubcategory:
                    case (int)BDConstants.BDNodeType.BDDisease:
                    case (int)BDConstants.BDNodeType.BDPathogenGroup:
                    case (int)BDConstants.BDNodeType.BDPathogen:
                        {
                            BDConstants.BDNodeType nType = (BDConstants.BDNodeType)nodeType;
                            nodeList.AddRange(BDNode.RetrieveNodesNameWithTextForType(pDataContext, pText,nType));
                        } break;
                    case (int)BDConstants.BDNodeType.BDTherapyGroup:
                        nodeList.AddRange(BDTherapyGroup.RetrieveTherapyGroupsNameWithText(pDataContext, pText));
                        break;
                    case (int)BDConstants.BDNodeType.BDTherapy:
                        {
                            nodeList.AddRange(BDTherapy.RetrieveTherapiesDosageWithText(pDataContext, pText));
                            nodeList.AddRange(BDTherapy.RetrieveTherapiesNameWithText(pDataContext, pText));
                            nodeList.AddRange(BDTherapy.RetrieveTherapiesDurationWithText(pDataContext, pText));
                        }
                        break;
                    default:
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
            Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]> result = new Tuple<BDConstants.BDNodeType,BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] {BDConstants.LayoutVariantType.TreatmentRecommendation01, BDConstants.LayoutVariantType.TreatmentRecommendation02_WoundMgmt});

            BDConstants.BDNodeType nodeType = result.Item1;
            BDConstants.LayoutVariantType[] variants = result.Item2;

            List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>> childDefinitionList = new List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>>();

            BDConstants.LayoutVariantType layoutVariant = pLayoutVariant;
            switch (pNodeType)
            {
                case BDConstants.BDNodeType.BDAntimicrobial:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosageGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosageGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
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
                        default:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDCategory:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation02_WoundMgmt, BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass,
                            BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I,BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II,BDConstants.LayoutVariantType.TreatmentRecommendation05_Peritonitis,BDConstants.LayoutVariantType.TreatmentRecommendation06_Meningitis,BDConstants.LayoutVariantType.TreatmentRecommendation07_Endocarditis }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubcategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        //Dental
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
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
                        case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                        case BDConstants.LayoutVariantType.Antibiotics:
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                        case BDConstants.LayoutVariantType.Dental: // Catch all for dental
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        case BDConstants.LayoutVariantType.PregancyLactation:
                        case BDConstants.LayoutVariantType.Organisms:
                           childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDDisease:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        //Dental
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDDosageGroup:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                        default:
                        break;
                    }
                    break;
                case BDConstants.BDNodeType.BDMicroorganism:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapy, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDMicroorganismGroup:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganism, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogen:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_Endocarditis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_Meningitis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenResistance, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogenGroup:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation05_Peritonitis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_Meningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_Endocarditis:
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
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_Meningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_Endocarditis:
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
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
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
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                        case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_3_Column }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_5_Column }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_4_Column }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubsection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;                        
                        case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                        case BDConstants.LayoutVariantType.Organisms:
                        case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;                        
                        case BDConstants.LayoutVariantType.Organisms_Antibiogram:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAttachment, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubcategory:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobialGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgery, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganism, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;

                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubsection:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
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
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                    }
                    break;

                case BDConstants.BDNodeType.BDSurgeryGroup:
                    switch (layoutVariant)
                    {
                        default:
                            // not in use yet - needed for chapter 3
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgeryClassification:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTable:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation02_WoundMgmt:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I:
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation05_Peritonitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_Meningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_Endocarditis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
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
                        //Dental
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity_HeaderRow, BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity_ContentRow }));
                            break;
                        // Pregancy/Lactation:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        // Organisms
                        case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Organisms_GramStainInterpretation_HeaderRow }));
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTableSection:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation02_WoundMgmt:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation02_WoundMgmt_ContentRow}));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass_ContentRow }));
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
                        //Dental
                        case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases_ContentRow }));
                            break;
                        case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Organisms_GramStainInterpretation_ContentRow }));
                            break;

                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTableSubsection:
                    switch (layoutVariant)
                    {
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
                        case BDConstants.LayoutVariantType.TreatmentRecommendation05_Peritonitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_Meningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_Endocarditis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
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
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_2_Column, BDConstants.LayoutVariantType.Table_3_Column, BDConstants.LayoutVariantType.Table_4_Column, BDConstants.LayoutVariantType.Table_5_Column }));
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            childDefinitionList = null;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis:
                            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubtopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                            break;
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

            switch (pNodeType)
            {
                case BDConstants.BDNodeType.None:
                    // do nothing
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
                default:
                    IQueryable<BDNode> nodeEntries = (from entry in pDataContext.BDNodes
                                                where entry.nodeType == (int)pNodeType
                                                orderby entry.displayOrder
                                                select entry);

                    if (nodeEntries.Count() > 0)
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
            }

            entryList.Sort(delegate(IBDNode n1, IBDNode n2) 
            {
                if (n1.DisplayOrder == null && n2.DisplayOrder == null) return 0;

                else if (n1.DisplayOrder != null && n2.DisplayOrder == null) return -1;

                else if (n1.DisplayOrder == null && n2.DisplayOrder != null) return 1;
                
                else
                    return (n1.DisplayOrder as IComparable).CompareTo(n2.DisplayOrder as IComparable);
            });

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
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        IQueryable<BDTherapyGroup> tgEntries = (from entry in pContext.BDTherapyGroups
                                                                where entry.parentId == pParentId
                                                                orderby entry.displayOrder ascending
                                                                select entry);
                        if (tgEntries.Count() > 0)
                        {
                            List<IBDNode> workingList = new List<IBDNode>(tgEntries.ToList<BDTherapyGroup>());
                            entryList.AddRange(workingList);
                        }
                        break;

                    case BDConstants.BDNodeType.BDTherapy:
                        IQueryable<BDTherapy> tEntries = (from entry in pContext.BDTherapies
                                                          where entry.parentId == pParentId
                                                          orderby entry.displayOrder ascending
                                                          select entry);
                        if (tEntries.Count() > 0)
                        {
                            List<IBDNode> workingList = new List<IBDNode>(tEntries.ToList<BDTherapy>());
                            entryList.AddRange(workingList);
                        }
                        break;
                    
                    case BDConstants.BDNodeType.BDTableRow:
                        IQueryable<BDTableRow> trEntries = (from entry in pContext.BDTableRows
                                                            where entry.parentId == pParentId
                                                            orderby entry.displayOrder ascending
                                                            select entry);
                        if (trEntries.Count() > 0)
                        {
                            List<IBDNode> workingList = new List<IBDNode>(trEntries.ToList<BDTableRow>());
                            entryList.AddRange(workingList);
                        }
                        break;
                    case BDConstants.BDNodeType.BDTableCell:
                        IQueryable<BDTableCell> tcEntries = (from entry in pContext.BDTableCells
                                                            where entry.parentId == pParentId
                                                            orderby entry.displayOrder ascending
                                                            select entry);
                        if (tcEntries.Count() > 0)
                        {
                            List<IBDNode> workingList = new List<IBDNode>(tcEntries.ToList<BDTableCell>());
                            entryList.AddRange(workingList);
                        }
                        break;
                    case BDConstants.BDNodeType.BDDosage:
                        IQueryable<BDDosage> doEntries = (from entry in pContext.BDDosages
                                                            where entry.parentId == pParentId
                                                            orderby entry.displayOrder ascending
                                                            select entry);
                        if (doEntries.Count() > 0)
                        {
                            List<IBDNode> workingList = new List<IBDNode>(doEntries.ToList<BDDosage>());
                            entryList.AddRange(workingList);
                        }
                        break;

                    case BDConstants.BDNodeType.BDPrecaution:
                        IQueryable<BDPrecaution> prEntries = (from entry in pContext.BDPrecautions
                                                            where entry.parentId == pParentId
                                                            orderby entry.displayOrder ascending
                                                            select entry);
                        if (prEntries.Count() > 0)
                        {
                            List<IBDNode> workingList = new List<IBDNode>(prEntries.ToList<BDPrecaution>());
                            entryList.AddRange(workingList);
                        }
                        break;

                    case BDConstants.BDNodeType.BDAntimicrobialRisk:
                        IQueryable<BDAntimicrobialRisk> arEntries = (from entry in pContext.BDAntimicrobialRisks
                                                            where entry.parentId == pParentId
                                                            orderby entry.displayOrder ascending
                                                            select entry);
                        if (arEntries.Count() > 0)
                        {
                            List<IBDNode> workingList = new List<IBDNode>(arEntries.ToList<BDAntimicrobialRisk>());
                            entryList.AddRange(workingList);
                        }
                        break;

                    default:
                        IQueryable<BDNode> nodeEntries = (from entry in pContext.BDNodes
                                                          where (entry.parentId == pParentId) && (entry.nodeType == (int)pChildNodeType)
                                                          orderby entry.displayOrder ascending
                                                          select entry);

                        if (nodeEntries.Count() > 0)
                        {
                            List<IBDNode> workingList = new List<IBDNode>(nodeEntries.ToList<BDNode>());
                            entryList.AddRange(workingList);
                        }
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
                    BDAntimicrobialRisk risk = BDAntimicrobialRisk.CreateBDAntimicrobialRisk(pContext, pChildType);
                    risk.displayOrder = siblingList.Count;
                    risk.SetParent(pParentNode);
                    risk.LayoutVariant = pLayoutVariant;
                    BDAntimicrobialRisk.Save(pContext, risk);
                    result = risk;
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
                case BDConstants.BDNodeType.BDTherapyGroup:
                    BDTherapyGroup therapyGroup = BDTherapyGroup.CreateBDTherapyGroup(pContext, pParentNode.Uuid);
                    therapyGroup.displayOrder = siblingList.Count;
                    therapyGroup.SetParent(pParentNode);
                    therapyGroup.LayoutVariant = pLayoutVariant;
                    therapyGroup.Name = String.Format("New Therapy Group {0}", therapyGroup.displayOrder + 1);
                    BDTherapyGroup.Save(pContext, therapyGroup);
                    result = therapyGroup;
                    break;

                case BDConstants.BDNodeType.BDTherapy:
                    BDTherapy therapy = BDTherapy.CreateBDTherapy(pContext, pParentNode.Uuid);
                    therapy.displayOrder = siblingList.Count;
                    therapy.SetParent(pParentNode);
                    therapy.LayoutVariant = pLayoutVariant;
                    therapy.Name = String.Format("New Therapy {0}", therapy.displayOrder + 1);
                    BDTherapy.Save(pContext, therapy);
                    result = therapy;
                    break;

                case BDConstants.BDNodeType.BDTableRow:
                    BDTableRow tableRow = BDTableRow.CreateBDTableRow(pContext, pChildType);
                    tableRow.displayOrder = siblingList.Count;
                    tableRow.SetParent(pParentNode);
                    tableRow.LayoutVariant = pLayoutVariant;
                    tableRow.Name = String.Format("New {0}-{1}", BDUtilities.GetEnumDescription(pChildType), tableRow.displayOrder + 1);
                    tableRow.rowType = -1;
                    BDTableRow.Save(pContext, tableRow);
                    result = tableRow;

                    // add BDTableCells for the row
                    int cellCount = GetTableColumnCount(pLayoutVariant);
                    for (int i = 0; i < cellCount; i++)
                    {
                        BDTableCell tableCell = BDTableCell.CreateBDTableCell(pContext);
                        tableCell.displayOrder = i + 1;
                        tableCell.SetParent(tableRow);
                        tableCell.alignment = 0;
                        BDTableCell.Save(pContext, tableCell);

                        BDString cellValue = BDString.CreateBDString(pContext);
                        cellValue.displayOrder = i;
                        cellValue.SetParent(tableCell.Uuid);
                        BDString.Save(pContext, cellValue);
                    }
                    
                    break;

                case BDConstants.BDNodeType.BDTable:
                default:
                    BDNode node = BDNode.CreateBDNode(pContext, pChildType);
                    node.displayOrder = siblingList.Count;
                    node.SetParent(pParentNode);
                    node.LayoutVariant = pLayoutVariant;
                    node.Name = String.Format("New {0}-{1}", BDUtilities.GetEnumDescription(pChildType), node.displayOrder + 1);
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
                case BDConstants.BDNodeType.BDTherapyGroup:
                    BDTherapyGroup therapyGroup = pNode as BDTherapyGroup;
                    BDTherapyGroup.Delete(pContext, therapyGroup, pCreateDeletionRecord);
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
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        result = BDTherapyGroup.RetrieveTherapyGroupWithId(pContext, pUuid.Value);
                        
                        break;

                    case BDConstants.BDNodeType.BDTherapy:
                        result = BDTherapy.RetrieveTherapyWithId(pContext, pUuid.Value);
                        break;

                    case BDConstants.BDNodeType.BDTableRow:
                        result = BDTableRow.RetrieveTableRowWithId(pContext, pUuid.Value);
                        break;

                    default:
                        result = BDNode.RetrieveNodeWithId(pContext, pUuid.Value);
                        break;
                }
            }
            return result;
        }

        public static IBDNode CreateNode(Entities pContext, BDConstants.BDNodeType pNodeType, Guid? pParentUuid, BDConstants.BDNodeType pParentNodeType)
        {
            IBDNode entry = null;
            switch (pNodeType)
            {
                case BDConstants.BDNodeType.None:
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
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            {
                                nodeControl = new BDNodeOverviewControl(pContext, pNode);
                                BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                                newOverviewControl.ShowAsChild = true;
                                newOverviewControl.ShowSiblingAdd = true;
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
                                newControl.ShowAsChild = true;
                                newControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDCategory:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            break;
                        default:
                            nodeControl = new BDNodeOverviewControl(pContext,pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDDisease:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDDosage:
                    switch (pNode.LayoutVariant)
                    {
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
                case BDConstants.BDNodeType.BDMicroorganism:
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
                case BDConstants.BDNodeType.BDMicroorganismGroup:
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
                case BDConstants.BDNodeType.BDSection:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Organisms_Antibiogram:
                            nodeControl = new BDAttachmentControl(pContext, pNode);
                            break;
                        default:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubcategory:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            break;
                        case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl newNodeControl = nodeControl as BDNodeControl;
                            newNodeControl.ShowAsChild = false;
                            newNodeControl.ShowSiblingAdd = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                            newOverviewControl.ShowAsChild = true;
                            newOverviewControl.ShowSiblingAdd = true;
                            break;
                        default:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubsection:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubtopic:
                    switch (pNode.LayoutVariant)
                    {
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
                                nodeControl = new BDNodeOverviewControl(pContext, pNode);
                                BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                                newOverviewControl.ShowAsChild = true;
                                newOverviewControl.ShowSiblingAdd = true;
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgery:
                case BDConstants.BDNodeType.BDSurgeryClassification:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                                nodeControl = new BDNodeControl(pContext, pNode);
                                BDNodeControl newControl = nodeControl as BDNodeControl;
                                newControl.ShowAsChild = false;
                                newControl.ShowSiblingAdd = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTable:
                    switch (pNode.LayoutVariant)
                    { 
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
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms_ContentRow:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                            newOverviewControl.ShowAsChild = true;
                            newOverviewControl.ShowSiblingAdd = true;
                            break;
                        default:
                            nodeControl = new BDTableCellControl();
                            BDTableCellControl cellControl = nodeControl as BDTableCellControl;

                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTableRow:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation02_WoundMgmt:
                            nodeControl = new BDNodeOverviewControl(pContext, pNode);
                            BDNodeOverviewControl newOverviewControl = nodeControl as BDNodeOverviewControl;
                            newOverviewControl.ShowAsChild = true;
                            newOverviewControl.ShowSiblingAdd = true;
                            break;
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
                    switch(pNode.LayoutVariant)
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
                            }
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTopic:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis:
                            nodeControl = new BDNodeControl(pContext, pNode);
                            BDNodeControl newControl = nodeControl as BDNodeControl;
                            newControl.ShowAsChild = true;
                            newControl.ShowSiblingAdd = true;
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

                default:
                    // Require explicit handling for given child types
                    // i.e. disease does not currently display children within this control
                    // Don't load children if not explicitly supported here
                    break;
            }
            return nodeControl;

        }

        public static int GetTableColumnCount(BDConstants.LayoutVariantType pLayoutVariant)
        {
            int maxColumns = 0;
            switch (pLayoutVariant)
            {
                case BDConstants.LayoutVariantType.TreatmentRecommendation02_WoundMgmt:
                case BDConstants.LayoutVariantType.TreatmentRecommendation02_WoundMgmt_ContentRow:
                    // no cells:  data is stored in the Name property and in a Linked Note (type Overview)
                    maxColumns = 0;
                    break;
                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I:
                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I_ContentRow:
                case BDConstants.LayoutVariantType.Table_2_Column:
                case BDConstants.LayoutVariantType.Table_2_Column_HeaderRow:
                case BDConstants.LayoutVariantType.Table_2_Column_ContentRow:
                case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis:
                case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms_ContentRow:
                case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation_HeaderRow:
                case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation_ContentRow:
                    maxColumns = 2;
                    break;
                case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                case BDConstants.LayoutVariantType.Table_3_Column:
                case BDConstants.LayoutVariantType.Table_3_Column_HeaderRow:
                case BDConstants.LayoutVariantType.Table_3_Column_ContentRow:
                case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
                case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                    maxColumns = 3;
                    break;

                case BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass:
                case BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass_HeaderRow:
                case BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass_ContentRow:
                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II:
                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_HeaderRow:
                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_ContentRow:
                case BDConstants.LayoutVariantType.Table_4_Column:
                case BDConstants.LayoutVariantType.Table_4_Column_HeaderRow:
                case BDConstants.LayoutVariantType.Table_4_Column_ContentRow:
                case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                    maxColumns = 4;
                    break;
                case BDConstants.LayoutVariantType.Table_5_Column:
                case BDConstants.LayoutVariantType.Table_5_Column_HeaderRow:
                case BDConstants.LayoutVariantType.Table_5_Column_ContentRow:
                case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases_HeaderRow:
                case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases_ContentRow:
                    maxColumns = 5;
                    break;
                case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity_ContentRow:
                case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity_HeaderRow:
                    maxColumns = 8;
                    break;
                default:
                    maxColumns = 4;
                    break;
            }
            return maxColumns;
        }
    }
}
