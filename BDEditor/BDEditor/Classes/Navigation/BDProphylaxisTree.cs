using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.Classes;
using BDEditor.DataModel;

namespace BDEditor.Classes.Navigation
{
    public class BDProphylaxisTree
    {
        private BDProphylaxisTree() { }

        public static TreeNode BuildBranch(Entities pDataContext, IBDNode pNode)
        {
            if (null == pNode) return null;

            TreeNode branchTreeNode = new TreeNode(pNode.Name);
            List<IBDNode> childList = BDFabrik.GetChildrenForParent(pDataContext, pNode);

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDChapter:
                case BDConstants.BDNodeType.BDSection:
                case BDConstants.BDNodeType.BDCategory:
                case BDConstants.BDNodeType.BDSubcategory:
                case BDConstants.BDNodeType.BDDisease:
                case BDConstants.BDNodeType.BDTopic:
                case BDConstants.BDNodeType.BDSurgery:
                case BDConstants.BDNodeType.BDSurgeryClassification:
                case BDConstants.BDNodeType.BDSurgeryGroup:
                case BDConstants.BDNodeType.BDRegimen:
                case BDConstants.BDNodeType.BDMicroorganismGroup:
                case BDConstants.BDNodeType.BDTable:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_version1:
                        case BDConstants.LayoutVariantType.Prophylaxis:
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Intraoperative:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_I:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II:
                        case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
                        case BDConstants.LayoutVariantType.Prophylaxis_IE:
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization:
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk:
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine:
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_VaccineDetail:
                        case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis:
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
