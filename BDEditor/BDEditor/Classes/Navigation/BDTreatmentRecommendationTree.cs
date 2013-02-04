using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.Classes;
using BDEditor.DataModel;

namespace BDEditor.Classes.Navigation
{
    public class BDTreatmentRecommendationTree
    {
        private BDTreatmentRecommendationTree() { }

        public static TreeNode BuildBranch(Entities pDataContext, IBDNode pNode)
        {
            if(null == pNode) return null;

            TreeNode branchTreeNode = new TreeNode(pNode.Name);
            List<IBDNode> childList = BDFabrik.GetChildrenForParent(pDataContext, pNode);

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDChapter:
                case BDConstants.BDNodeType.BDSection:
                case BDConstants.BDNodeType.BDCategory:
                case BDConstants.BDNodeType.BDSubcategory:
                case BDConstants.BDNodeType.BDDisease:
                case BDConstants.BDNodeType.BDTable:
                case BDConstants.BDNodeType.BDTopic:
                case BDConstants.BDNodeType.BDSubtopic:
                case BDConstants.BDNodeType.BDResponse:
                case BDConstants.BDNodeType.BDPathogenGroup:
                case BDConstants.BDNodeType.BDPresentation:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation16_CultureDirected:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                            foreach (IBDNode childNode in childList)
                            {
                                string name = childNode.Name;
                                if ((null == name) || (name.Length == 0))
                                {
                                    //name = childNode.Uuid.ToString();
                                    name = @"< intentionally blank >";
                                }
                                TreeNode childTreeNode = new TreeNode(name);
                                childTreeNode.Tag = childNode;
                                branchTreeNode.Nodes.Add(childTreeNode);
                            }
                            break;
                    }
                    break;
            }

            return branchTreeNode;
        }
    }

}
