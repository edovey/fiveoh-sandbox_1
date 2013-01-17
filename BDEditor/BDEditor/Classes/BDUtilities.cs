using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Reflection;
using System.ComponentModel;
using BDEditor.DataModel;
using BDEditor.Classes;

using System.Security.Permissions;
using Microsoft.Win32;

namespace BDEditor.Classes
{
    public class BDUtilities
    {
        private BDUtilities() { }

        public static string GetEnumDescription(Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return value.ToString();
        }

        public static string GetEnumDescriptionWithNumber(Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            string number = string.Format("{0:D}", value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return string.Format("{0} [{1}]", attr.Description, number);
                    }
                }
            }
            return string.Format("{0} [{1}]",value.ToString(), number);
        }

        /// <summary>
        /// Determine the parent hierarchy of the selected entry
        /// </summary>
        /// <param name="pStartNode"></param>
        /// <returns></returns>
        public static string BuildHierarchyString(Entities pContext, IBDNode pStartNode, string pSeparationString)
        {
            StringBuilder hStringBuilder = new StringBuilder();
            if (pStartNode != null)
                return getParentName(pContext, pStartNode, hStringBuilder, pSeparationString).ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// recursive call to determine the parent name of the node
        /// </summary>
        /// <param name="pNode"></param>
        /// <param name="pHierarchyValue"></param>
        /// <returns></returns>
        private static StringBuilder getParentName(Entities pContext, IBDNode pNode, StringBuilder pHierarchyValue, string pSeparationString)
        {
            if (null != pNode && pNode.ParentId != Guid.Empty)
            {
                IBDNode parentNode = BDFabrik.RetrieveNode(pContext, pNode.ParentType, pNode.ParentId);
                if (null != parentNode)
                {
                    pHierarchyValue.Insert(0, string.Format("{0}{1}", parentNode.Name, pSeparationString));
                    if (parentNode.ParentId != Guid.Empty)
                        pHierarchyValue = getParentName(pContext, parentNode, pHierarchyValue, pSeparationString);
                }
            }
            return pHierarchyValue;
        }

        public static void InjectNodeIntoHierarhy(Entities pContext)
        {
            BDNode disease = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("96cbc7d0-c4ba-4593-a1d9-0e7908deeffd"));
            BDNode newTable = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDTable, Guid.NewGuid());
            newTable.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers;
            newTable.SetParent(disease);
            newTable.name = "Genital ulcers / lesions table";
            BDNode.Save(pContext, newTable);

            BDNode gTopic1 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("32379749-9852-46f4-884b-e2443dd02805"));
            gTopic1.SetParent(newTable);
            BDNode.Save(pContext, gTopic1);

            BDNode gTopic2 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("8b3f3d30-3be9-4aef-89d3-acbbd2c1e147"));
            gTopic2.SetParent(newTable);
            BDNode.Save(pContext, gTopic2);

            BDNode gTopic3 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("9cf8dbb0-817f-4c3d-9228-c02d959f02c7"));
            gTopic3.SetParent(newTable);
            BDNode.Save(pContext, gTopic3);

            BDNode gTopic4 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("a0e6a20b-c494-4a98-b6dc-00dea74da92d"));
            gTopic4.SetParent(newTable);
            BDNode.Save(pContext, gTopic4);

            BDNode gTopic5 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("d2b8a93f-eda0-40ef-8e3c-368321689d16"));
            gTopic5.SetParent(newTable);
            BDNode.Save(pContext, gTopic5);

            BDNode gTopic6 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("e5ebc317-d83c-4bb3-bc66-df1404bbed25"));
            gTopic6.SetParent(newTable);
            BDNode.Save(pContext, gTopic6);

            BDNode gTopic7 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("f9b33cc4-122b-4f27-bc8e-8dd20d31e94f"));
            gTopic7.SetParent(newTable);
            BDNode.Save(pContext, gTopic7);
        }

        public static byte[] ReadFileAsByteArray(string fullFilenameAndPath)
        {
            if (!File.Exists(fullFilenameAndPath))
                throw new IOException("File does not exist. File: " + fullFilenameAndPath);
            FileInfo file = new FileInfo(fullFilenameAndPath);

            if (file.IsReadOnly) return null;

            byte[] fileAsByteArray;

            try
            {
                using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        fileAsByteArray = br.ReadBytes((int)fs.Length);
                        //br.Close();
                        //fs.Close();
                    }
                }
            }
            catch (IOException ex)
            {
                throw new Exception("Error while accessing. File: " + fullFilenameAndPath, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("File Error: " + fullFilenameAndPath, ex);
            }
            return fileAsByteArray;
        }

        public static void WriteByteArrayAsFile(string fullFilenameAndPath, byte[] fileAsByteArray)
        {
            try
            {
                FileInfo file = new FileInfo(fullFilenameAndPath);
                using (FileStream fs = new FileStream(file.FullName, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(fileAsByteArray);
                        //bw.Close();
                        //fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while writing file: " + fullFilenameAndPath, ex);
            }
        }

        public static string GetMIMEType(FileInfo fi)
        {
            new RegistryPermission(RegistryPermissionAccess.Read, "\\\\HKEY_CLASSES_ROOT");

            RegistryKey classesRoot = Registry.ClassesRoot;

            string dotExt = fi.Extension;

            RegistryKey typeKey = classesRoot.OpenSubKey("MIME\\Database\\Content Type");


            foreach (string keyname in typeKey.GetSubKeyNames())
            {
                RegistryKey curKey = classesRoot.OpenSubKey("MIME\\Database\\Content Type\\" + keyname);


                if (curKey != null && curKey.GetValue("Extension") != null &&
                    curKey.GetValue("Extension").ToString().ToLower() == dotExt)
                {
                    return keyname;
                }
            }
            return string.Empty;
        }

        public static string FilterFilenameForInvalidChars(string input)
        {
            foreach (char invalidChar in System.IO.Path.GetInvalidFileNameChars())
            {
                int invalidCharIndex = input.IndexOf(invalidChar);

                while (invalidCharIndex > -1)
                {
                    input = input.Remove(invalidCharIndex, 1);

                    invalidCharIndex = input.IndexOf(invalidChar);
                }
            }

            foreach (char invalidChar in System.IO.Path.GetInvalidPathChars())
            {
                int invalidCharIndex = input.IndexOf(invalidChar);

                while (invalidCharIndex > -1)
                {
                    input = input.Remove(invalidCharIndex, 1);

                    invalidCharIndex = input.IndexOf(invalidChar);
                }
            }

            return input;
        }

        public static void ExecuteBatchMove(Entities pContext)
        {
            // These operations are CUSTOM, ** BY REQUEST ONLY **

            // the following are complete, but left as samples.

            #region earlier
            /*
            // move Pharyngitis to Respiratory
            BDNode node = BDNode.RetrieveNodeWithId(dataContext, new Guid("a55e8e97-16cf-4074-ae98-26627bb25143"));
            BDUtilities.MoveNode(dataContext, node, "Respiratory");

            // move Zoster subentries to Vesicular Lesions
            BDUtilities.MoveAllChildren(dataContext, new Guid("10dce9db-6b5a-4249-b156-c86b18635852"), new Guid("5ce93aff-137f-487b-8f8a-1e9076acc8cb"));

            // Move Gastroenteritis Mild-moderate to Gastroenteritis - Severe 
            //BDNode node = BDNode.RetrieveNodeWithId(dataContext, new Guid("f80d2660-66c0-4640-9d35-c4f90a369a97"));
            //BDUtilities.MoveNode(dataContext, node, "Gastroenteritis - Severe");
             */

            //BDNode node = BDNode.RetrieveNodeWithId(dataContext, new Guid("10dce9db-6b5a-4249-b156-c86b18635852"));
            //node.parentId = new Guid("5ce93aff-137f-487b-8f8a-1e9076acc8cb");
            //node.parentKeyName = "BDDisease";
            //node.nodeKeyName = "BDPresentation";
            //node.nodeType = (int)BDConstants.BDNodeType.BDPresentation;

            //BDNode.Save(dataContext, node);

            // reset node type for all table cells
            //BDUtilities.SetTableCellNodeType(dataContext);

            // reset layout variant for TreatmentRecommendation>Adult>Genital>GenitalUlcers
            //BDUtilities.SetDiseaseLayoutVariant(dataContext);

            // repair hierarchy for Genital Ulcers/ lesions in Treatment Recommendations > Adult > Genital...
            // BDUtilities.InjectNodeIntoHierarhy(dataContext);

            // reset all layoutVariants for children of Adults > SSTI > Vesicular Lesions
            // IBDNode startNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("5ce93aff-137f-487b-8f8a-1e9076acc8cb"));
            // BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode,BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions, false);

            // reset all layoutVariants for children of Peds > SSTI > Vesicular Lesions
            // IBDNode startNode2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a088c4d8-46d6-4b9f-a31b-bbf8a64f22e9"));
            // BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode2, BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions, false);

            // reset layoutVariant for Adults > Respiratory > Table xx Treatment of culture-proven pneumonia and all children 
            //IBDNode startNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("391bd1a4-daca-44e8-8fda-863f22128f1f"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode, BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia, true);

            ////Gastroenteritis node: 8bb5b60e-68fd-4df2-bb77-f8f6e2f17c48 adult  d1c0f6ca-a501-4635-9241-aaeded1877be paediatric
            //// Change layoutVariant from TreatmentRecommendation01 (101) to TreatmentRecommendation01_Gastroenteritis (10141)
            //IBDNode startNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("8bb5b60e-68fd-4df2-bb77-f8f6e2f17c48"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis, true);

            //IBDNode startNode2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("d1c0f6ca-a501-4635-9241-aaeded1877be"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode2, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis, true);

            //// Move children of Severe to culture-directed
            //// severe #1 e83a8e3f-10b4-4677-aa73-197aa8ce5c8c , target & exception: 879a8931-648f-4e16-9704-2d30ab0d76f9 exception: 14be53d4-a30a-4037-98a1-077445d78202
            //// severe #2 f80d2660-66c0-4640-9d35-c4f90a369a97 , target & exception: 874a2a14-c7c2-41dd-8799-80c13392441b exception: 62c6cfcc-cf6d-4640-9335-4327e937d415

            //List<Guid> exceptionList1 = new List<Guid>();
            //List<Guid> exceptionList2 = new List<Guid>();

            //exceptionList1.Add(Guid.Parse("879a8931-648f-4e16-9704-2d30ab0d76f9"));
            //exceptionList1.Add(Guid.Parse("14be53d4-a30a-4037-98a1-077445d78202"));

            //exceptionList2.Add(Guid.Parse("874a2a14-c7c2-41dd-8799-80c13392441b"));
            //exceptionList2.Add(Guid.Parse("62c6cfcc-cf6d-4640-9335-4327e937d415"));

            //BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("e83a8e3f-10b4-4677-aa73-197aa8ce5c8c"), Guid.Parse("879a8931-648f-4e16-9704-2d30ab0d76f9"),  exceptionList1);
            //BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("f80d2660-66c0-4640-9d35-c4f90a369a97"), Guid.Parse("874a2a14-c7c2-41dd-8799-80c13392441b"), exceptionList2);

            //IBDNode startNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("879a8931-648f-4e16-9704-2d30ab0d76f9"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected, true);

            // //IBDNode startNode2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("874a2a14-c7c2-41dd-8799-80c13392441b"));
            // //BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode2, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected, true);
            // exceptionList1 = new List<Guid>();
            // //Move the "single presentation" children to the parent of the presentation
            // BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("2af20800-f237-4e7c-b270-a91ea35f3d13"), Guid.Parse("9e322d09-ae3d-41a2-b7c9-c6ef706bc986"), exceptionList1);
            // BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("77637ddd-81a6-4d9a-a92f-f7811c6e9446"), Guid.Parse("e10f7eb9-66d5-4225-b01e-06a0acefe34c"), exceptionList1);

            // //Move the children of (Adult) Necrotizing fasciitis/myositis down into the manually created subsection
            // exceptionList1 = new List<Guid>();
            // exceptionList1.Add(Guid.Parse("1323d38a-2cd4-4f38-a976-09ce67dae5ec"));
            // BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("9e2409c4-1ef0-49a9-ab20-887954f25ca0"), Guid.Parse("1323d38a-2cd4-4f38-a976-09ce67dae5ec"), exceptionList1);

            // //Move the children of (Child) Necrotizing fasciitis/myositis down into the manually created subsection
            // exceptionList1 = new List<Guid>();
            // exceptionList1.Add(Guid.Parse("91da8752-752a-43de-a315-159e3baeb838"));
            // BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("44e9114b-d3d5-4fa6-a56f-ce3eb346d6eb"), Guid.Parse("91da8752-752a-43de-a315-159e3baeb838"), exceptionList1);

            // // Manually change the nodeType and NodeKeyName from disease to presentation (5 - BDDisease) (6 - BDPresentation)
            // // Manually change parent info from (3 - BDCategory) to (5 - BDDisease)
            ////  9e322d09-ae3d-41a2-b7c9-c6ef706bc986	Synergistic necrotizing cellulitis, Fournier's gangrene	
            ////  e10f7eb9-66d5-4225-b01e-06a0acefe34c	Gas gangrene

            // delete orphan table
            //BDNode.DeleteLocal(dataContext, Guid.Parse(@"5c75848a-bf64-45e5-93fe-670e8790a13c"));

            //// create new antimicrobialSection for 'Vancomycin Dosing And Monitoring...'
            //BDNode chapter = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"45e13826-aedb-48d0-baf6-2f06ff45017f"));
            //BDNode antimicrobialSection = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection);
            //antimicrobialSection.name = @"Vancomycin Dosing and Monitoring Guidelines";
            //antimicrobialSection.SetParent(chapter);
            //antimicrobialSection.displayOrder = 0;
            //antimicrobialSection.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring;
            //BDNode.Save(dataContext, antimicrobialSection);

            //// retrieve Vancomycin Dosing - adjust type and make a child of the new antimicrobialSection 
            //BDNode vd = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"2dd4578a-a856-49e4-852a-e18eb43fb1b2"));
            //List<IBDNode> vdChildren = BDFabrik.GetChildrenForParent(dataContext, vd);
            //vd.SetParent(antimicrobialSection);
            //vd.nodeType = (int)BDConstants.BDNodeType.BDCategory;
            //vd.nodeKeyName = vd.nodeType.ToString();
            //BDNode.Save(dataContext, vd);
            //foreach (IBDNode vdChild in vdChildren)
            //{   // reset parentType and parentKeyName or lookups will break
            //    vdChild.SetParent(vd);
            //    BDNode.Save(dataContext, vdChild as BDNode);
            //}
            //// retrieve Vancomycin Monitoring
            //BDNode vm = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"10a23f72-58ea-49c2-ad3f-d8d97d1846c6"));
            //List<IBDNode> vmChildren = BDFabrik.GetChildrenForParent(dataContext, vm);
            //vm.SetParent(antimicrobialSection);
            //vm.nodeType = (int)BDConstants.BDNodeType.BDCategory;
            //vm.nodeKeyName = vm.nodeType.ToString();
            //BDNode.Save(dataContext, vm);
            //foreach (IBDNode vmChild in vmChildren)
            //{
            //    vmChild.SetParent(vm);
            //    BDNode.Save(dataContext, vmChild as BDNode);
            //}

            // move values from BDString object up to table cell as 'value'
            //List<BDTableCell> cells = BDTableCell.RetrieveAll(dataContext);
            //foreach (BDTableCell cell in cells)
            //{
            //    StringBuilder cellString = new StringBuilder();
            //    List<BDString> strings = BDString.RetrieveStringsForParentId(dataContext, cell.Uuid);
            //    foreach (BDString stringValue in strings)
            //        cellString.Append(stringValue.value);

            //    cell.value = cellString.ToString();
            //    BDTableCell.Save(dataContext, cell);
            //}

            //// fetch the antimicrobial name listing table
            //BDNode nameTable = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"13acf8a8-6f81-4ba1-a5ca-620dc978596b"));
            //if (nameTable.name == "Antimicrobial Formulary and Generic / Trade Name Listing")
            //{
            //    List<BDNode> nameTableSections = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, nameTable.Uuid, BDConstants.BDNodeType.BDSection);
            //    // Adjust layout variant for children
            //    foreach (BDNode tableSection in nameTableSections)
            //    {
            //        List<BDNode> subsections = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, tableSection.Uuid, BDConstants.BDNodeType.BDSubsection);
            //        BDUtilities.ResetLayoutVariantInTable3RowsForParent(dataContext, tableSection, BDConstants.LayoutVariantType.Antibiotics_NameListing);
            //        tableSection.LayoutVariant = nameTable.LayoutVariant;
            //        BDNode.Save(dataContext, tableSection);
            //        foreach (BDNode subsection in subsections)
            //        {
            //            BDUtilities.ResetLayoutVariantInTable3RowsForParent(dataContext, subsection, BDConstants.LayoutVariantType.Antibiotics_NameListing);
            //            subsection.LayoutVariant = nameTable.LayoutVariant;
            //            BDNode.Save(dataContext, subsection);
            //        }
            //    }
            //    // reset header tableChildren
            //    BDUtilities.ResetLayoutVariantInTable3RowsForParent(dataContext, nameTable, BDConstants.LayoutVariantType.Antibiotics_NameListing);
            //}

            //// move Adults > SSTI > Fournier's Gangrene to presentation level of Rapidly progressive SSTI
            //BDUtilities.MoveNodeToParentSibling(dataContext, Guid.Parse(@"9e322d09-ae3d-41a2-b7c9-c6ef706bc986"), Guid.Parse(@"6e4c8849-ad12-4b5f-b5fc-5fc53288cfad"), "SINGLE PRESENTATION", BDConstants.BDNodeType.BDPresentation);

            //// move Adults > SSTI > Gas Gangrene to presentation level of Rapidly progressive SSTI
            //BDUtilities.MoveNodeToParentSibling(dataContext, Guid.Parse(@"e10f7eb9-66d5-4225-b01e-06a0acefe34c"), Guid.Parse(@"6e4c8849-ad12-4b5f-b5fc-5fc53288cfad"), "SINGLE PRESENTATION", BDConstants.BDNodeType.BDPresentation);
            #endregion
            #region v.1.6.12
            // fetch the antimicrobial stepdown table
            //BDNode nameTable = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"45301fa9-55ac-4c8f-95f0-1008016635c4"));
            //if (nameTable.name == "Stepdown Recommendations")
            //{
            //    List<BDNode> nameTableSections = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, nameTable.Uuid, BDConstants.BDNodeType.BDTableSection);
            //    // Adjust layout variant for children
            //    foreach (BDNode tableSection in nameTableSections)
            //    {
            //        List<BDNode> subsections = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, tableSection.Uuid, BDConstants.BDNodeType.BDSubsection);
            //        BDUtilities.ResetLayoutVariantInTable5RowsForParent(dataContext, tableSection, BDConstants.LayoutVariantType.Antibiotics_Stepdown);
            //        tableSection.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Stepdown;
            //        BDNode.Save(dataContext, tableSection);
            //        foreach (BDNode subsection in subsections)
            //        {
            //            BDUtilities.ResetLayoutVariantInTable5RowsForParent(dataContext, subsection, BDConstants.LayoutVariantType.Antibiotics_Stepdown);
            //            subsection.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Stepdown;
            //            BDNode.Save(dataContext, subsection);
            //        }
            //    }
            //    // reset header tableChildren
            //    BDUtilities.ResetLayoutVariantInTable5RowsForParent(dataContext, nameTable, BDConstants.LayoutVariantType.Antibiotics_Stepdown);
            //}

            //BDNode vancomycin = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"c42a29c2-f5a1-48a4-b140-3e4dae56b445"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, vancomycin, BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin, true);

            #endregion
            #region v.1.6.13
            /*
            // move selected tables out of Treatment Recommendations > Adults into new antimicrobialSection Treatment recommendations > Culture Directed Infections in Adults
            // create new antimicrobialSection in TreatmentRecoomendations chapter
            BDNode chapter = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("f92fec3a-379d-41ef-a981-5ddf9c9a9f0e"));
            BDNode newSection = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection);
            newSection.SetParent(chapter);
            newSection.name = "Recommended Empiric Therapy of Culture-Directed Infections in Adult Patients";
            newSection.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation16_CultureDirected;
            BDNode.Save(dataContext, newSection);

            // move related tables to new antimicrobialSection & assign new layout variant
            // get culture-proven pneumonia table
            BDNode table1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("391bd1a4-daca-44e8-8fda-863f22128f1f"));
            table1.SetParent(newSection);
            table1.DisplayOrder = 0;
            BDNode.Save(dataContext, table1);

            // get culture-proven PD Peritonitis table
            BDNode table2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("5b8548c8-0e54-4cb5-910e-2f55a41f9ecc"));
            table2.SetParent(newSection);
            table2.DisplayOrder = 1;
            BDNode.Save(dataContext, table2);

            // get culture-proven meningitis table
            BDNode table3 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("fc25fee8-8ded-4d41-895d-b415bab02eb7"));
            table3.SetParent(newSection);
            table3.DisplayOrder = 2;
            BDNode.Save(dataContext, table3);

            // get culture-proven endocarditis table
            BDNode table4 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("8741d365-16ef-4de8-8d08-dabc574c010a"));
            table4.SetParent(newSection);
            table4.DisplayOrder = 3;
            BDNode.Save(dataContext, table4);

            // get culture-proven BCNE table
            BDNode table5 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("b38a1c03-2f74-4b08-b104-0da7f054c529"));
            table5.SetParent(newSection);
            table5.DisplayOrder = 4;
            BDNode.Save(dataContext, table5);

            // change pathogen groups in Antimicrobial_ClinicalGuidelines  to topics
            List<BDNode> therapyGroups = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDPathogenGroup);
            foreach(BDNode therapyGroup in therapyGroups)
            {
                if (therapyGroup.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines)
                {
                    if (therapyGroup.Name == "Predictable Activity" || therapyGroup.Name == "Unpredictable Activity" || therapyGroup.Name == "No/insufficient Activity")
                    {
                        therapyGroup.nodeType = (int)BDConstants.BDNodeType.BDTopic;
                        therapyGroup.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
                        BDNode.Save(dataContext, therapyGroup);
                    }
                }
            }

            // add categories into Antimicrobial_ClinicalGuidelines, move child nodes of the antimicrobialSection into the categories.
            BDNode antimicrobialSection = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("01933c56-dedf-4c1a-9191-d541819000d8"));
            BDNode c1 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory);
            c1.SetParent(antimicrobialSection);
            c1.name = "ANTIMICROBIAL SPECTRUM";
            c1.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum;
            c1.DisplayOrder = 0;
            BDNode.Save(dataContext, c1);

            BDNode c2 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory);
            c2.SetParent(antimicrobialSection);
            c2.Name = "ANTIMICROBIAL AGENTS";
            c2.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
            c2.DisplayOrder = 1;
            BDNode.Save(dataContext, c2);

            BDNode c3 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory);
            c3.SetParent(antimicrobialSection);
            c3.Name = "ANTIFUNGALS";
            c3.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
            c3.DisplayOrder = 2;
            BDNode.Save(dataContext, c3);

            // move selected Antimicrobials to new category for Antifungals
            BDNode af1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("bf1d8db1-7243-4139-90b9-60f10c2e4953"));
            af1.SetParent(c3);
            BDNode.Save(dataContext, af1);

            BDNode af2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("7a0fd527-3afd-4f57-9419-1d56eaf17110"));
            af2.SetParent(c3);
            BDNode.Save(dataContext, af2);

            BDNode af3 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("afe6f889-4289-4255-9dd4-be193568bbbf"));
            af3.SetParent(c3);
            BDNode.Save(dataContext, af3);

            BDNode af4 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial);
            af4.SetParent(c3);
            af4.Name = "ITRACONAZOLE";
            af4.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
            BDNode.Save(dataContext, af4);

            BDNode af5 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial);
            af5.SetParent(c3);
            af5.Name = "MICAFUNGIN IV";
            af5.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
            BDNode.Save(dataContext, af5);

            BDNode af6 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial);
            af6.SetParent(c3);
            af6.Name = "POSACONAZOLE PO";
            af6.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
            BDNode.Save(dataContext, af6);

            BDNode af7 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ff32e382-13b6-4b36-8b6e-f40734bdcc1e"));
            af7.SetParent(c3);
            BDNode.Save(dataContext, af7);

            // get remaining children of original section, change all to children of new category for antimicrobial agents (c2)
            List<BDNode> antimicrobials = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, antimicrobialSection.Uuid, BDConstants.BDNodeType.BDAntimicrobial);
            foreach (BDNode am in antimicrobials)
            {
                am.SetParent(c2);
                BDNode.Save(dataContext, am);
            }

            // create new topic in c1 for 'Organism Groups'
            BDNode o1 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic);
            o1.SetParent(c1);
            o1.Name = "Organism Groups";
            o1.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum;
            BDNode.Save(dataContext, o1);

            // create new subtopics for 'Organism Groups'
            BDNode st1 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st1.SetParent(o1);
            st1.Name = "Viridans Group Streptococci";
            st1.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st1);

            BDNode st2 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st2.SetParent(o1);
            st2.Name = "Enterobacteriaceae";
            st2.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st2);

            BDNode st3 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st3.SetParent(o1);
            st3.Name = "Coryneform bacteria";
            st3.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st3);

            BDNode st4 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st4.SetParent(o1);
            st4.Name = "Enterobacteriaceae that produce inducible cephalosporinase";
            st4.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st4);

            BDNode st5 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st5.SetParent(o1);
            st5.Name = "Staphylococci spp";
            st5.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st5);

            BDNode st6 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st6.SetParent(o1);
            st6.Name = "Streptococcus pneumoniae";
            st6.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st6);   */
            #endregion
            #region v.1.6.14
            /*
            // fix incorrect assignment of property 
            BDLinkedNoteAssociation lna = BDLinkedNoteAssociation.GetLinkedNoteAssociationWithId(dataContext, Guid.Parse("7cbbf37c-f7bd-4d75-b4a3-abe70a206c8f"));
            lna.parentKeyPropertyName = "Duration";
            BDLinkedNoteAssociation.Save(dataContext, lna);

            // Reset layout variant on Adults > SSTI > Cellulitis > Extremities
            BDNode ext = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("d8e71b5c-2456-40b9-8886-d202d436f314"));
            ext.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities;
            BDNode.Save(dataContext, ext);

            // Reset layout variant for Genital Ulcers nodes and children
            IBDNode startNode2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("96cbc7d0-c4ba-4593-a1d9-0e7908deeffd"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode2, BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers, true);

            // create new layer under Treatment Recommendations for Pneumonia CURB table sections
            BDNode respiratoryCategory = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("0b65f9e2-7436-4c22-9785-93be2b7e4f5a"));
            BDNode curbSubcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory);
            curbSubcategory.SetParent(respiratoryCategory);
            curbSubcategory.Name = "Table 3.  CURB-65 - Pneumonia Severity of Illness Scoring System";
            curbSubcategory.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia;
            BDNode partOne = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("9f5604d2-7c8c-4216-aba2-6a12fa3c58bd"));
            curbSubcategory.DisplayOrder = partOne.DisplayOrder;
            BDNode.Save(dataContext, curbSubcategory);

            partOne.SetParent(curbSubcategory);
            BDNode.Save(dataContext, partOne);

            BDNode partTwo = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("22437404-f46a-46c3-badb-dfe50aa6afd3"));
            partTwo.SetParent(curbSubcategory);
            BDNode.Save(dataContext, partTwo);

            // remove layer from hierarchy in CP Meningitis
            BDNode mTable = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("fc25fee8-8ded-4d41-895d-b415bab02eb7"));
            BDNode pGroup = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("d677df95-937a-41d9-b879-ef01af8ca99b"));
            List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(dataContext, pGroup);
            foreach (IBDNode pathogen in pathogens)
            {
                BDNode node = pathogen as BDNode;
                if (node != null)
                {
                    node.SetParent(mTable);
                    BDNode.Save(dataContext, node);
                }
            }
            BDNode.Delete(dataContext, pGroup, false);

            // remove layer from CP Pneumonia
            BDNode pTable = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("391bd1a4-daca-44e8-8fda-863f22128f1f"));
            BDNode pGroup2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a39027d0-758f-45db-bc5f-97f73b6081d0"));
            List<IBDNode> nodes = BDFabrik.GetChildrenForParent(dataContext, pGroup2);
            foreach (IBDNode pathogen in nodes)
            {
                BDNode node = pathogen as BDNode;
                if (node != null)
                {
                    node.SetParent(pTable);
                    BDNode.Save(dataContext, node);
                }
            }
            BDNode.Delete(dataContext, pGroup2, false); */
            #endregion
            #region v.1.6.16
            /*
            // Set new layoutvariant in table TR > Paediatrics > Repiratory
            BDNode xxtable = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("c77ee0f1-e2f8-4e18-82c5-b4ab0bc917cc"));
            List<IBDNode> pgroups = BDFabrik.GetChildrenForParent(dataContext, xxtable);
            foreach (IBDNode pathogenGroup in pgroups)
            {
                List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(dataContext, pathogenGroup);
                foreach (IBDNode pathogen in pathogens)
                {
                    List<IBDNode> pathogenResistances = BDFabrik.GetChildrenForParent(dataContext, pathogen);
                    foreach(IBDNode pathogenResistance in pathogenResistances)
                     {
                        List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(dataContext, pathogenResistance);
                        foreach (IBDNode therapyGroup in therapyGroups)
                        {
                            List<IBDNode> therapies = BDFabrik.GetChildrenForParent(dataContext, therapyGroup);
                            foreach (IBDNode therapy in therapies)
                            {
                                therapy.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics;
                                BDFabrik.SaveNode(dataContext, therapy);
                            }
                            therapyGroup.Name = string.Empty;
                            therapyGroup.SetParent(xxtable);
                            therapyGroup.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics;
                            BDFabrik.SaveNode(dataContext, therapyGroup);
                        }
                        BDFabrik.DeleteNode(dataContext, pathogenResistance);
                    }
                    BDFabrik.DeleteNode(dataContext, pathogen);
                }
                BDFabrik.DeleteNode(dataContext, pathogenGroup);
            }
            xxtable.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics;
            BDFabrik.SaveNode(dataContext, xxtable);

            // Delete the 'indicated' and 'not indicated' topics from Antibiotic Guidelines > Antimicrobials 
            BDNode category = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("1997c207-d754-4907-8e32-6722f7578641"));
            List<IBDNode> antimicrobials = BDFabrik.GetChildrenForParent(dataContext, category);
            foreach (IBDNode antimicrobial in antimicrobials)
            {
                List<IBDNode> topics = BDFabrik.GetChildrenForParent(dataContext, antimicrobial);
                foreach (IBDNode topic in topics)
                {
                    if (topic.Name == "Indicated" || topic.Name == "Not Indicated")
                        BDFabrik.DeleteNode(dataContext, topic);
                }
            }

            BDNode pdAdults = BDNode.RetrieveNodeWithId(dataContext,Guid.Parse("7366ae8d-03e9-4e3e-bf7d-8037e544deb4"));
            BDNode pdPeds = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("267febcb-f1da-431c-94df-1c612a6e7d3e"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, pdAdults, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult, true);
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, pdPeds, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric, true);

            // Reset layout variants for Antimicrobial generic/trade name listing
            BDNode table = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("13acf8a8-6f81-4ba1-a5ca-620dc978596b"));
            List<BDNode> tableSections = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, table.Uuid, BDConstants.BDNodeType.BDTableSection);
            foreach (BDNode section in tableSections)
            {
                section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_NameListing;
                BDFabrik.SaveNode(dataContext, section);
            }
            BDTableCell cell3 = BDTableCell.RetrieveWithId(dataContext, Guid.Parse("d829e699-13a2-49e9-a2a8-3701c0784920"));
            BDTableCell.Delete(dataContext, cell3, false);
            */

            #endregion
            #region v.1.6.18
            /*
            // add layout variants for subset of Antibiotics Dosing Guide and Daily costs:  split to peds and adults
            // needed for unique titles in output
            BDNode adultSubcategory = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("66a8dd87-98a7-4886-8d91-ab0d1e5457d8"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, adultSubcategory, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, true);
            BDNode pedsSubcategory = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a99f114d-4bce-4389-922c-4396d1894c14"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, pedsSubcategory, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, true);

            // create new layouts : Hepatic Impairment Grading table
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading, 0, "Score",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading, 1, "1",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, 0,"");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading, 2, "2",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading, 3, "3",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03, 0, "");

            // create layout for CSF Penetration / Intrathecal and/or Intraventricular
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages, 0, "Drug",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages, 1, "Doses",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages, 2, "Reported severe adverse effects",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, 0, "");
            
            // create footnote for Blood/Body Fluid Exposure
            BDUtilities.CreateFootnoteForLayoutColumn(dataContext, BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II, 3, "A non-responder is a person with inadequate response to vaccination (i.e anti-HBsAg <10IU/L)");

            // create layout for Antibiotics Dosing guide & daily costs - Peds
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, 0, "Antimicrobial",
                BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, 1, "Recommended Paediatric Dose",
                BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE, 0, @"mg/kg/d = milligrams per kilogram per <u>day</u>.  Usual doses for paediatric patients with normal renal and hepatic function.  Paediatric dose should not exceed recommended adult dose (except for cefuroxime where maximum is 1.5g IV q8h).  For disease-specific dosing see Recommended Empiric Therapy in Neonatal/Paediatric Patients.");
            BDUtilities.CreateFootnoteForLayoutColumn(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, 1, "These doses do not apply to neonates, except where noted.");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, 2, "Cost ($)/Day",
                BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_COST, 0, "Based on a 20kg child.");
            BDUtilities.CreateFootnoteForLayoutColumn(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, 2, "Based on Alberta Health Drug Benefit List price, January 2006, or manufacturer's list price if drug not on AH DBL.  Prices in the hospital setting may be significantly different due to contract pricing.  Check with pharmacy for actual prices.  Does not include preparation, administration, supplies or serum level costs.");
            */
            #endregion
            #region v.1.6.20
            /*
            // adjust layout variant for Dosing Guide and Daily Costs - Paediatric
            BDNode category = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a99f114d-4bce-4389-922c-4396d1894c14"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, category, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, true);
            */
            #endregion
            #region v.1.6.22
            /*
            // adjust layout variant for Dosing Guide and Daily Costs - Adult
            BDNode aCategory = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("66a8dd87-98a7-4886-8d91-ab0d1e5457d8"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, aCategory, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, true);

            // set value to 'Dosage'
            IQueryable<BDLinkedNoteAssociation> linkedNoteAssociations = (from bdLinkedNoteAssociations in dataContext.BDLinkedNoteAssociations
                                                                          where bdLinkedNoteAssociations.parentType == 18
                                                                          orderby bdLinkedNoteAssociations.displayOrder
                                                                          select bdLinkedNoteAssociations);

            List<BDLinkedNoteAssociation> resultList = linkedNoteAssociations.ToList<BDLinkedNoteAssociation>();
            foreach (BDLinkedNoteAssociation assn in resultList)
                assn.parentKeyPropertyName = BDDosage.PROPERTYNAME_DOSAGE;

            dataContext.SaveChanges();
            */
            #endregion
            #region v.1.6.23
            /*
            // create layout for Antibiotics Dosing guide & daily costs - Adults
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, 0, "Antimicrobial",
                BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, 1, "Normal Adult Dose",
                BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE, 0, @"Based on a 70kg adult with normal renal and hepatic function.  For disease-specific dosing, see also Recommended Empiric Therapy in Adult Patients and/or Recommended Therapy of Culture-Directed Infections in Adult Patients.");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, 2, "Cost ($)/Day",
                BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_COST, 0, "Based on Alberta Health Drug Benefit List (AH DBL) price, September 2012, or manufacturer's list price or wholesale price if drug not on AH DBL.  Prices in the hospital setting may be significantly different due to contract pricing.  Check with pharmacy for actual prices.  Does not include administration, supplies, or serum level costs.");
            */
            #endregion
            #region v.1.6.24
            /*
            // fix child layout variants for vancomycin section
            BDNode vancSection = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("c42a29c2-f5a1-48a4-b140-3e4dae56b445"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, vancSection, BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin, true);

            // create layout metadata for Dental Prophylaxis
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Dental_Prophylaxis, 0, "Antimicrobial", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Dental_Prophylaxis, 1, "Adult Dose", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Dental_Prophylaxis, 2, "Paediatric Dose", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, 0, "");
            */
            #endregion
            #region v.1.6.26
            /*
            // layout metadata for Renal impairment
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment, 0, "Antimicrobial", BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment, 1, "Normal Adult<br>Dose", BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment, 2, "Creatinine Clearance (mL/min)", BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE2, 0, ">50mL/min = >0.83mL/s; 10-50mL/mon = 0.17-0.83mL/s; <10mL/min = <0.17mL/s");
            // reset layout variant for Aminoglycoside conventional dosing and monitoring to accommodate adult/peds category in hierarchy
            BDNode section = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ca7eefb1-81f8-465e-8818-34a35d92736a"));
            List<IBDNode> sectionChildren = BDFabrik.GetChildrenForParent(dataContext, section);

            BDNode categoryA = BDFabrik.CreateNode(dataContext, BDConstants.BDNodeType.BDCategory, section.Uuid, BDConstants.BDNodeType.BDSection) as BDNode;
            categoryA.DisplayOrder = 0;
            categoryA.Name = "ADULTS";
            categoryA.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional;
            BDNode.Save(dataContext, categoryA);

            BDNode categoryP = BDFabrik.CreateNode(dataContext, BDConstants.BDNodeType.BDCategory, section.Uuid, BDConstants.BDNodeType.BDSection) as BDNode;
            categoryP.DisplayOrder = 1;
            categoryP.Name = "PAEDIATRICS";
            categoryP.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional;
            BDNode.Save(dataContext, categoryP);

            foreach (IBDNode child in sectionChildren)
                child.SetParent(categoryA);
            dataContext.SaveChanges();

            BDUtilities.ResetLayoutVariantWithChildren(dataContext, section, BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional, true);
            dataContext.SaveChanges();
            */
            #endregion
            #region v.1.6.27
            /*
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation, 0, "Bacterial Morphology", BDConstants.BDNodeType.BDSubcategory, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation, 1, "Probable Organisms", BDConstants.BDNodeType.BDMicroorganism, BDNode.PROPERTYNAME_NAME, 0, "");

            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 0, "DRUG", BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 1, "RISK CATEGORY", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_LACTATIONRISK, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 2, "AAP RATING", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 3, "RELATIVE INFANT DOSE", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE, 0, "");

            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy, 0, "DRUG", BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy, 1, "FDA RISK CATEGORY", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy, 2, "RECOMMENDATION", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, 0, "");
            
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection, 0, "Antimicrobial Regimen", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection, 1, "Dose & Duration", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, 0, "");

            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage, 0, "SITUATION", BDConstants.BDNodeType.BDTherapyGroup, BDTherapyGroup.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage, 1, "DRUG", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage, 2, "ADULT DOSE", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage, 3, "PAEDIATRIC DOSE", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, 0, "");

            BDNode ieProphylaxis = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("376b287e-1d80-40f5-bb0b-512e52720687"));
            ieProphylaxis.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_IERecommendation;
            BDNode.Save(dataContext, ieProphylaxis);
            
            BDNode intro = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.NewGuid());
            intro.Name = "FrontMatter";
            intro.DisplayOrder = 0;
            intro.parentId = Guid.Empty;
            intro.parentKeyName = BDUtilities.GetEnumDescription(BDConstants.BDNodeType.None);
            intro.LayoutVariant = BDConstants.LayoutVariantType.FrontMatter;
            BDNode antibiotics = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("45e13826-aedb-48d0-baf6-2f06ff45017f"));
            antibiotics.DisplayOrder = 1;
            BDNode treatmentRecommendations = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("f92fec3a-379d-41ef-a981-5ddf9c9a9f0e"));
            treatmentRecommendations.DisplayOrder = 2;
            BDNode prophylaxis = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("76e2f45c-c8c5-45e4-a079-65e1a3908cde"));
            prophylaxis.DisplayOrder = 3;
            BDNode dental = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("cddcc760-d8f2-460f-8982-668f98b5404b"));
            dental.DisplayOrder = 4;
            BDNode pregnancyLactation = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("690c044e-72c4-4115-b90e-33c9807dfe50"));
            pregnancyLactation.DisplayOrder = 5;
            BDNode organisms = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("fc322f9f-7204-42a7-9e6a-322a0869e6aa"));
            organisms.DisplayOrder = 6;
            organisms.Name = "Organisms";
            dataContext.SaveChanges();

            BDNode preface = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.NewGuid());
            preface.DisplayOrder = 0;
            preface.Name = "Preface";
            preface.LayoutVariant = BDConstants.LayoutVariantType.FrontMatter;
            preface.SetParent(intro);

            BDNode foreward = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.NewGuid());
            foreward.SetParent(intro);
            foreward.DisplayOrder = 1;
            foreward.LayoutVariant = BDConstants.LayoutVariantType.FrontMatter;
            foreward.Name = "Foreward";
            dataContext.SaveChanges();
          
            // change parent of BDAttachment in Fungal infections
            BDNode section = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("40d92304-3224-4af0-8371-bcc27edad7dd"));
            BDNode topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.NewGuid());
            topic.Name = "Management of Adult Patients on Amphotericin B";
            topic.DisplayOrder = 12;
            topic.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal;
            topic.SetParent(section);

            BDAttachment att1 = BDAttachment.RetrieveWithId(dataContext, Guid.Parse("a9f389cf-f134-4513-bba4-9896c5e17356"));
            att1.SetParent(topic);
            att1.DisplayOrder = 0;

            // for DEBUG only
            //BDAttachment att2 = BDAttachment.RetrieveWithId(dataContext, Guid.Parse("8875a8cf-6b7b-490b-9158-2a7948f21363"));
            //if (att2 != null)
            //{
            //    att2.SetParent(topic);
            //    att2.DisplayOrder = 1;
            //}
            dataContext.SaveChanges();
             */
            #endregion
            #region v.1.6.28
            /* BDNode sAssault = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a6fdd2d3-96df-47b6-a598-8adbfa474c1b"));
            BDNode bbFluidExposure = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ec9b96ff-8744-4e27-82f6-9608fe22e4b4"));
            sAssault.SetParent(bbFluidExposure);
            sAssault.nodeType = (int)BDConstants.BDNodeType.BDCategory;
            BDNode.Save(dataContext, sAssault);
            */
            #endregion
            #region v.1.6.29
            /*
            // move Organism groups up in the hierarchy
            BDNode legend = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("d013a04e-a653-4509-9ab3-907effef5ee6"));
            BDNode legendParent = BDNode.RetrieveNodeWithId(dataContext, legend.parentId.Value);
            List<IBDNode> children = BDFabrik.GetChildrenForParent(dataContext, legend);
            foreach (IBDNode child in children)
                child.SetParent(legendParent);
            dataContext.SaveChanges();
            
            BDNode antibioticChapter = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("45e13826-aedb-48d0-baf6-2f06ff45017f"));
            BDNode admg = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.NewGuid());
            admg.Name = "Aminoglycoside Dosing and Monitoring Guidelines";
            admg.DisplayOrder = 3;
            admg.SetParent(antibioticChapter);
            admg.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring;

            BDNode highDose = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("147410a3-ca09-4b4a-af50-c5f8719f1729"));
            highDose.nodeType = (int)BDConstants.BDNodeType.BDSubsection;
            highDose.nodeKeyName = BDUtilities.GetEnumDescription(BDConstants.BDNodeType.BDSubsection);
            highDose.SetParent(BDConstants.BDNodeType.BDSection, admg.Uuid);
            highDose.DisplayOrder = 0;

            BDNode conventionalDose = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ca7eefb1-81f8-465e-8818-34a35d92736a"));
            conventionalDose.nodeType = (int)BDConstants.BDNodeType.BDSubsection;
            conventionalDose.nodeKeyName = BDUtilities.GetEnumDescription(BDConstants.BDNodeType.BDSubsection);
            conventionalDose.SetParent(BDConstants.BDNodeType.BDSection, admg.Uuid);
            conventionalDose.DisplayOrder = 1;
            dataContext.SaveChanges();

            BDNode newParent = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.NewGuid());
            newParent.Name = "Monitoring";
            newParent.SetParent(conventionalDose);
            newParent.LayoutVariant = conventionalDose.LayoutVariant;

            BDNode monitoring = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("54aee8b1-9c6f-4f08-9491-622347f222fc"));
            monitoring.SetParent(newParent);
            BDNode serum = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("785e754c-6fb6-43ca-9cbd-b871ba2dc8ba"));
            serum.SetParent(newParent);
            BDNode frequency = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("9293d8bb-a55e-41fa-a941-7b1376715763"));
            frequency.SetParent(newParent);
            dataContext.SaveChanges();
            */
            #endregion
            #region v.1.6.33
            //BDNode backMatter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.NewGuid());
            //backMatter.Name = "Back Matter";
            //backMatter.DisplayOrder = 7;
            //backMatter.parentId = Guid.Empty;
            //backMatter.parentKeyName = BDUtilities.GetEnumDescription(BDConstants.BDNodeType.None);
            //backMatter.LayoutVariant = BDConstants.LayoutVariantType.BackMatter;

            //BDNode afterword = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.NewGuid());
            //afterword.Name = "Afterword";
            //afterword.DisplayOrder = 0;
            //afterword.SetParent(backMatter);
            //afterword.LayoutVariant = BDConstants.LayoutVariantType.BackMatter;

            //BDNode frontMatter = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("5fe7fc06-5d76-438b-ac5b-0c3c483ea871"));
            //frontMatter.name = "Front Matter";

            //dataContext.SaveChanges();

            #endregion
            #region v.1.6.34
            //BDNode table = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("f7998d6e-78d9-49b2-aea5-d7396be0a877"));
            //BDNode odontogenic = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a6874e73-7f2c-4f29-8e24-d0d2bb57b8a6"));
            //odontogenic.nodeType = (int)BDConstants.BDNodeType.BDSubcategory;
            //odontogenic.nodeKeyName = "BDSubcategory";
            //odontogenic.SetParent(table);

            //BDNode peri = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("e1bb6301-0ab0-47ae-a15d-6607897aac84"));
            //peri.nodeType = (int)BDConstants.BDNodeType.BDSubcategory;
            //peri.nodeKeyName = "BDSubcategory";
            //peri.SetParent(table);

            //BDNode oral = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("030842eb-f3cd-4679-ae60-deac35161bca"));
            //oral.nodeType = (int)BDConstants.BDNodeType.BDSubcategory;
            //oral.nodeKeyName = "BDSubcategory";
            //oral.SetParent(table);

            //dataContext.SaveChanges();
            #endregion
            #region v.1.6.35
            //#region Part One
            //// move child nodes from Otitis Media to Otiis Externa
            //BDNode externa = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("7fda2fdf-f8cd-4e87-ad1c-a377f196bd6c"));
            //BDNode child1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("da944325-f078-4d4f-8608-136d457c8fd8"));
            //child1.SetParent(externa);
            //child1.DisplayOrder = 0;
            //BDNode child2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("265c7303-3c3d-4e16-a120-3cade8343fb3"));
            //child2.SetParent(externa);
            //child2.DisplayOrder = 1;
            //dataContext.SaveChanges();

            //// Move Human Bites up to Disease level and out of Animal Bites
            //BDNode parentNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("8d188a42-7ac0-453c-95cd-f753a9c9d81a"));
            //BDNode hbites = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDDisease, Guid.NewGuid());
            //hbites.Name = "Human Bites";
            //hbites.DisplayOrder = 9;
            //hbites.SetParent(parentNode);
            //hbites.layoutVariant = (int)parentNode.LayoutVariant;

            //BDNode presentation = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("0845c606-5018-4711-b413-0053e415da50"));
            //presentation.Name = "SINGLE PRESENTATION";
            //presentation.SetParent(hbites);
            //presentation.DisplayOrder = 0;
            //dataContext.SaveChanges();

            //// Move presentaions out of CAP into Hospitalized
            //BDNode hospitalized = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("026f2db5-a22c-434b-927b-0fadd648c19f"));
            //BDNode moderate = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ef43b894-e820-4499-9466-83c5a15845ae"));
            //BDNode severe = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("c085d1e6-31eb-4704-b11b-7379d2751804"));
            //moderate.DisplayOrder = 0;
            //moderate.SetParent(hospitalized);
            //severe.DisplayOrder = 1;
            //severe.SetParent(hospitalized);
            //dataContext.SaveChanges();

            //// change node from presentation to disease, move (sibling) nodes under 
            //BDNode respiratory = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("0b65f9e2-7436-4c22-9785-93be2b7e4f5a"));
            ////AECB
            //BDNode newDisease = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("cb141fa1-9769-49a9-86fe-793c6c259405"));
            //newDisease.nodeType = (int)BDConstants.BDNodeType.BDDisease;
            //newDisease.nodeKeyName = "BDDisease";
            //newDisease.DisplayOrder = 23;
            //newDisease.SetParent(respiratory);
            //BDNode d1c1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("477f6ba2-d2b9-4b80-af7f-3e9ecc0da21b"));
            //d1c1.SetParent(newDisease);
            //d1c1.DisplayOrder = 0;
            //BDNode d1c2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("f9a7701a-0ffe-46bf-9f67-676110c0d5e1"));
            //d1c2.SetParent(newDisease);
            //d1c2.DisplayOrder = 1;
            ////Bronchiectasis
            //BDNode d2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("63a768cd-397d-491b-9978-97e38c054acf"));
            //d2.nodeType = (int)BDConstants.BDNodeType.BDDisease;
            //d2.nodeKeyName = "BDDisease";
            //d2.DisplayOrder = 24;
            //d2.SetParent(respiratory);
            //BDNode d2c1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a3b15507-15af-4276-ba06-76c996c822fc"));
            //d2c1.SetParent(d2);
            //d2.DisplayOrder = 0;
            //dataContext.SaveChanges();
            //#endregion

            //#region Part Two
            //// remove pathogen group from hierarchy in Culture-proven Endocarditis
            //BDNode parent = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("8741d365-16ef-4de8-8d08-dabc574c010a"));
            //BDNode pathogenGroup = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("251bbd0f-2d96-433c-acfa-ac21553859f2"));
            //List<BDNode> pathogens = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, pathogenGroup.Uuid, BDConstants.BDNodeType.BDPathogen);
            ////List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(dataContext, pathogenGroup);
            //foreach (BDNode pathogen in pathogens)
            //    pathogen.SetParent(parent);
            //dataContext.SaveChanges();

            //// Remove first column from CURB table
            //BDTableRow  header = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse("ab6fb715-929b-4d47-acad-41a202b1708b"));
            //List<BDTableCell> headerCells = BDTableCell.RetrieveTableCellsForParentId(dataContext, header.uuid);
            //BDTableCell cellToDelete = null;
            //for(int i = 0; i < headerCells.Count; i++)
            //{
            //    BDTableCell hCell = headerCells[i];
            //    if (i == 0)
            //        cellToDelete = hCell;
            //    else
            //        hCell.DisplayOrder = i - 1;
            //}
            //if (cellToDelete != null)
            //    BDTableCell.Delete(dataContext, cellToDelete, false);

            //BDNode tSection = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("de6c6855-f290-471b-a884-accb87cf24e6"));
            //List<BDTableRow> contentRows = BDTableRow.RetrieveTableRowsForParentId(dataContext, tSection.Uuid);
            //foreach(BDTableRow cRow in contentRows)
            //{
            //    List<BDTableCell> cCells = BDTableCell.RetrieveTableCellsForParentId(dataContext, cRow.uuid);
            //    BDTableCell contentCellToDelete = null;
            //    for(int i = 0; i < cCells.Count; i++)
            //    {
            //        BDTableCell cCell = cCells[i];
            //        if(i == 0)
            //            contentCellToDelete = cCell;
            //        else
            //            cCell.DisplayOrder = i - 1;
            //    }
            //    if (cellToDelete != null)
            //        BDTableCell.Delete(dataContext, contentCellToDelete, false);
            //}
            //dataContext.SaveChanges();
            //#endregion
            #endregion
            #region v.1.5.36
            // move sexual assault up to Section under Chapter
            //BDNode sAssault = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a6fdd2d3-96df-47b6-a598-8adbfa474c1b"));
            //sAssault.nodeType = (int)BDConstants.BDNodeType.BDSection;
            //sAssault.nodeKeyName = "BDSection";
            //BDNode chapter = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("76e2f45c-c8c5-45e4-a079-65e1a3908cde"));
            //sAssault.SetParent(chapter);
            //dataContext.SaveChanges();

            // update pathogens in treatment recommendations > culture directed > endocarditis with new layout variant
            //BDNode p1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("df678b4a-2df9-436e-b8ee-729775c575b7"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, p1, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration, true);
            //BDNode p2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("eda0074a-8a49-4b17-a4e0-268622d7ec08"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, p2, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration, true);
            //BDNode p3 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("005ce2ad-c080-41f0-9d4b-0133d98d6325"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, p3, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration, true);
            //BDNode p4 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("b05d29ad-ef60-40a1-bc4c-c094c6aeb8ba"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, p4, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration, true);
            //BDNode p5 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ca8fce91-4008-4607-9bc8-ecdb3a1eedfb"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, p5, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration, true);
            //BDNode p6 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("e93196e1-f9a4-4acf-a426-06ecb543bf46"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, p6, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration, true);
            //BDNode p7 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a090e43d-cab0-410e-a695-d3f0a448a18f"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, p7, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration, true);
            //BDNode p8 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("839b5b46-84bc-4720-ace3-46bff17cbf70"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, p8, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration, true);
            //BDNode p9 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("b569ad10-a31f-450e-8dc8-f9f8df83ae6d"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, p9, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration, true);
            //dataContext.SaveChanges();

            // update structure for genital warts
            // change Genital Warts to a Topic, reassign layout variant
            //BDNode gw = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("305e98f5-cbe2-4e7b-ba1f-6c06d04c065a"));
            //gw.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic;
            //BDNode gwChild = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("6794bbba-54d4-4b5e-9e91-6ef6788c7cad"));
            //gwChild.SetParent(gw);
            //gwChild.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //gwChild.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //dataContext.SaveChanges();
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, gwChild, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, true);
            //dataContext.SaveChanges();

            // change Syphilis layout variant
            //BDNode sy = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("9fab60cf-dccc-45f7-84cb-a2ac156ed30e"));
            //sy.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic;
            //BDNode syChild1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("06682028-0f1f-4b5d-87df-ff8ed24afa20"));
            //BDNode syChild2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("e13bc857-aeea-4b54-af38-cb27977b6062"));
            //BDNode syChild3 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("522c06ef-91f5-44a3-9736-1552bc3fbdc6"));
            //syChild1.SetParent(sy);
            //syChild1.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //syChild1.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //syChild2.SetParent(sy);
            //syChild2.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //syChild2.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //syChild3.SetParent(sy);
            //syChild3.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //syChild3.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, syChild1, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, true);
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, syChild2, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, true);
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, syChild3, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, true);

            // change Herpes layout variant
            //BDNode hp = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ac4e5aae-4ccc-4c35-898e-40328cdd3146"));
            //hp.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic;
            // pregnancy
            //BDNode hpChild1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ef7d314a-301d-4389-bfc7-c945412ef598"));
            //hpChild1.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //hpChild1.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //hpChild1.SetParent(hp);
            //hpChild1.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic;
            // primary
            //BDNode hpChild2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("18dd3ae5-5374-4c03-b334-5efeabc4d5a1"));
            //hpChild2.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //hpChild2.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //hpChild2.SetParent(hp);
            // recurrent
            //BDNode hpChild3 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("0c8311d6-cb82-41e5-b040-8ca58706d800"));
            //hpChild3.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //hpChild3.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //hpChild3.SetParent(hp);
            // recurrent immunocompromised
            //BDNode hpChild4 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("3ca492df-3782-474c-bf99-f19053a79518"));
            //hpChild4.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //hpChild4.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //hpChild4.SetParent(hp);
            //dataContext.SaveChanges();

            //BDNode hpg_Child1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("40f1e9dd-2833-40f2-8d45-0b8c0b93f8ab"));
            //hpg_Child1.SetParent(hpChild1);
            //hpg_Child1.nodeType = (int)BDConstants.BDNodeType.BDSubtopic;
            //hpg_Child1.nodeKeyName = BDConstants.BDNodeType.BDSubtopic.ToString();
            //BDNode hpg_Child2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("b611e280-1617-4ed8-88c8-aab60d534fba"));
            //hpg_Child2.SetParent(hpChild1);
            //hpg_Child2.nodeType = (int)BDConstants.BDNodeType.BDSubtopic;
            //hpg_Child2.nodeKeyName = BDConstants.BDNodeType.BDSubtopic.ToString();
            //dataContext.SaveChanges();

            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, hpChild2, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, true);
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, hpChild3, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, true);
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, hpChild4, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, true);

            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, hpg_Child1, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic, true);
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, hpg_Child2, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic, true);
            //dataContext.SaveChanges();

            // change layout variant and children of Vulvovaginitis
            //BDNode vv = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("32ca6e75-3180-4706-a3a8-6835cdb9a0d3"));
            //BDNode candidiasis = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPresentation, Guid.NewGuid());
            //candidiasis.SetParent(vv);
            //candidiasis.name = "Candidiasis";
            //candidiasis.displayOrder = 2;
            //candidiasis.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic;
            //dataContext.SaveChanges();

            //BDNode vvC1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a5510470-d077-4ad7-ac49-d23d9fe7e777"));
            //vvC1.SetParent(candidiasis);
            //vvC1.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //vvC1.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //dataContext.SaveChanges();
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, vvC1, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, true);

            //BDNode vvC2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("058fd5c0-ad16-46c3-ba22-fc57963d1341"));
            //vvC2.SetParent(candidiasis);
            //vvC2.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //vvC2.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //dataContext.SaveChanges();
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, vvC2, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, true);

            //BDNode vvC3 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.NewGuid());
            //vvC3.name = "Candidiasis - symptomatic, complicated";
            //vvC3.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //vvC3.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //vvC3.DisplayOrder = 0;
            //vvC3.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic;
            //vvC3.SetParent(candidiasis);
            //dataContext.SaveChanges();

            //BDNode vvC3C1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("78c88fab-2027-4937-8405-ba29104b34eb"));
            //vvC3C1.SetParent(vvC3);
            //vvC3C1.nodeType = (int)BDConstants.BDNodeType.BDSubtopic;
            //vvC3C1.nodeKeyName = BDConstants.BDNodeType.BDSubtopic.ToString();
            //BDNode vvC3C2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("f576d9d1-1b8b-43f4-80d0-8dd9f8520210"));
            //vvC3C2.SetParent(vvC3);
            //vvC3C2.nodeType = (int)BDConstants.BDNodeType.BDSubtopic;
            //vvC3C2.nodeKeyName = BDConstants.BDNodeType.BDSubtopic.ToString();
            //BDNode vvC3C3 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("c4b4b893-113c-4d93-81be-b2d2223dd7ba"));
            //vvC3C3.SetParent(vvC3);
            //vvC3C3.nodeType = (int)BDConstants.BDNodeType.BDSubtopic;
            //vvC3C3.nodeKeyName = BDConstants.BDNodeType.BDSubtopic.ToString();
            //BDNode vvC3C4 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("3963e550-4524-4224-8885-99fe28bc4fb9"));
            //vvC3C4.SetParent(vvC3);
            //vvC3C4.nodeType = (int)BDConstants.BDNodeType.BDSubtopic;
            //vvC3C4.nodeKeyName = BDConstants.BDNodeType.BDSubtopic.ToString();
            //BDNode vvC3C5 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a1640930-83c7-4e2c-a444-e6cdc5d638eb"));
            //vvC3C5.SetParent(vvC3);
            //vvC3C5.nodeType = (int)BDConstants.BDNodeType.BDSubtopic;
            //vvC3C5.nodeKeyName = BDConstants.BDNodeType.BDSubtopic.ToString();
            //dataContext.SaveChanges();

            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, vvC3C1, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic, true);
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, vvC3C2, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic, true);
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, vvC3C3, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic, true);
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, vvC3C4, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic, true);
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, vvC3C5, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic, true);
            //dataContext.SaveChanges();
            #endregion
            #region v.1.6.37
            //// retrieve Adults > Recurrent Cystitis (Presentation); add 2 new Topics & move children under as requested. Reset them to Subtopic
            //BDNode r_cystitis = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("d427b03f-8fe2-45a6-ba9f-9d584aaf854d"));
            //r_cystitis.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic;
            //BDNode c1 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.NewGuid());
            //BDNode c2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("369708ef-4b19-4c43-8f11-19d12ec1be15"));
            //BDNode c1g1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("3e9cc4be-daed-4b6d-bdb4-bee39b7185dd"));
            //BDNode c1g2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("b95c9bdd-55a2-4380-914a-60c16a5a3a66"));
            //BDNode c2g1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("2d05e6fd-cc53-48c8-b78e-d103083a60f6"));
            //BDNode c2g2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("e104104c-d2e4-42f6-a48d-64e94fa72837"));

            //c1.name = "Females Sexually active";
            //c1.DisplayOrder = 0;
            //c1.SetParent(r_cystitis);
            //c1.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic;

            //c2.DisplayOrder = 1;
            //c2.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //c2.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //c2.SetParent(r_cystitis);
            //c2.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic;

            //dataContext.SaveChanges();

            //c1g1.SetParent(c1);
            //c1g1.DisplayOrder = 0;
            //c1g1.nodeType = (int)BDConstants.BDNodeType.BDSubtopic;
            //c1g1.nodeKeyName = BDConstants.BDNodeType.BDSubtopic.ToString();

            //c1g2.SetParent(c1);
            //c1g2.DisplayOrder = 0;
            //c1g2.nodeType = (int)BDConstants.BDNodeType.BDSubtopic;
            //c1g2.nodeKeyName = BDConstants.BDNodeType.BDSubtopic.ToString();

            //c2g1.SetParent(c2);
            //c2g1.DisplayOrder = 0;
            //c2g1.nodeType = (int)BDConstants.BDNodeType.BDSubtopic;
            //c2g1.nodeKeyName = BDConstants.BDNodeType.BDSubtopic.ToString();

            //c2g2.SetParent(c2);
            //c2g2.DisplayOrder = 1;
            //c2g2.nodeType = (int)BDConstants.BDNodeType.BDSubtopic;
            //c2g2.nodeKeyName = BDConstants.BDNodeType.BDSubtopic.ToString();

            //dataContext.SaveChanges();

            //// Part Deux
            //// retrieve Adults > Respiratory > HAP and children. Create New topics, move children under as requested.
            //BDNode hap = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("49c1480c-78a0-4ce5-ac1a-daf74757d251"));
            //BDNode h1 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPresentation, Guid.NewGuid());
            //BDNode h2 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPresentation, Guid.NewGuid());
            //BDNode h1c1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("d2fa1506-331a-424a-b937-321e82382ab1"));
            //BDNode h2c1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("9a2562f5-96ae-414b-9ee9-b6b6487a427b"));
            //BDNode h2c2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ed212ed9-23d5-40ac-ab06-2b7e89dddde2"));
            //BDNode h2c3 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("28fcacef-e218-411b-9087-dae8d41547fb"));
            //BDNode h2c4 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("6f3e8da6-b041-417b-9e34-0636d123a85a"));

            //h1.name = "Early onset";
            //h1.DisplayOrder = 0;
            //h1.SetParent(hap);
            //h1.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic;

            //h2.name = "Late onset";
            //h2.DisplayOrder = 1;
            //h2.SetParent(hap);
            //h2.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic;

            //h1c1.SetParent(h1);
            //h1c1.DisplayOrder = 0;
            //h1c1.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //h1c1.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //h2c1.SetParent(h2);
            //h2c1.DisplayOrder = 0;
            //h2c1.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //h2c1.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //h2c2.SetParent(h2);
            //h2c2.DisplayOrder = 1;
            //h2c2.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //h2c2.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //h2c3.SetParent(h2);
            //h2c3.DisplayOrder = 2;
            //h2c3.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //h2c3.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //h2c4.SetParent(h2);
            //h2c4.DisplayOrder = 3;
            //h2c4.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //h2c4.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //dataContext.SaveChanges();

            //// Change layout variant for all
            //BDNode r_cystitis = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("d427b03f-8fe2-45a6-ba9f-9d584aaf854d"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, r_cystitis, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic, true);
            //dataContext.SaveChanges();

            //BDNode hap = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("49c1480c-78a0-4ce5-ac1a-daf74757d251"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, hap, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, true);
            //dataContext.SaveChanges();
            #endregion
            #region v.1.6.38
            //BDNode cardio = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("9a80eb9b-6438-48bd-901c-e258f29f4bf3"));
            //BDNode deviceRelated = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDDisease, Guid.NewGuid());
            //BDNode endocarditis = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("c0cf6533-c4c9-480b-ad85-c7d187672849"));
            //BDNode dr2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("3e59213d-8ba4-4a83-acda-edb66eb0a1fb"));
            //BDNode pValve = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPresentation, Guid.NewGuid());
            //BDNode pvc1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("12c6c370-b63b-4b3c-9dc1-5cb9fa988918"));
            //BDNode pvc2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("cbf0e055-db66-449f-9f49-0bdb0473f942"));

            //deviceRelated.Name = @"Cardiac device-related infections/endocarditis (CDRIE)";
            //deviceRelated.SetParent(cardio);
            //deviceRelated.DisplayOrder = 0;
            //deviceRelated.LayoutVariant = cardio.LayoutVariant;

            //dr2.SetParent(deviceRelated);

            //dataContext.SaveChanges();

            //pValve.Name = "Prosthetic Valve";
            //pValve.SetParent(endocarditis);
            //pValve.DisplayOrder = 2;
            //pValve.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01;
            //dataContext.SaveChanges();

            //pvc1.SetParent(pValve);
            //pvc2.SetParent(pValve);
            //pvc1.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //pvc1.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //pvc2.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //pvc2.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();

            //dataContext.SaveChanges();
            //dataContext.Refresh(System.Data.Objects.RefreshMode.ClientWins, pValve);

            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, pValve, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, true);

            //dataContext.SaveChanges();
            #endregion
            #region v.1.6.39
            // fix layout variant for Antibiotics > BLactam > cross-reactivity table
            //BDNode crossReactivityTable = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("2ff81d9e-1f69-455f-846e-16a9b098ad50"));
            //List<IBDNode> tablesections = BDFabrik.GetChildrenForParent(pContext, crossReactivityTable);
            //foreach (IBDNode section in tablesections)
            //{
            //    List<IBDNode> tableRows = BDFabrik.GetChildrenForParent(pContext, section);
            //    foreach (IBDNode row in tableRows)
            //        BDUtilities.ResetLayoutVariantWithChildren(pContext, row, BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity_ContentRow, true);

            //    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity;
            //    BDNode.Save(pContext, section as BDNode);
            //}
            //crossReactivityTable.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity;
            //BDNode.Save(pContext, crossReactivityTable);


            //// reassign layout variant in TreatmentRecommendations > Culture-Directed > Endocarditis > Viridans Group,,,
            //BDNode pathogen = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("24dee453-d880-4d8e-b869-ca89dbe13067"));
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, pathogen, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep, true);
            //pContext.SaveChanges();
            #endregion
            #region v.1.6.41
            //BDNode nodeToMove = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("9904de9f-dc28-4ab3-a5e6-2a3bc9809938"));
            //BDNode parentNode = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("5bc35b60-135e-4d7d-89d3-b0e45d6ce9bf"));
            //nodeToMove.SetParent(parentNode);
            //nodeToMove.nodeType = (int)BDConstants.BDNodeType.BDCategory;
            //nodeToMove.nodeKeyName = "BDCategory";
            //nodeToMove.DisplayOrder = 2;
            //BDNode.Save(pContext, nodeToMove);

            //BDNode newAntimatterNode = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDChapter, Guid.Parse("a6d03c7e-a095-4c04-b0e7-ffe74bcfa8e6"));
            //newAntimatterNode.LayoutVariant = BDConstants.LayoutVariantType.FrontMatter;
            //newAntimatterNode.name = "Publication Notes";
            //newAntimatterNode.nodeKeyName = "BDChapter";
            //newAntimatterNode.parentId = Guid.Empty;
            //newAntimatterNode.DisplayOrder = 0;
            //newAntimatterNode.parentKeyName = string.Empty;
            //newAntimatterNode.LayoutVariant = BDConstants.LayoutVariantType.FrontMatter;
            //pContext.SaveChanges();

            //BDNode newAntimatterNode = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("a6d03c7e-a095-4c04-b0e7-ffe74bcfa8e6"));
            //BDNode preface = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("1400e49c-8e18-4571-aba5-b792ab9332f7"));
            //preface.nodeType = (int)BDConstants.BDNodeType.BDSection;
            //preface.nodeKeyName = BDConstants.BDNodeType.BDSection.ToString();
            //preface.SetParent(newAntimatterNode);

            //BDNode foreword = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("e7ee1b98-5c2d-46ab-ada2-9599baaa5260"));
            //foreword.nodeType = (int)BDConstants.BDNodeType.BDSection;
            //foreword.nodeKeyName = BDConstants.BDNodeType.BDSection.ToString();
            //foreword.SetParent(newAntimatterNode);

            //BDNode afterword = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("9fa45212-0b08-452d-8443-0d4ec86792f7"));
            //afterword.nodeType = (int)BDConstants.BDNodeType.BDSection;
            //afterword.nodeKeyName = BDConstants.BDNodeType.BDSection.ToString();
            //afterword.SetParent(newAntimatterNode);
            //afterword.DisplayOrder = 2;
            //pContext.SaveChanges();

            //// delete old 'frontmatter' and 'backmatter' sections
            //BDNode.DeleteLocal(pContext, Guid.Parse("5fe7fc06-5d76-438b-ac5b-0c3c483ea871"));
            //BDNode.DeleteLocal(pContext, Guid.Parse("2cbb9208-5003-4155-a24e-dc4a86026d00"));
            //pContext.SaveChanges();
            #endregion
            #region v.1.6.42
            // Part 1:  reorganize 'Sepsis without a focus' in TR > Peds.
            //BDNode parentNode = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("9e263b5f-27e7-4a51-ab2f-34eb157ed273"));
            //BDNode neonates = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("f3d08e78-829c-45ab-9cfc-320cbf4b4fbb"));
            //BDNode children = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("a4ef0b9e-56a2-4cc6-971b-3b31ab3bbf78"));
            //// change to subcategory, change layout variant
            //neonates.nodeType = (int)BDConstants.BDNodeType.BDSubcategory;
            //neonates.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus;
            //neonates.nodeKeyName = BDConstants.BDNodeType.BDSubcategory.ToString();
            //neonates.SetParent(parentNode);
            //children.nodeType = (int)BDConstants.BDNodeType.BDSubcategory;
            //children.nodeKeyName = BDConstants.BDNodeType.BDSubcategory.ToString();
            //children.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus;
            //children.SetParent(parentNode);
            //// create new node for Infants
            //BDNode infants = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse("0b24d10d-8375-45c1-82dc-f7aef63f71ec"));
            //infants.SetParent(parentNode);
            //infants.DisplayOrder = 1;
            //infants.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk;
            //infants.name = "Infants 1-3 months";
            //infants.nodeKeyName = BDConstants.BDNodeType.BDSubcategory.ToString();
            //pContext.SaveChanges();

            // Part 2:  rename node (previously for infants) and reset layout variants
            //BDNode newInfants = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("0b24d10d-8375-45c1-82dc-f7aef63f71ec"));
            //BDNode prevInfants = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("5634d24c-d08b-44db-b85f-dcf844d3236e"));
            //prevInfants.SetParent(newInfants);
            //prevInfants.name = "Low risk";
            //prevInfants.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk;
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, prevInfants, BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk, false);
            //pContext.SaveChanges();

            //// Part 3:  Create a copy of the BDPresentation and its children for 'High risk'
            //BDNode lowRiskPresentation = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("5634d24c-d08b-44db-b85f-dcf844d3236e"));

            //BDNode newInfantsNode = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("0b24d10d-8375-45c1-82dc-f7aef63f71ec"));
            //BDNode highRiskPresentation = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDPresentation, Guid.NewGuid());
            //highRiskPresentation.SetParent(newInfantsNode);
            //highRiskPresentation.DisplayOrder = 1;
            //highRiskPresentation.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk;
            //highRiskPresentation.name = "High risk";
            //highRiskPresentation.nodeKeyName = BDConstants.BDNodeType.BDPresentation.ToString();
            //pContext.SaveChanges();

            //List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, lowRiskPresentation);
            //for (int idxPG = 0; idxPG < pathogenGroups.Count; idxPG++)
            //{
            //    BDNode pathogenGroup = pathogenGroups[idxPG] as BDNode;
            //    BDNode hr_pathogenGroup = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDPathogenGroup, Guid.NewGuid());
            //    hr_pathogenGroup.SetParent(highRiskPresentation);
            //    hr_pathogenGroup.DisplayOrder = idxPG;
            //    hr_pathogenGroup.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk;
            //    hr_pathogenGroup.name = pathogenGroup.name;
            //    pContext.SaveChanges();

            //    List<IBDNode> pg_children = BDFabrik.GetChildrenForParent(pContext, pathogenGroup);
            //    for (int idxPG_C = 0; idxPG_C < pg_children.Count; idxPG_C++)
            //    {
            //        IBDNode pgChild = pg_children[idxPG_C];
            //        if (pgChild.NodeType == BDConstants.BDNodeType.BDPathogen)
            //        {
            //            BDNode pathogen = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDPathogen, Guid.NewGuid());
            //            pathogen.SetParent(hr_pathogenGroup);
            //            pathogen.DisplayOrder = idxPG_C;
            //            pathogen.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk;
            //            pathogen.name = pgChild.Name;
            //            pContext.SaveChanges();
            //        }
            //        else if (pgChild.NodeType == BDConstants.BDNodeType.BDTherapyGroup)
            //        {
            //            BDTherapyGroup tGroup = BDTherapyGroup.CreateBDTherapyGroup(pContext, hr_pathogenGroup.Uuid);
            //            tGroup.DisplayOrder = idxPG_C;
            //            tGroup.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk;
            //            tGroup.Name = pgChild.Name;
            //            pContext.SaveChanges();

            //            List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, pgChild.Uuid);
            //            for (int idxTherapies = 0; idxTherapies < therapies.Count; idxTherapies++)
            //            {
            //                BDTherapy lr_Therapy = therapies[idxTherapies];
            //                BDTherapy therapy = BDTherapy.CreateBDTherapy(pContext, tGroup.Uuid);
            //                therapy.DisplayOrder = idxTherapies;
            //                therapy.name = lr_Therapy.name;
            //                therapy.dosage = lr_Therapy.dosage;
            //                therapy.duration = lr_Therapy.duration;
            //                therapy.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk;
            //                pContext.SaveChanges();
            //            }
            //        }
            //    }
            //}
            
            #endregion
            #region v.1.5.43
            BDNode amphoB = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("3dc48109-147a-47a7-99c9-118373374784"));
            amphoB.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B;
            amphoB.nodeType = (int)BDConstants.BDNodeType.BDCategory;
            amphoB.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();
            #endregion
        }

        /// <summary>
        /// Manual (non-UI) move of nodes from one parent to the parent's sibling
        /// Does not change the node type
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNodeId"></param>
        /// <param name="pNewParentId"></param>
        /// <returns></returns>
        public static void MoveNode(Entities pContext, Guid pNodeId, Guid pNewParentId)
        {
            BDNode nodeToMove = BDNode.RetrieveNodeWithId(pContext, pNodeId);
            BDNode currentParent = BDNode.RetrieveNodeWithId(pContext, nodeToMove.ParentId.Value);

            BDNode newParent = BDNode.RetrieveNodeWithId(pContext, pNewParentId);

            nodeToMove.SetParent(newParent);
            nodeToMove.LayoutVariant = newParent.LayoutVariant;
            BDNode.Save(pContext, nodeToMove);
        }

        /// <summary>
        /// Manual (non-UI) move of nodes from one parent to the parent's sibling
        /// Does not change the node type
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <param name="pNewParentName"></param>
        /// <returns></returns>
        public static void MoveNode(Entities pContext, BDNode pNode, string pNewParentName)
        {
            BDNode currentParent = BDNode.RetrieveNodeWithId(pContext, pNode.ParentId.Value);

            List<IBDNode> parentSiblings = BDFabrik.GetChildrenForParent(pContext, BDNode.RetrieveNodeWithId(pContext, currentParent.parentId.Value));
            BDNode newParent = null;
            foreach (BDNode n in parentSiblings)
            {
                if (n.Name == pNewParentName)
                    newParent = n;
            }
            if (newParent != null)
                MoveNode(pContext, pNode.Uuid, newParent.Uuid);
        }

        /// <summary>
        /// Move all children from one parent node to another - parent nodes should be siblings
        /// Does not change the node type
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pCurrentParentId"></param>
        /// <param name="pNewParentId"></param>
        public static void MoveAllChildren(Entities pContext, Guid pCurrentParentId, Guid pNewParentId)
        {
            // check that parents are siblings
            BDNode currentParent = BDNode.RetrieveNodeWithId(pContext, pCurrentParentId);
            List<IBDNode> siblings = BDFabrik.GetChildrenForParent(pContext, BDNode.RetrieveNodeWithId(pContext, currentParent.parentId.Value));
            bool isSibling = false;
            foreach (BDNode s in siblings)
                if (s.Uuid == pNewParentId)
                    isSibling = true;

            if (isSibling)
            {
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, BDNode.RetrieveNodeWithId(pContext, pCurrentParentId));
                foreach (BDNode n in children)
                    MoveNode(pContext, n.Uuid, pNewParentId);
            }
        }

        /// <summary>
        /// Move all children from one parent node to another - parent nodes should be siblings
        /// Does not change the node type
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pCurrentParentId"></param>
        /// <param name="pNewParentId"></param>
        public static void MoveAllChildrenExcept(Entities pContext, Guid pCurrentParentId, Guid pNewParentId, List<Guid>pExceptionList)
        {
            // check that parents are siblings
            BDNode currentParent = BDNode.RetrieveNodeWithId(pContext, pCurrentParentId);
            BDNode newParent = BDNode.RetrieveNodeWithId(pContext, pNewParentId);

            List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, currentParent);
            foreach (BDNode n in children)
            {
                if(!pExceptionList.Contains(n.Uuid)) 
                    MoveNode(pContext, n.Uuid, pNewParentId);
            }
        }

        /// <summary>
        /// Reset the node type for BDTableCell which was set incorrectly by LoadFromAttributes
        /// </summary>
        /// <param name="pNodeType"></param>
        /// <param name="?"></param>
        public static void SetTableCellNodeType(Entities pContext)
        {
            List<IBDObject> entryList = new List<IBDObject>();
            IQueryable<BDTableCell> entries;

            // get ALL entries
            entries = (from entry in pContext.BDTableCells
                       select entry);

            //if (entries.Count() > 0)
            //    entryList = new List<IBDObject>(entries.ToList<BDTableCell>());

            foreach (BDTableCell cell in entries)
            {
                cell.nodeType = (int)BDConstants.BDNodeType.BDTableCell;
                BDTableCell.Save(pContext, cell);
            }
        }

        /// <summary>
        /// Reset layout variant for children of specified node.  Does not reset layout variant of parent.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pStartNode"></param>
        /// <param name="pNewLayoutVariant"></param>
        public static void ResetLayoutVariantWithChildren(Entities pContext, IBDNode pStartNode, BDConstants.LayoutVariantType pNewLayoutVariant, bool pResetParent)
        {
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pStartNode);

            if (childNodes.Count > 0)
            foreach (IBDNode child in childNodes)
            {
                ResetLayoutVariantWithChildren(pContext, child, pNewLayoutVariant, true);
                BDFabrik.SaveNode(pContext, child);
            }

            if (pResetParent)
            {
                pStartNode.LayoutVariant = pNewLayoutVariant;
                BDFabrik.SaveNode(pContext, pStartNode);
            }
        }

        public static void ResetLayoutVariantInTable3RowsForParent(Entities pContext, BDNode pParentNode, BDConstants.LayoutVariantType pNewLayoutVariant)
        {
            List<BDTableRow> tableRows = BDTableRow.RetrieveTableRowsForParentId(pContext, pParentNode.Uuid);
            pParentNode.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_NameListing;
            BDNode.Save(pContext, pParentNode);
            foreach (BDTableRow row in tableRows)
            {
                List<BDTableCell> nameTableCells = BDTableCell.RetrieveTableCellsForParentId(pContext, row.Uuid);
                row.Name = string.Empty;
                row.LayoutVariant = (row.LayoutVariant == BDConstants.LayoutVariantType.Table_3_Column_ContentRow ? BDConstants.LayoutVariantType.Antibiotics_NameListing_ContentRow : BDConstants.LayoutVariantType.Antibiotics_NameListing_HeaderRow);
                BDTableRow.Save(pContext, row);
                foreach (BDTableCell nameTableCell in nameTableCells)
                {
                    nameTableCell.LayoutVariant = row.LayoutVariant;
                    BDTableCell.Save(pContext, nameTableCell);
                }
            }
        }

        public static void ResetLayoutVariantInTable5RowsForParent(Entities pContext, BDNode pParentNode, BDConstants.LayoutVariantType pNewLayoutVariant)
        {
            List<BDTableRow> tableRows = BDTableRow.RetrieveTableRowsForParentId(pContext, pParentNode.Uuid);
            pParentNode.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity;
            BDNode.Save(pContext, pParentNode);
            foreach (BDTableRow row in tableRows)
            {
                List<BDTableCell> nameTableCells = BDTableCell.RetrieveTableCellsForParentId(pContext, row.Uuid);
                row.Name = string.Empty;
                row.LayoutVariant = (row.LayoutVariant == BDConstants.LayoutVariantType.Table_5_Column_ContentRow ? BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity_ContentRow : BDConstants.LayoutVariantType.Undefined);
                BDTableRow.Save(pContext, row);
                foreach (BDTableCell nameTableCell in nameTableCells)
                {
                    nameTableCell.LayoutVariant = row.LayoutVariant;
                    BDTableCell.Save(pContext, nameTableCell);
                }
            }
        }

        public static void MoveNodeToParentSibling(Entities pContext, Guid pNodeToMove, Guid pParentSibling, string nodeNameForRename, BDConstants.BDNodeType nodeTypeOfChildren)
        {
            BDNode parentSiblingNode = BDNode.RetrieveNodeWithId(pContext, pParentSibling);

            // move Adults > SSTI > Fournier's Gangrene to presentation level of Rapidly progressive SSTI
            BDNode nodeToMove = BDNode.RetrieveNodeWithId(pContext, pNodeToMove);
            //  find 'SINBLE PRESENTATION' and rename it with name of parent
            List<BDNode> childrenToMove = BDNode.RetrieveNodesForParentIdAndChildNodeType(pContext, nodeToMove.Uuid, nodeTypeOfChildren);
            foreach (BDNode childToMove in childrenToMove)
            {
                if (childToMove.Name == nodeNameForRename)
                {
                    {
                        childToMove.name = nodeToMove.name;
                        // change parent to parent's sibling
                        childToMove.SetParent(parentSiblingNode);
                        BDNode.Save(pContext, childToMove);
                    }
                }
            }
            // delete parent
            BDNode.Delete(pContext, nodeToMove, false);
        }

        public static void ConfigureLayoutMetadata(Entities pContext, BDConstants.LayoutVariantType pLayoutVariant, int pColumnDisplayOrder, string pColumnLabel, BDConstants.BDNodeType pNodeType, string pPropertyName, int pPropertyDisplayOrder, string pLinkedNoteText)
        {
            BDLayoutMetadata.Rebuild(pContext);
            // turn on the Layout metadata:  find the entry, set included to True
            BDLayoutMetadata layout = BDLayoutMetadata.Retrieve(pContext, pLayoutVariant);
            layout.included = true;
            BDLayoutMetadata.Save(pContext, layout);
            
            // check if columns exist
            // create columns for the layout
            BDLayoutMetadataColumn layoutC1 = BDLayoutMetadataColumn.Retrieve(pContext, pLayoutVariant, pNodeType, pPropertyName);
            if (layoutC1 == null)
                layoutC1 = BDLayoutMetadataColumn.Create(pContext, layout, pColumnDisplayOrder,pColumnLabel);
            
            // create linked note for column
            //TODO:  adjust this to deal with > 1 
            if (pLinkedNoteText.Length > 0)
            {
                StringBuilder noteText = new StringBuilder();
                noteText.AppendFormat(@"<p>{0}</p>", pLinkedNoteText);
                BDLinkedNote note = BDLinkedNote.CreateBDLinkedNote(pContext, Guid.NewGuid());
                note.documentText = noteText.ToString();
                BDLinkedNoteAssociation lna = BDLinkedNoteAssociation.CreateBDLinkedNoteAssociation(pContext, BDConstants.LinkedNoteType.Footnote, note.Uuid, BDConstants.BDNodeType.BDLayoutColumn, layoutC1.Uuid, BDLayoutMetadataColumn.PROPERTYNAME_LABEL);
                pContext.SaveChanges();
            }

            // associate properties with the column
            BDLayoutMetadataColumnNodeType layoutC1T1 = BDLayoutMetadataColumnNodeType.Retrieve(pContext, pLayoutVariant, pNodeType, layoutC1.Uuid);
            if(layoutC1T1 == null)
                layoutC1T1 = BDLayoutMetadataColumnNodeType.Create(pContext, layoutC1,pNodeType, pPropertyName, pPropertyDisplayOrder);

        }

        public static void CreateFootnoteForLayoutColumn(Entities pContext, BDConstants.LayoutVariantType pLayoutVariant, int pColumnDisplayOrder, string pFootnoteText)
        {
            BDLayoutMetadata.Rebuild(pContext);
            BDLayoutMetadata layout = BDLayoutMetadata.Retrieve(pContext, pLayoutVariant);
            if (layout != null)
            {
                List<BDLayoutMetadataColumn> columns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, layout);
                foreach (BDLayoutMetadataColumn col in columns)
                {
                    if (col.displayOrder == pColumnDisplayOrder)
                    {
                        StringBuilder noteText = new StringBuilder();
                        noteText.AppendFormat(@"<p>{0}</p>", pFootnoteText);
                        BDLinkedNote note = BDLinkedNote.CreateBDLinkedNote(pContext, Guid.NewGuid());
                        note.documentText = noteText.ToString();
                        BDLinkedNoteAssociation lna = BDLinkedNoteAssociation.CreateBDLinkedNoteAssociation(pContext, BDConstants.LinkedNoteType.Footnote, note.Uuid, BDConstants.BDNodeType.BDLayoutColumn, col.Uuid, BDLayoutMetadataColumn.PROPERTYNAME_LABEL);
                        pContext.SaveChanges();
                    }
                }
            }
        }

        public static string cleanNoteText(string pNoteText)
        {
            string resultText = string.Empty;
            string noteText = CleanseStringOfEmptyTag(pNoteText, "p");
            if (!string.IsNullOrEmpty(noteText))
            {
                resultText = pNoteText.Replace("<p>", string.Empty);
                resultText = resultText.Replace("</p>", "<br>");

                if (resultText.EndsWith("<br>")) resultText = resultText.Substring(0, resultText.Length - 4);
            }
            return resultText;
        }

        public static string buildTextFromInlineNotes(List<BDLinkedNote> pNotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder noteString = new StringBuilder();
            if (null != pNotes)
            {
                foreach (BDLinkedNote note in pNotes)
                {
                    if (null != note)
                    {
                        string documentText = CleanseStringOfEmptyTag(note.documentText, "p");
                        if (!string.IsNullOrEmpty(documentText))
                        {
                            documentText = documentText.Replace("<p>", string.Empty);
                            documentText = documentText.Replace("</p>", "<br>");

                            if (documentText.EndsWith("<br>"))
                            {
                                documentText = documentText.Substring(0, documentText.Length - 4);
                            }

                            noteString.AppendFormat(" {0}", documentText);
                            if (null != pObjectsOnPage) pObjectsOnPage.Add(note.Uuid);
                        }
                    }
                }
            }
            return noteString.ToString();
        }

        public static string buildTextFromNotes(List<BDLinkedNote> pNotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder noteString = new StringBuilder();
            foreach (BDLinkedNote note in pNotes)
            {
                if (null != note)
                {
                    string documentText = CleanseStringOfEmptyTag(note.documentText, "p");
                    if (!string.IsNullOrEmpty(documentText))
                    {
                        noteString.Append(note.documentText);
                        pObjectsOnPage.Add(note.Uuid);
                    } 
                }
            }

            return noteString.ToString();
        }

        /// <summary>
        /// Test for and cleanse if necessary, a string containing only a start and end tag. 
        /// Returns original string if not "empty" or string.Empty if cleansed
        /// </summary>
        /// <param name="pString"></param>
        /// <param name="pTagRoot"></param>
        /// <returns>Returns original string if not "empty" or string.Empty if cleansed</returns>
        public static string CleanseStringOfEmptyTag(string pString, string pTagRoot)
        {
            string result = pString;

            if (!String.IsNullOrEmpty(pTagRoot))
            {
                string startTag = string.Format("<{0}>", pTagRoot);
                string endTag = string.Format("</{0}>", pTagRoot);
                result = CleanseStringOfEmptyTag(pString, startTag, endTag);
            }
            return result;
        }

        /// <summary>
        /// Test for and cleanse if necessary, a string containing only a start and end tag.
        /// Returns original string if not "empty" or string.Empty if cleansed
        /// </summary>
        /// <param name="pString"></param>
        /// <param name="pStartTag"></param>
        /// <param name="pEndTag"></param>
        /// <returns>Returns original string if not "empty" or string.Empty if cleansed</returns>
        public static string CleanseStringOfEmptyTag(string pString, string pStartTag, string pEndTag)
        {
            string resultValue = pString;
            if (!String.IsNullOrEmpty(pStartTag) && !String.IsNullOrEmpty(pEndTag))
            {
                string startTag = pStartTag.ToLower();
                string endTag = pEndTag.ToLower();

                string testValue = pString.ToLower();
                testValue = testValue.Replace(startTag, string.Empty);
                testValue = testValue.Replace(endTag, string.Empty);

                if (string.IsNullOrEmpty(testValue))
                {
                    resultValue = string.Empty;
                }
            }

            return resultValue;
        }
    }
}
