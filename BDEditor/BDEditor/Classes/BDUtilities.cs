using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;
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

        public static string BuildHierarchyString(Entities pContext, Guid pStartNodeUuid, string pSeparationString)
        {
            IBDNode startNode = BDFabrik.RetrieveNode(pContext, pStartNodeUuid);
            return BuildHierarchyString(pContext, startNode, pSeparationString);
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
            {
                getParentName(pContext, pStartNode, hStringBuilder, pSeparationString).ToString();
                hStringBuilder.Append(ProcessTextToPlainText(pContext,pStartNode.Name));
                return hStringBuilder.ToString();
            }
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
                    string cleanedName = ProcessTextToPlainText(pContext, parentNode.Name);
                    pHierarchyValue.Insert(0, string.Format("{0}{1}", cleanedName, pSeparationString));
                    if (parentNode.ParentId != Guid.Empty)
                        pHierarchyValue = getParentName(pContext, parentNode, pHierarchyValue, pSeparationString);
                }
            }
            return pHierarchyValue;
        }

        public static void InjectNodeIntoHierarhy(Entities pContext)
        {
            //BDNode chapter = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDChapter, Guid.NewGuid());
            //BDNode section = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSection, Guid.NewGuid());

            //chapter.parentId = Guid.Empty;
            //chapter.parentType = 0;
            //chapter.parentKeyName = @"None";
            //chapter.name = "References";
            //chapter.layoutVariant = (int)BDConstants.LayoutVariantType.References;
            //chapter.displayOrder = 7;
            //BDNode.Save(pContext, chapter);

            //section.SetParent(chapter);
            //section.name = "";
            //section.layoutVariant = (int)BDConstants.LayoutVariantType.References;
            //section.displayOrder = 0;
            //BDNode.Save(pContext, section);
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

        public static void LoadSearchEntries(Entities pContext)
        {
            List<string> pathogens = BDNode.RetrieveNodeNamesForType(pContext, BDConstants.BDNodeType.BDPathogen).ToList<string>();
            List<string> therapies = BDTherapy.RetrieveTherapyNames(pContext).ToList<string>();
            List<string> diseases = BDNode.RetrieveNodeNamesForType(pContext, BDConstants.BDNodeType.BDDisease).ToList<string>();
            List<string> antimicrobials = BDNode.RetrieveNodeNamesForType(pContext, BDConstants.BDNodeType.BDAntimicrobial).ToList<string>();
            List<string> microorganisms = BDNode.RetrieveNodeNamesForType(pContext, BDConstants.BDNodeType.BDMicroorganism).ToList<string>();
            List<string> regimens = BDRegimen.RetrieveBDRegimenNames(pContext).ToList<string>();

            List<string> searchEntries = new List<string>();
            searchEntries.AddRange(pathogens);
            searchEntries.AddRange(therapies);
            searchEntries.AddRange(diseases);
            searchEntries.AddRange(antimicrobials);
            searchEntries.AddRange(microorganisms);
            searchEntries.AddRange(regimens);

            foreach (string node in searchEntries)
            {
                // get existing matching search entries
                IQueryable<BDSearchEntry> entries = (from entry in pContext.BDSearchEntries
                                                     where entry.name == node
                                                     select entry);

                // get existing matching search entries
                IQueryable<BDSearchEntry> contains = (from entry in pContext.BDSearchEntries
                                                     where node.Contains(entry.name)
                                                     select entry);

                    // get existing like search entries
                    IQueryable<BDSearchEntry> likeEntries = (from entry in pContext.BDSearchEntries
                                                             where entry.name.Contains(node)
                                                             select entry);


                // if matching search entry is not found, create one
                if (entries.Count() == 0 && contains.Count() == 0 && likeEntries.Count() == 0)
                        BDSearchEntry.CreateBDSearchEntry(pContext, node);
            }
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
                if (child.NodeType == BDConstants.BDNodeType.BDTableRow)
                {
                    List<BDTableCell> cells = BDTableCell.RetrieveTableCellsForParentId(pContext, child.Uuid);
                    foreach (BDTableCell cell in cells)
                    {
                        cell.LayoutVariant = pNewLayoutVariant;
                        BDTableCell.Save(pContext, cell);
                    }
                }
                else
                    ResetLayoutVariantWithChildren(pContext, child, pNewLayoutVariant, true);
                BDFabrik.SaveNode(pContext, child);
            }

            if (pResetParent)
            {
                pStartNode.LayoutVariant = pNewLayoutVariant;
                BDFabrik.SaveNode(pContext, pStartNode);
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

        /// <summary>
        /// Replaces p tags with br.
        /// </summary>
        /// <param name="pNoteText"></param>
        /// <returns></returns>
        public static string CleanNoteText(string pNoteText)
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

        public static string BuildTextFromInlineNotes(List<BDLinkedNote> pNotes)
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
                        }
                    }
                }
            }
            return noteString.ToString();
        }

        public static string BuildTextFromNotes(List<BDLinkedNote> pNotes)
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

        public static List<BDLinkedNote> RetrieveNotesForParentAndPropertyOfLinkedNoteType(Entities pContext, Guid pParentId, string pPropertyName, BDConstants.LinkedNoteType pNoteType, out List<Guid> pLinkedNoteAssociations)
        {
            List<BDLinkedNote> noteList = new List<BDLinkedNote>();
            pLinkedNoteAssociations = new List<Guid>();
            if (null != pPropertyName && pPropertyName.Length > 0)
            {
                List<BDLinkedNoteAssociation> list = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(pContext, pParentId, pPropertyName);
                foreach (BDLinkedNoteAssociation assn in list)
                {
                    if (assn.linkedNoteType == (int)pNoteType)
                    {
                        BDLinkedNote linkedNote = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, assn.linkedNoteId);
                        if (null != linkedNote)
                        {
                            noteList.Add(linkedNote);
                            pLinkedNoteAssociations.Add(assn.Uuid);
                        }
                    }
                }
            }
            return noteList;
        }

        public static string ProcessTextForCarriageReturn(Entities pContext, string pTextToProcess)
        {
            if (pTextToProcess.Contains("\n"))
                pTextToProcess.Replace("\n", "<br>");
            return pTextToProcess;
        }

        public static string ProcessTextForSubscriptAndSuperscriptMarkup(Entities pContext, string pTextToProcess)
        {
            string superscriptStart = @"{";
            string superscriptEnd = @"}";
            string subscriptStart = @"{{";
            string subscriptEnd = @"}}";
            string htmlSuperscriptStart = @"<sup>";
            string htmlSuperscriptEnd = @"</sup>";
            string htmlSubscriptStart = @"<sub>";
            string htmlSubscriptEnd = @"</sub>";

            if(!string.IsNullOrEmpty(pTextToProcess))
            {
                // do subscripts first because of double braces
                while (pTextToProcess.Contains(subscriptStart))
                {
                    int tStartIndex = pTextToProcess.IndexOf(subscriptStart);
                    pTextToProcess = pTextToProcess.Remove(tStartIndex, subscriptStart.Length);
                    pTextToProcess = pTextToProcess.Insert(tStartIndex, htmlSubscriptStart);
                    int tEndIndex = pTextToProcess.IndexOf(subscriptEnd, tStartIndex);
                    pTextToProcess = pTextToProcess.Remove(tEndIndex, subscriptEnd.Length);
                    pTextToProcess = pTextToProcess.Insert(tEndIndex, htmlSubscriptEnd);
                }

                while (pTextToProcess.Contains(superscriptStart))
                {
                    int tStartIndex = pTextToProcess.IndexOf(superscriptStart);
                    pTextToProcess = pTextToProcess.Remove(tStartIndex, superscriptStart.Length);
                    pTextToProcess = pTextToProcess.Insert(tStartIndex, htmlSuperscriptStart);
                    int tEndIndex = pTextToProcess.IndexOf(superscriptEnd, tStartIndex);
                    pTextToProcess = pTextToProcess.Remove(tEndIndex, superscriptEnd.Length);
                    pTextToProcess = pTextToProcess.Insert(tEndIndex, htmlSuperscriptEnd);
                }
            }

            return pTextToProcess;
        }

        public static string ProcessTextToPlainText(Entities pContext, string pTextToProcess)
        {
            string superscriptStart = @"{";
            string superscriptEnd = @"}";
            string subscriptStart = @"{{";
            string subscriptEnd = @"}}";
            string htmlSuperscriptStart = @"<sup>";
            string htmlSuperscriptEnd = @"</sup>";
            string htmlSubscriptStart = @"<sub>";
            string htmlSubscriptEnd = @"</sub>";
            string boldStart = @"<b>";
            string boldEnd = @"</b>";

            if (!string.IsNullOrEmpty(pTextToProcess))
            {
                // do subscripts first because of double braces
                pTextToProcess = pTextToProcess.Replace(subscriptStart, "");
                pTextToProcess = pTextToProcess.Replace(subscriptEnd, "");
                pTextToProcess = pTextToProcess.Replace(superscriptStart, "");
                pTextToProcess = pTextToProcess.Replace(superscriptEnd, "");
                pTextToProcess = pTextToProcess.Replace(htmlSuperscriptStart, "");
                pTextToProcess = pTextToProcess.Replace(htmlSuperscriptEnd, "");
                pTextToProcess = pTextToProcess.Replace(htmlSubscriptStart, "");
                pTextToProcess = pTextToProcess.Replace(htmlSubscriptEnd, "");
                pTextToProcess = pTextToProcess.Replace(boldStart, "");
                pTextToProcess = pTextToProcess.Replace(boldEnd, "");
                pTextToProcess = pTextToProcess.Replace("<br>", " ");
                pTextToProcess = pTextToProcess.Replace("<hr>", "");
                pTextToProcess = pTextToProcess.Replace("[", "");
                pTextToProcess = pTextToProcess.Replace("]", "");
            }
            return pTextToProcess;
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
            #region v.1.6.12 - v1.6.49
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
            BDNode tables = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("391bd1a4-daca-44e8-8fda-863f22128f1f"));
            tables.SetParent(newSection);
            tables.DisplayOrder = 0;
            BDNode.Save(dataContext, tables);

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
            List<BDNode> regimenGroups = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDPathogenGroup);
            foreach(BDNode regimenGroup in regimenGroups)
            {
                if (regimenGroup.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines)
                {
                    if (regimenGroup.Name == "Predictable Activity" || regimenGroup.Name == "Unpredictable Activity" || regimenGroup.Name == "No/insufficient Activity")
                    {
                        regimenGroup.nodeType = (int)BDConstants.BDNodeType.BDTopic;
                        regimenGroup.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
                        BDNode.Save(dataContext, regimenGroup);
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
                        List<IBDNode> regimenGroups = BDFabrik.GetChildrenForParent(dataContext, pathogenResistance);
                        foreach (IBDNode regimenGroup in regimenGroups)
                        {
                            List<IBDNode> regimens = BDFabrik.GetChildrenForParent(dataContext, regimenGroup);
                            foreach (IBDNode regimen in regimens)
                            {
                                regimen.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics;
                                BDFabrik.SaveNode(dataContext, regimen);
                            }
                            regimenGroup.Name = string.Empty;
                            regimenGroup.SetParent(xxtable);
                            regimenGroup.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics;
                            BDFabrik.SaveNode(dataContext, regimenGroup);
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
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Organisms_GramStainInterpretation, 0, "Bacterial Morphology", BDConstants.BDNodeType.BDSubcategory, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Organisms_GramStainInterpretation, 1, "Probable Organisms", BDConstants.BDNodeType.BDMicroorganism, BDNode.PROPERTYNAME_NAME, 0, "");

            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 0, "DRUG", BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 1, "RISK CATEGORY", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_LACTATIONRISK, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 2, "AAP RATING", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 3, "RELATIVE INFANT DOSE", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE, 0, "");

            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy, 0, "DRUG", BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy, 1, "FDA RISK CATEGORY", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy, 2, "RECOMMENDATION", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, 0, "");
            
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection, 0, "Antimicrobial Regimen", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection, 1, "Dose & Duration", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, 0, "");

            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen, 0, "SITUATION", BDConstants.BDNodeType.BDTherapyGroup, BDTherapyGroup.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen, 1, "DRUG", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen, 2, "ADULT DOSE", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen, 3, "PAEDIATRIC DOSE", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, 0, "");

            BDNode ieProphylaxis = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("376b287e-1d80-40f5-bb0b-512e52720687"));
            ieProphylaxis.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_IE;
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
            //    foreach (IBDNode tableRows in tableRows)
            //        BDUtilities.ResetLayoutVariantWithChildren(pContext, tableRows, BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity_ContentRow, true);

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

            //            List<BDTherapy> regimens = BDTherapy.RetrieveTherapiesForParentId(pContext, pgChild.Uuid);
            //            for (int idxTherapies = 0; idxTherapies < regimens.Count; idxTherapies++)
            //            {
            //                BDTherapy lr_Therapy = regimens[idxTherapies];
            //                BDTherapy regimen = BDTherapy.CreateBDTherapy(pContext, tGroup.Uuid);
            //                regimen.DisplayOrder = idxTherapies;
            //                regimen.name = lr_Therapy.name;
            //                regimen.dosage = lr_Therapy.dosage;
            //                regimen.duration = lr_Therapy.duration;
            //                regimen.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk;
            //                pContext.SaveChanges();
            //            }
            //        }
            //    }
            //}
            #endregion
            #region v.1.5.43
            //BDNode amphoB = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("3dc48109-147a-47a7-99c9-118373374784"));
            //amphoB.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B;
            //amphoB.nodeType = (int)BDConstants.BDNodeType.BDCategory;
            //amphoB.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            //pContext.SaveChanges();
            #endregion
            #region v.1.6.44
            BDNode section = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("376b287e-1d80-40f5-bb0b-512e52720687"));
            //BDNode category = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory, Guid.Parse("93e36f86-d472-49de-b002-843e12f7366b"));
            //category.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen;
            //category.displayOrder = 2;
            //category.SetParent(section);
            //category.Name = "Antimicrobial Regimens for Endocarditis Prophylaxis";
            //category.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            //pContext.SaveChanges();

            //BDNode newCategory = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("93e36f86-d472-49de-b002-843e12f7366b"));
            //List<BDTherapyGroup> children = BDTherapyGroup.RetrieveTherapyGroupsForParentId(pContext, section.Uuid);
            //foreach (BDTherapyGroup child in children)
            //    child.SetParent(newCategory);
            //pContext.SaveChanges();
            #endregion
            #region v.1.6.46
            //// necrotizing fasciitis - change layout variant
            //BDNode nf = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("9e2409c4-1ef0-49a9-ab20-887954f25ca0"));
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, nf, BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis, true);
            //pContext.SaveChanges();

            //// reorganize TR > Adult > CAP
            //BDNode cap = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("cc7487dd-3b06-4826-9554-4b822bcfd9ba"));

            //BDNode outpatient = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDPresentation, Guid.Parse("0a73d4a0-5c7a-4204-a0d5-0136ccdd973b"));
            ////BDNode outpatient = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("0a73d4a0-5c7a-4204-a0d5-0136ccdd973b"));
            //outpatient.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic;
            //outpatient.displayOrder = 0;
            //outpatient.SetParent(cap);
            //outpatient.Name = "CAP, Outpatient";
            //outpatient.nodeKeyName = BDConstants.BDNodeType.BDPresentation.ToString();
            //outpatient.nodeType = (int)BDConstants.BDNodeType.BDPresentation;
            //pContext.SaveChanges();

            //BDNode hospitalized = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("026f2db5-a22c-434b-927b-0fadd648c19f"));
            //hospitalized.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic;
            //hospitalized.displayOrder = 1;
            //hospitalized.SetParent(cap);
            //hospitalized.Name = "CAP, Hospitalized";
            //hospitalized.nodeKeyName = BDConstants.BDNodeType.BDPresentation.ToString();
            //hospitalized.nodeType = (int)BDConstants.BDNodeType.BDPresentation;
            //pContext.SaveChanges();

            //BDNode op_1 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("4a0d101e-b220-4564-b7de-1b3bfdd9c308"));
            //op_1.SetParent(outpatient);
            //op_1.Name = "No comorbid factors";
            //op_1.DisplayOrder = 0;
            //op_1.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //op_1.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //BDNode op_2 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("e429a80e-8518-4ea2-81dc-f27010e880a2"));
            //op_2.SetParent(outpatient);
            //op_2.Name = "Comorbid factors";
            //op_2.DisplayOrder = 1;
            //op_2.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //op_2.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //BDNode op_3 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("13b73c3b-5eff-48b5-935f-411dd7930c82"));
            //op_3.SetParent(outpatient);
            //op_3.Name = "Failure of 1st line agents";
            //op_3.DisplayOrder = 2;
            //op_3.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //op_3.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //BDNode ho_1 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("ef43b894-e820-4499-9466-83c5a15845ae"));
            //ho_1.SetParent(hospitalized);
            //ho_1.Name = "Moderate";
            //ho_1.DisplayOrder = 0;
            //ho_1.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //ho_1.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //BDNode ho_2 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("c085d1e6-31eb-4704-b11b-7379d2751804"));
            //ho_2.SetParent(hospitalized);
            //ho_2.Name = "Severe/ICU";
            //ho_2.DisplayOrder = 1;
            //ho_2.nodeType = (int)BDConstants.BDNodeType.BDTopic;
            //ho_2.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            //pContext.SaveChanges();

            //BDNode outpatient = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("0a73d4a0-5c7a-4204-a0d5-0136ccdd973b"));
            //BDNode hospitalized = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("026f2db5-a22c-434b-927b-0fadd648c19f"));
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, outpatient, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, false);
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, hospitalized, BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic, false);
            #endregion
            #region v.1.6.47 / 48
            //// necrotizing fasciitis - change layout variant (peds)
            //BDNode nf = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("4c2e056c-8276-45be-964d-e9f64e036beb"));
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, nf, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected, false);
            //pContext.SaveChanges();

            //BDNode node = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("0797d5fd-b764-4a88-aa2e-bf55d81b87bf"));
            //node.name = "S. lugdunensis";
            //pContext.SaveChanges();
            #endregion
            #region v1.6.49
            // clean hierarchy for Organisms_Therapy (remove microorganism, microorganism group
            //BDNode organismTherapies = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("472244a0-f8a3-43b2-b6dd-c23902e5ee28"));
            //List<IBDNode> categories = BDFabrik.GetChildrenForParent(pContext, organismTherapies);
            //foreach (IBDNode category in categories)
            //{
            //    List<IBDNode> microorganismGroups = BDFabrik.GetChildrenForParent(pContext, category);
            //    foreach (IBDNode mGroup in microorganismGroups)
            //    {
            //        List<IBDNode> micros = BDFabrik.GetChildrenForParent(pContext, mGroup);
            //        foreach (IBDNode m in micros)
            //        {
            //            if (m.NodeType == BDConstants.BDNodeType.BDMicroorganism)
            //                BDNode.Delete(pContext, m.Uuid, false);
            //            else
            //                Debug.WriteLine("Node is not a Microorganism: {0}", m.Uuid);
            //        }
            //        if (mGroup.NodeType == BDConstants.BDNodeType.BDMicroorganismGroup)
            //            BDNode.Delete(pContext, mGroup, false);
            //        else
            //            Debug.WriteLine("Node is not a Microorganism Group: {0}", mGroup.Uuid);
            //    }
            //    if (category.Uuid == Guid.Parse("8f959f69-fd2d-4074-bf0d-4a257a23828b"))
            //    {
            //        category.LayoutVariant = BDConstants.LayoutVariantType.Organisms_Therapy_with_Subcategory;
            //        category.Name = "GRAM NEGATIVE BACILLI";
            //        BDNode subcat1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSubcategory, Guid.NewGuid());
            //        subcat1.SetParent(category);
            //        subcat1.Name = "Enterobacteriaceae";
            //        subcat1.layoutVariant = (int)category.LayoutVariant;
            //        subcat1.DisplayOrder = 0;
            //        subcat1.nodeKeyName = BDConstants.BDNodeType.BDSubcategory.ToString();

            //        BDNode subcat2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSubcategory, Guid.NewGuid());
            //        subcat2.SetParent(category);
            //        subcat2.Name = "Fermentative (other)";
            //        subcat2.layoutVariant = (int)category.LayoutVariant;
            //        subcat2.DisplayOrder = 0;
            //        subcat2.nodeKeyName = BDConstants.BDNodeType.BDSubcategory.ToString();

            //        BDNode subcat3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSubcategory, Guid.NewGuid());
            //        subcat3.SetParent(category);
            //        subcat3.Name = "Non-Fermentative";
            //        subcat3.layoutVariant = (int)category.LayoutVariant;
            //        subcat3.DisplayOrder = 0;
            //        subcat3.nodeKeyName = BDConstants.BDNodeType.BDSubcategory.ToString();
            //    }
            //    if (category.Uuid == Guid.Parse("1de822f9-49b7-4e0c-a32f-fc0cb94fa619") || category.Uuid == Guid.Parse("af07c9f5-579a-4092-8acd-6c14fcc80cc5"))
            //        BDNode.Delete(pContext, category.Uuid, false);

            //}
            //pContext.SaveChanges();

            //// refactoring & cleanup of Surgical prophylaxis
            //BDNode preOp = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("6490c259-0188-4e1a-9ff1-001bb24db3c4"));
            //List<IBDNode> tables = BDFabrik.GetChildrenForParent(pContext, preOp);
            //// delete children of table 1 -- switching to a configured entry
            //foreach (BDNode table in tables)
            //{
            //    List<BDTableRow> tableRows = BDTableRow.RetrieveTableRowsForParentId(pContext, table.Uuid);
            //    foreach (BDTableRow row in tableRows)
            //    {
            //        List<BDTableCell> cells = BDTableCell.RetrieveTableCellsForParentId(pContext, row.Uuid);
            //        foreach (BDTableCell cell in cells)
            //            BDTableCell.Delete(pContext, cell, false);
            //        BDTableRow.Delete(pContext, row.Uuid, false);
            //    }
            //    BDNode surgical = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("da1fcc78-d169-45a8-a391-2b3db6247075"));
            //    if (table.Uuid == Guid.Parse("aba7ff59-63b6-41a6-afcb-1e5b2683ea9f"))
            //    {
            //        table.SetParent(surgical);
            //    }
            //    surgical.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            //    pContext.SaveChanges();
            //}

            //BDNode.Delete(pContext, preOp, false);
            #endregion           
            #endregion
            #endregion

            #region data clean for 1.6.63
            //List<IBDObject> associations = BDSearchEntryAssociation.RetrieveAll(pContext);

            //foreach (IBDObject entry in associations)
            //{
            //    BDSearchEntryAssociation seAssociation = entry as BDSearchEntryAssociation;

            //            if (seAssociation.editorContext.IndexOf("*") == 0)
            //                seAssociation.editorContext = seAssociation.editorContext.Substring(1);
            //            if (!string.IsNullOrEmpty(seAssociation.editorContext))
            //            {
            //                seAssociation.displayContext = seAssociation.editorContext;
            //            }
            //        BDSearchEntryAssociation.Save(pContext, seAssociation);  // will only save if there are changes.
                
            //}

            #endregion
            #region Surgical Proplylaxis : remove v1 data structures for 1.6.73
            // clean hierarchy for Surgical Prophylaxis 
             // preserve existing section
/*            BDNode prophylaxis = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("da1fcc78-d169-45a8-a391-2b3db6247075"));
            prophylaxis.layoutVariant = 3999;
            pContext.SaveChanges();

            IQueryable<BDNode> nodes = (from bdNodes in pContext.BDNodes
                                                                          where bdNodes.layoutVariant == 301 || bdNodes.layoutVariant == 302
                                                                          select bdNodes);

            List<BDNode> resultList = nodes.ToList<BDNode>();
            foreach (BDNode node in nodes)
            {
                BDNode nodeToDelete = BDNode.RetrieveNodeWithId(pContext, node.uuid);
                if (nodeToDelete.layoutVariant == 301 || nodeToDelete.layoutVariant == 302)
                {
                    BDNode.Delete(pContext, nodeToDelete.uuid, false);

                    pContext.SaveChanges();
                }
            }

            prophylaxis.layoutVariant = 301;
            pContext.SaveChanges();

            BDConfiguredEntry tableConfig = BDConfiguredEntry.RetrieveConfiguredEntryWithId(pContext, Guid.Parse("44a54d1c-c472-47f6-85e5-76a9d7ad5ca1"));
            BDConfiguredEntry.Delete(pContext, tableConfig.uuid);

            // create metadata for new layout variants: 3011, 3012, 3013, 3014, 3015, 3016
            string colTitle_1 = "COMMON PATHOGENS";
            string colTitle_2 = "REGIMEN(S) OF CHOICE";
            string colTitle_3 = "ALTERNATIVE REGIMENS FOR CEPHALOSPORIN ALLERGY or SEVERE PENICILLIN ALLERGY/ANAPHYLAXIS";
            string inlineNote = "(See General Principles)";

            // 3011
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp, 0, "Prophylactic Antibiotic", BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, 0, "");
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp, 1, "Recommended Adult Dose", BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, 1, "");
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp, 2, "Recommended Administration", BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03, 2, "");

            // 3012
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Intraoperative, 0, "Prophylactic Antibiotic", BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, 0, "");
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Intraoperative, 1, "Recommended intraoperative redosing interval (from time of administration of pre-op dose):", BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, 1, "");

            // 3013
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery, 0, colTitle_1, BDConstants.BDNodeType.BDMetaDecoration, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery, 1, colTitle_2, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, 1, inlineNote);
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery, 2, colTitle_3, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, 2, inlineNote);

            // 3014
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries, 0, colTitle_1, BDConstants.BDNodeType.BDMetaDecoration, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries, 1, colTitle_2, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, 1, inlineNote);
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries, 2, colTitle_3, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, 2, inlineNote);

            // 3015
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification, 0, colTitle_1, BDConstants.BDNodeType.BDMetaDecoration, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification, 1, colTitle_2, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, 1, inlineNote);
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification, 2, colTitle_3, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, 2, inlineNote);

            // 3016
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification, 0, colTitle_1, BDConstants.BDNodeType.BDMetaDecoration, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification, 1, colTitle_2, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, 1, inlineNote);
            BDUtilities.ConfigureLayoutMetadata(pContext, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification, 2, colTitle_3, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, 2, inlineNote);

            // Add new topic 'GENERAL PRINCIPLES'
            BDNode topic = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDTopic);
            topic.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Topic;
            topic.displayOrder = 0;
            topic.SetParent(prophylaxis);
            topic.Name = "GENERAL PRINCIPLES";
            topic.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
            pContext.SaveChanges();
            
            // Add new table 
            BDNode table_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDTable);
            table_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp;
            table_1.displayOrder = 1;
            table_1.SetParent(prophylaxis);
            table_1.Name = "Table 1:  Pre-Op Antibiotic Administration";
            table_1.nodeKeyName = BDConstants.BDNodeType.BDTable.ToString();
            pContext.SaveChanges();

            // Add new table
            BDNode table_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDTable);
            table_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Intraoperative;
            table_2.displayOrder = 2;
            table_2.SetParent(prophylaxis);
            table_2.Name = "Table 2:  Intraoperative Antibiotic Administration";
            table_2.nodeKeyName = BDConstants.BDNodeType.BDTable.ToString();
            pContext.SaveChanges();
            
            // Add new categories
            BDNode cat_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_1.displayOrder = 3;
            cat_1.SetParent(prophylaxis);
            cat_1.Name = "GENERAL";
            cat_1.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();

            BDNode cat_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_2.displayOrder = 4;
            cat_2.SetParent(prophylaxis);
            cat_2.Name = "OBSTETRICAL/GYNECOLOGICAL";
            cat_2.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();

            BDNode cat_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_3.displayOrder = 5;
            cat_3.SetParent(prophylaxis);
            cat_3.Name = "UROLOGY";
            cat_3.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();

            BDNode cat_4 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_4.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_4.displayOrder = 6;
            cat_4.SetParent(prophylaxis);
            cat_4.Name = "CARDIAC";
            cat_4.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();

            BDNode cat_5 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_5.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_5.displayOrder = 7;
            cat_5.SetParent(prophylaxis);
            cat_5.Name = "THORACIC";
            cat_5.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();

            BDNode cat_6 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_6.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_6.displayOrder = 8;
            cat_6.SetParent(prophylaxis);
            cat_6.Name = "VASCULAR";
            cat_6.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();

            BDNode cat_7 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_7.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_7.displayOrder = 9;
            cat_7.SetParent(prophylaxis);
            cat_7.Name = "PLASTICS";
            cat_7.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();

            BDNode cat_8 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_8.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_8.displayOrder = 10;
            cat_8.SetParent(prophylaxis);
            cat_8.Name = "ORTHOPAEDIC";
            cat_8.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();

            BDNode cat_9 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_9.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_9.displayOrder = 11;
            cat_9.SetParent(prophylaxis);
            cat_9.Name = "SPINAL SURGERY";
            cat_9.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();

            BDNode cat_10 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_10.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_10.displayOrder = 12;
            cat_10.SetParent(prophylaxis);
            cat_10.Name = "NEUROSURGERY";
            cat_10.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();

            BDNode cat_11 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_11.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_11.displayOrder = 13;
            cat_11.SetParent(prophylaxis);
            cat_11.Name = "HEAD AND NECK SURGERY";
            cat_11.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges();

            BDNode cat_12 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDCategory);
            cat_12.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            cat_12.displayOrder = 14;
            cat_12.SetParent(prophylaxis);
            cat_12.Name = "OPHTHALMOLOGY";
            cat_12.nodeKeyName = BDConstants.BDNodeType.BDCategory.ToString();
            pContext.SaveChanges(); */
            #endregion

            #region v1.6.74

            #region update schema: part 1
            // could not get this to work from code : execute manually in SQL Mgmt Studio.
            //BDUtilities.updateSchema("Schema.v1.6.74.txt");
            #endregion

            #region clean existing structures (part 2)
            //// clean BDConfigured Entries from data (structure change -> BDRegimenGroup, BDRegimen)
            //// Gastroesphageal low risk
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("b88f77cb-f142-4181-9a2e-4a004e289764"));
            //// Gastroesophageal high risk
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("a3e5931c-d7b3-4d44-a2e8-a1fc9c701271"));
            //// gastroduodenal
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("9c260527-70ca-4354-af8e-e888a223777a"));

            //// hepatobiliary ERCP
            //BDNode s1 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("573a55e3-7706-4c32-ac9a-c45a719c84e0"));
            //s1.nodeType = (int)BDConstants.BDNodeType.BDSurgery;
            //s1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, s1, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery, true);
            //BDUtilities.MoveNode(pContext, s1.Uuid, Guid.Parse("de6dfcce-cd3e-46b4-8dd7-468aa63ccdf7"));

            //// hepatobiliary liver resection
            //BDNode s2 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("04c2ad30-3453-4c11-b8eb-8bc9cb9ea4f8"));
            //s2.nodeType = (int)BDConstants.BDNodeType.BDSurgery;
            //s2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, s2, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery, true);
            //BDUtilities.MoveNode(pContext, s2.Uuid, Guid.Parse("de6dfcce-cd3e-46b4-8dd7-468aa63ccdf7"));

            //// hepatobiliary
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("f3efab98-3dc5-44e0-8b83-829d8e0ed970"));
            ////BDNode s3 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("f3efab98-3dc5-44e0-8b83-829d8e0ed970")); // hepatobiliary 

            //// bowel small intestine -- reclassify and move up to parent
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("82964846-20a9-425d-97fc-f3876b59bb23"));
            //BDNode s4 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("82964846-20a9-425d-97fc-f3876b59bb23"));
            //s4.nodeType = (int)BDConstants.BDNodeType.BDSurgery;
            //s4.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
            //s4.DisplayOrder = s2.DisplayOrder + 1;
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, s4, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery, true);
            //BDUtilities.MoveNode(pContext, s4.Uuid, Guid.Parse("de6dfcce-cd3e-46b4-8dd7-468aa63ccdf7"));

            //// bowel surgeries
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("3db49314-cab9-4830-90dd-3d68ea40773a"));
            //BDNode s5 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("3db49314-cab9-4830-90dd-3d68ea40773a"));
            //List<BDNode> surgeries = BDNode.RetrieveNodesForParentIdAndChildNodeType(pContext, s5.Uuid, BDConstants.BDNodeType.BDSurgeryClassification);
            //foreach (BDNode surgery in surgeries)
            //{
            //    surgery.nodeType = (int)BDConstants.BDNodeType.BDSurgery;
            //    surgery.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
            //    surgery.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
            //}

            //BDNode s5_1 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("ff86d931-5034-40d8-b6d5-24c34879c225"));
            //s5_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
            //s5_1.DisplayOrder++;
            //s5.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
            //pContext.SaveChanges();

            //BDNode s5_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
            //s5_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
            //s5_2.displayOrder = s5_1.DisplayOrder - 1;
            //s5_2.SetParent(s5);
            //s5_2.Name = "Bowel obstruction";
            //s5_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
            //pContext.SaveChanges();

            //// bowel - preforated viscus... -- reclassify and move up to parent
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("1fade626-21e3-4bfe-ae0c-b16437b5ff59"));
            //BDNode s6 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("1fade626-21e3-4bfe-ae0c-b16437b5ff59"));
            //s6.nodeType = (int)BDConstants.BDNodeType.BDSurgery;
            //s6.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
            //s6.DisplayOrder = s5.DisplayOrder + 1;
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, s6, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery, true);
            //BDUtilities.MoveNode(pContext, s6.Uuid, Guid.Parse("de6dfcce-cd3e-46b4-8dd7-468aa63ccdf7"));

            //// Anal Surgery - low risk
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("cae21114-bb50-4b5b-b27d-30d23da54920"));
            ////BDNode s7 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("cae21114-bb50-4b5b-b27d-30d23da54920")); // Anal Surgery - low risk

            //// anal surgery - high risk
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("c56d1557-a07c-4985-9827-36d35ff8b102 "));
            ////BDNode s8 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("c56d1557-a07c-4985-9827-36d35ff8b102 ")); // anal surgery - high risk

            //// Herniorraphy...
            //// delete child surgery classification
            //// clear name
            //// create 2 child surgeries
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("234c71a0-facf-458d-a000-90a139185315"));
            //BDNode s9 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("234c71a0-facf-458d-a000-90a139185315"));

            //Guid nodeToDeleteUuid = Guid.Empty;
            //List<BDNode> children = BDNode.RetrieveNodesForParentIdAndChildNodeType(pContext, s9.Uuid, BDConstants.BDNodeType.BDSurgeryClassification);
            //foreach (BDNode child in children)
            //{
            //    List<BDNode> pathogens = BDNode.RetrieveNodesForParentIdAndChildNodeType(pContext, s9.Uuid, BDConstants.BDNodeType.BDPathogen);
            //    foreach (BDNode pathogen in pathogens)
            //        BDUtilities.MoveNode(pContext, pathogen.Uuid, s9.Uuid);
            //    nodeToDeleteUuid = child.Uuid;
            //}
            //BDNode.Delete(pContext, nodeToDeleteUuid, true);
            //s9.name = String.Empty;

            //BDNode s9_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
            //s9_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
            //s9_1.displayOrder = 0;
            //s9_1.SetParent(s9);
            //s9_1.Name = "Herniorrhaphy (suture repair)";
            //s9_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();

            //BDNode s9_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
            //s9_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
            //s9_2.displayOrder = 1;
            //s9_2.SetParent(s9);
            //s9_2.Name = "Hernioplasty (mesh insertion)";
            //s9_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
            
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, s9, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries, true);

            //// Therapeutic termination of pregnancy
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("c6152d2f-3824-476e-b912-2e349050b28b"));
            ////BDNode s10 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("c6152d2f-3824-476e-b912-2e349050b28b")); // Therapeutic termination of pregnancy

            //// c section
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("c9a6b538-24d4-46cf-83c1-aca068036f6d"));
            ////BDNode s11 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("c9a6b538-24d4-46cf-83c1-aca068036f6d")); // c section

            //// Hysterectomy
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("9be19021-b01a-4384-981d-9bfbe1ccf936"));
            ////BDNode s12 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("9be19021-b01a-4384-981d-9bfbe1ccf936")); // Hysterectomy

            //// endometrial ablation
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("fe19c17f-3121-4109-8591-2cca9258c9d9"));
            ////BDNode s13 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("fe19c17f-3121-4109-8591-2cca9258c9d9")); // endometrial ablation

            //// dilatation and curettage
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("4aa907b8-45ab-4f15-902f-fd43b67bb3f0"));
            ////BDNode s14 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("4aa907b8-45ab-4f15-902f-fd43b67bb3f0")); // dilatation and curettage

            //// laparoscopic...
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("4479c873-987f-4ed4-9563-f0b71d117620"));
            ////BDNode s15 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("4479c873-987f-4ed4-9563-f0b71d117620")); // laparoscopic...

            //// open or laparoscopic...
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("bc42f59a-cd92-43a5-8067-12cc5301ee1f"));
            ////BDNode s16 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("bc42f59a-cd92-43a5-8067-12cc5301ee1f")); // open or laparoscopic...

            //// open or laparoscopic... with risk
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("441c1c3c-a154-4b7b-96be-9911d1ea12b1"));
            ////BDNode s17 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("441c1c3c-a154-4b7b-96be-9911d1ea12b1")); // open or laparoscopic... with risk

            //// cystoscopy.. low risk
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("149f37be-88e9-4158-b8af-736a1d3d564f"));
            ////BDNode s18 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("149f37be-88e9-4158-b8af-736a1d3d564f")); // cystoscopy.. low risk

            //// cystoscopy .. high risk
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("ece29577-d224-4839-80a4-0b212037d630"));
            ////BDNode s19 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("ece29577-d224-4839-80a4-0b212037d630")); // cystoscopy .. high risk

            //// shock wave lithotripsy
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("68baf6dc-3b18-42d8-943f-c69fba8b1d43"));
            ////BDNode s20 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("68baf6dc-3b18-42d8-943f-c69fba8b1d43")); // shock wave lithotripsy

            //// '' with risk factors
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("3512c95b-3688-49bd-8cf1-a40878f36eac"));
            ////BDNode s21 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("3512c95b-3688-49bd-8cf1-a40878f36eac")); // '' with risk factors

            //// surgery: adrenalectomy, nephrectomy
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("1cf37708-6290-4a2a-81a7-9e279dc71e8b"));
            //BDNode s22 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("1cf37708-6290-4a2a-81a7-9e279dc71e8b"));
            //// change to surgery group, make name blank, change layout type to surgeries
            //s22.nodeType = (int)BDConstants.BDNodeType.BDSurgeryGroup;
            //s22.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
            //s22.name = String.Empty;

            //// add surgeries: Adrenalectomy, Nephrectomy
            //BDNode s22_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
            //s22_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
            //s22_1.displayOrder = 1;
            //s22_1.SetParent(s22);
            //s22_1.Name = "Adrenalectomy";
            //s22_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();

            //BDNode s22_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
            //s22_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
            //s22_2.displayOrder = 2;
            //s22_2.SetParent(s22);
            //s22_2.Name = "Nephrectomy";
            //s22_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();

            //pContext.SaveChanges();

            //BDUtilities.ResetLayoutVariantWithChildren(pContext, s22, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries, true);

            //// surgery: cystoscopy, Urethral dilation 
            //BDUtilities.deleteConfiguredEntryChildrenForNode(pContext, Guid.Parse("b7a5aa78-5d51-42a7-8657-74d8716c714f"));
            //BDNode s23 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("b7a5aa78-5d51-42a7-8657-74d8716c714f"));
            //// change nodetype to surgery group, change layout to surgeries with risk classification, add surgeries
            //s23.nodeType = (int)BDConstants.BDNodeType.BDSurgeryGroup;
            //s23.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
            //s23.name = String.Empty;
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, s23, BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification, true);
            //int d_order = 2;

            //List<BDNode> classifications = BDNode.RetrieveNodesForParentIdAndChildNodeType(pContext, s23.Uuid, BDConstants.BDNodeType.BDSurgeryClassification);
            //foreach (BDNode cfn in classifications)
            //{
            //    cfn.parentType = (int)BDConstants.BDNodeType.BDSurgeryGroup;
            //    cfn.parentKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
            //    cfn.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification;
            //    cfn.displayOrder = d_order;
            //    d_order++;
            //}

            //// add surgeries: Cystoscopy, Urethral dilatation
            //BDNode s23_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
            //s23_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification;
            //s23_1.displayOrder = 0;
            //s23_1.SetParent(s23);
            //s23_1.Name = "Cystoscopy";
            //s23_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();

            //BDNode s23_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
            //s23_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification;
            //s23_2.displayOrder = 1;
            //s23_2.SetParent(s23);
            //s23_2.Name = "Urethral dilatation";
            //s23_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();

            //pContext.SaveChanges();

            #endregion

            #region add new layer to hierarchy (part 3)
           // // Urology
           // BDNode c1 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("fcd5c270-6bd8-40b1-a31e-030350fa069c"));

           // BDNode c1_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c1_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c1_1.DisplayOrder = 3;
           // c1_1.SetParent(c1);
           // c1_1.Name = string.Empty;
           // c1_1.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c1_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c1_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c1_2.DisplayOrder = 4;
           // c1_2.SetParent(c1);
           // c1_2.Name = String.Empty;
           // c1_2.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c1_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c1_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c1_3.DisplayOrder = 5;
           // c1_3.SetParent(c1);
           // c1_3.Name = String.Empty;
           // c1_3.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c1_4 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c1_4.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c1_4.DisplayOrder = 6;
           // c1_4.SetParent(c1);
           // c1_4.Name = "Vasectomy";
           // c1_4.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();


           // // reset existing node - shock wave lithotripsy.. with risk
           // BDNode c1_1_1 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("3512c95b-3688-49bd-8cf1-a40878f36eac"));
           // c1_1_1.SetParent(c1_1);
           // c1_1_1.DisplayOrder = 0;
           // c1_1_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // pContext.SaveChanges();
            
           // BDNode c1_1_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c1_1_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c1_1_2.DisplayOrder = 1;
           // c1_1_2.SetParent(c1_1);
           // c1_1_2.Name = "Ureteroscopy";
           // c1_1_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c1_2_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c1_2_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c1_2_1.DisplayOrder = 0;
           // c1_2_1.SetParent(c1_2);
           // c1_2_1.Name = "Transrectal prostatic biopsy";
           // c1_2_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c1_2_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c1_2_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c1_2_2.DisplayOrder = 1;
           // c1_2_2.SetParent(c1_2);
           // c1_2_2.Name = "Prostatectomy";
           // c1_2_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c1_3_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c1_3_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c1_3_1.DisplayOrder = 0;
           // c1_3_1.SetParent(c1_3);
           // c1_3_1.Name = "Ileal conduit / urinary diversion";
           // c1_3_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c1_3_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c1_3_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c1_3_2.DisplayOrder = 1;
           // c1_3_2.SetParent(c1_3);
           // c1_3_2.Name = "Cystectomy";
           // c1_3_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c1_3_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c1_3_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c1_3_3.DisplayOrder = 2;
           // c1_3_3.SetParent(c1_3);
           // c1_3_3.Name = "Radical prostatectomy";
           // c1_3_3.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();
            
           // // Cardiac
           // BDNode c2 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("93debd98-9aa0-4ee5-8197-9e15b8c20d13"));
           // BDNode c2_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c2_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c2_1.DisplayOrder = 0;
           // c2_1.SetParent(c2);
           // c2_1.Name = "Open heart surgery";
           // c2_1.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c2_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c2_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c2_2.DisplayOrder = 1;
           // c2_2.SetParent(c2);
           // c2_2.Name = "Placement of electrophysiologic devices";
           // c2_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c2_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c2_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c2_3.DisplayOrder = 2;
           // c2_3.SetParent(c2);
           // c2_3.Name = "";
           // c2_3.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c2_1_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c2_1_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c2_1_1.DisplayOrder = 0;
           // c2_1_1.SetParent(c2_1);
           // c2_1_1.Name = "Prosthetic valve";
           // c2_1_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c2_1_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c2_1_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c2_1_2.DisplayOrder = 1;
           // c2_1_2.SetParent(c2_1);
           // c2_1_2.Name = "Coronary artery bypass";
           // c2_1_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c2_1_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c2_1_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c2_1_3.DisplayOrder = 2;
           // c2_1_3.SetParent(c2_1);
           // c2_1_3.Name = "Other open heart surgery";
           // c2_1_3.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c2_3_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c2_3_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c2_3_1.DisplayOrder = 0;
           // c2_3_1.SetParent(c2_3);
           // c2_3_1.Name = "Cardiac catheterization";
           // c2_3_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c2_3_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c2_3_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c2_3_2.DisplayOrder = 1;
           // c2_3_2.SetParent(c2_3);
           // c2_3_2.Name = "Transesophageal echocardiogram";
           // c2_3_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // // Thoracic
           // BDNode c3 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("04dc812f-4fb0-4cc2-95e2-a7666f0a35c2")); 
           // BDNode c3_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c3_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c3_1.DisplayOrder = 0;
           // c3_1.SetParent(c3);
           // c3_1.Name = "Esophageal resection";
           // c3_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c3_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c3_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c3_2.DisplayOrder = 1;
           // c3_2.SetParent(c3);
           // c3_2.Name = "";
           // c3_2.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c3_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c3_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c3_3.DisplayOrder = 2;
           // c3_3.SetParent(c3);
           // c3_3.Name = "";
           // c3_3.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c3_4 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c3_4.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c3_4.DisplayOrder = 3;
           // c3_4.SetParent(c3);
           // c3_4.Name = "Closed chest tube insertion for chest trauma with hemo/pneumothorax";
           // c3_4.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();
            
           // BDNode c3_2_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c3_2_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c3_2_1.DisplayOrder = 0;
           // c3_2_1.SetParent(c3_2);
           // c3_2_1.Name = "Pneumonectomy";
           // c3_2_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c3_2_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c3_2_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c3_2_2.DisplayOrder = 1;
           // c3_2_2.SetParent(c3_2);
           // c3_2_2.Name = "Lobectomy, complete or partial";
           // c3_2_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c3_2_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c3_2_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c3_2_3.DisplayOrder = 2;
           // c3_2_3.SetParent(c3_2);
           // c3_2_3.Name = "Thoracotomy";
           // c3_2_3.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c3_2_4 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c3_2_4.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c3_2_4.DisplayOrder = 3;
           // c3_2_4.SetParent(c3_2);
           // c3_2_4.Name = "Thorascopy, including VATS";
           // c3_2_4.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();
            
           // BDNode c3_3_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c3_3_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c3_3_1.DisplayOrder = 0;
           // c3_3_1.SetParent(c3_3);
           // c3_3_1.Name = "Chest tube insertion for spontaneous pneumothorax";
           // c3_3_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c3_3_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c3_3_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c3_3_2.DisplayOrder = 1;
           // c3_3_2.SetParent(c3_3);
           // c3_3_2.Name = "Thoracentesis";
           // c3_3_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // // Vascular
           // BDNode c4 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("74882c7d-9cd8-4551-b07c-fede43517e08"));
           // BDNode c4_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c4_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c4_1.DisplayOrder = 0;
           // c4_1.SetParent(c4);
           // c4_1.Name = "Arterial surgery involving the abdominal aorta or a groin incision";
           // c4_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c4_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c4_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c4_2.DisplayOrder = 1;
           // c4_2.SetParent(c4);
           // c4_2.Name = "Arterial surgery involving placement of prosthetic material";
           // c4_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c4_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c4_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification;
           // c4_3.DisplayOrder = 2;
           // c4_3.SetParent(c4);
           // c4_3.Name = "";
           // c4_3.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c4_4 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c4_4.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c4_4.DisplayOrder = 3;
           // c4_4.SetParent(c4);
           // c4_4.Name = "Renal access procedures - native AV fistula";
           // c4_4.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c4_5 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c4_5.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c4_5.DisplayOrder = 4;
           // c4_5.SetParent(c4);
           // c4_5.Name = "Renal access procedures - artificial AV graft";
           // c4_5.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c4_6 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c4_6.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c4_6.DisplayOrder = 5;
           // c4_6.SetParent(c4);
           // c4_6.Name = "Peritoneal dialysis";
           // c4_6.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c4_3_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c4_3_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification;
           // c4_3_1.DisplayOrder = 0;
           // c4_3_1.SetParent(c4_3);
           // c4_3_1.Name = "Carotid endarterectomy";
           // c4_3_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c4_3_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c4_3_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification;
           // c4_3_2.DisplayOrder = 1;
           // c4_3_2.SetParent(c4_3);
           // c4_3_2.Name = "Brachial artery repair";
           // c4_3_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c4_3_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c4_3_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification;
           // c4_3_3.DisplayOrder = 2;
           // c4_3_3.SetParent(c4_3);
           // c4_3_3.Name = "Endovascular stenting";
           // c4_3_3.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c4_3_4 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c4_3_4.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification;
           // c4_3_4.DisplayOrder = 3;
           // c4_3_4.SetParent(c4_3);
           // c4_3_4.Name = "Low risk";
           // c4_3_4.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges();

           // BDNode c4_3_5 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c4_3_5.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification;
           // c4_3_5.DisplayOrder = 4;
           // c4_3_5.SetParent(c4_3);
           // c4_3_5.Name = "High risk";
           // c4_3_5.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges();

           // // Plastics
           // BDNode c5 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("3f55e087-6046-4278-8649-7692a1303d43"));
           // BDNode c5_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c5_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c5_1.DisplayOrder = 0;
           // c5_1.SetParent(c5);
           // c5_1.Name = "Clean procedures";
           // c5_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c5_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c5_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c5_2.DisplayOrder = 1;
           // c5_2.SetParent(c5);
           // c5_2.Name = "Clean-contaminated procedures";
           // c5_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c5_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c5_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c5_3.DisplayOrder = 2;
           // c5_3.SetParent(c5);
           // c5_3.Name = "Breast surgery";
           // c5_3.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c5_4 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c5_4.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c5_4.DisplayOrder = 3;
           // c5_4.SetParent(c5);
           // c5_4.Name = "Autologous breast reconstruction";
           // c5_4.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c5_5 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c5_5.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c5_5.DisplayOrder = 4;
           // c5_5.SetParent(c5);
           // c5_5.Name = "Reconstructive surgery";
           // c5_5.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c5_6 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c5_6.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c5_6.DisplayOrder = 5;
           // c5_6.SetParent(c5);
           // c5_6.Name = "";
           // c5_6.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c5_7 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c5_7.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c5_7.DisplayOrder = 6;
           // c5_7.SetParent(c5);
           // c5_7.Name = "Carpal tunnel";
           // c5_7.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c5_1_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c5_1_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c5_1_1.DisplayOrder = 0;
           // c5_1_1.SetParent(c5_1);
           // c5_1_1.Name = "Low risk";
           // c5_1_1.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges();

           // BDNode c5_1_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c5_1_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c5_1_2.DisplayOrder = 1;
           // c5_1_2.SetParent(c5_1);
           // c5_1_2.Name = "High risk";
           // c5_1_2.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges();

           // BDNode c5_3_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c5_3_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c5_3_1.DisplayOrder = 0;
           // c5_3_1.SetParent(c5_3);
           // c5_3_1.Name = "Low risk";
           // c5_3_1.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges();

           // BDNode c5_3_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c5_3_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c5_3_2.DisplayOrder = 1;
           // c5_3_2.SetParent(c5_3);
           // c5_3_2.Name = "High risk";
           // c5_3_2.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges();

           // BDNode c5_5_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c5_5_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c5_5_1.DisplayOrder = 0;
           // c5_5_1.SetParent(c5_5);
           // c5_5_1.Name = "Tissue flaps";
           // c5_5_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c5_5_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c5_5_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c5_5_2.DisplayOrder = 1;
           // c5_5_2.SetParent(c5_5);
           // c5_5_2.Name = "Panniculectomy";
           // c5_5_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c5_6_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c5_6_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c5_6_1.DisplayOrder = 0;
           // c5_6_1.SetParent(c5_6);
           // c5_6_1.Name = "Reconstructive limb surgery";
           // c5_6_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c5_6_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c5_6_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c5_6_2.DisplayOrder = 1;
           // c5_6_2.SetParent(c5_6);
           // c5_6_2.Name = "Traumatic / crush hand injuries";
           // c5_6_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c5_7_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c5_7_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c5_7_1.DisplayOrder = 0;
           // c5_7_1.SetParent(c5_7);
           // c5_7_1.Name = "Low risk";
           // c5_7_1.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges();

           // BDNode c5_7_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c5_7_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c5_7_2.DisplayOrder = 1;
           // c5_7_2.SetParent(c5_7);
           // c5_7_2.Name = "High risk";
           // c5_7_2.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges(); 
            
           // // Orthopaedic
           // BDNode c6 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("8a6a8d33-d0bd-4746-83d6-1714847d1acd"));
           // BDNode c6_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c6_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c6_1.DisplayOrder = 0;
           // c6_1.SetParent(c6);
           // c6_1.Name = "Diagnostic or operative arthroscopy";
           // c6_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c6_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c6_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c6_2.DisplayOrder = 1;
           // c6_2.SetParent(c6);
           // c6_2.Name = "Fractures with internal fixation";
           // c6_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c6_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c6_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c6_3.DisplayOrder = 2;
           // c6_3.SetParent(c6);
           // c6_3.Name = "Joint replacement";
           // c6_3.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c6_4 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c6_4.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c6_4.DisplayOrder = 3;
           // c6_4.SetParent(c6);
           // c6_4.Name = "Fractures, complex(open)";
           // c6_4.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c6_5 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c6_5.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c6_5.DisplayOrder = 4;
           // c6_5.SetParent(c6);
           // c6_5.Name = "Amputation of lower limb";
           // c6_5.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c6_6 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c6_6.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c6_6.DisplayOrder = 5;
           // c6_6.SetParent(c6);
           // c6_6.Name = "Fasciotomy";
           // c6_6.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // // Spinal
           // BDNode c7 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("58734f9a-c070-49a7-9369-fb2328d81694"));
           // BDNode c7_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c7_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c7_1.DisplayOrder = 0;
           // c7_1.SetParent(c7);
           // c7_1.Name = "";
           // c7_1.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c7_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c7_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c7_2.DisplayOrder = 1;
           // c7_2.SetParent(c7);
           // c7_2.Name = "";
           // c7_2.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c7_1_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c7_1_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c7_1_1.DisplayOrder = 0;
           // c7_1_1.SetParent(c7_1);
           // c7_1_1.Name = "Laminectomy";
           // c7_1_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c7_1_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c7_1_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c7_1_2.DisplayOrder = 1;
           // c7_1_2.SetParent(c7_1);
           // c7_1_2.Name = "Microdiscectomy";
           // c7_1_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c7_2_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c7_2_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c7_2_1.DisplayOrder = 0;
           // c7_2_1.SetParent(c7_2);
           // c7_2_1.Name = "Spinal fusion";
           // c7_2_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c7_2_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c7_2_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c7_2_2.DisplayOrder = 1;
           // c7_2_2.SetParent(c7_2);
           // c7_2_2.Name = "Insertion of foreign material";
           // c7_2_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges(); 
            

           // // Neuro
           // BDNode c8 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("f6c6e10f-2609-44b8-ae63-d58a40ab6642"));
           // BDNode c8_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c8_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c8_1.DisplayOrder = 0;
           // c8_1.SetParent(c8);
           // c8_1.Name = "";
           // c8_1.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c8_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c8_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c8_2.DisplayOrder = 1;
           // c8_2.SetParent(c8);
           // c8_2.Name = "Cerebrospinal fluid shunting operations";
           // c8_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c8_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c8_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c8_3.DisplayOrder = 2;
           // c8_3.SetParent(c8);
           // c8_3.Name = "";
           // c8_3.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c8_4 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c8_4.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery;
           // c8_4.DisplayOrder = 3;
           // c8_4.SetParent(c8);
           // c8_4.Name = "Contaminated procedures";
           // c8_4.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c8_1_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c8_1_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c8_1_1.DisplayOrder = 0;
           // c8_1_1.SetParent(c8_1);
           // c8_1_1.Name = "Craniotomy";
           // c8_1_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c8_1_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c8_1_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c8_1_2.DisplayOrder = 1;
           // c8_1_2.SetParent(c8_1);
           // c8_1_2.Name = "Stereotactic brain biopsy / procedure";
           // c8_1_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c8_3_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c8_3_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c8_3_1.DisplayOrder = 0;
           // c8_3_1.SetParent(c8_3);
           // c8_3_1.Name = "External ventricular drain (EVD)";
           // c8_3_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c8_3_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c8_3_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c8_3_2.DisplayOrder = 1;
           // c8_3_2.SetParent(c8_3);
           // c8_3_2.Name = "Intracranial pressure (ICP) monitor";
           // c8_3_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // // Head and Neck
           // BDNode c9 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("3355a333-238e-47d4-a1e4-8203b9ebfa74"));
           // BDNode c9_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c9_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c9_1.DisplayOrder = 0;
           // c9_1.SetParent(c9);
           // c9_1.Name = "Clean procedures";
           // c9_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           //  BDNode c9_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c9_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c9_2.DisplayOrder = 0;
           // c9_2.SetParent(c9);
           // c9_2.Name = "Clean contaminated procedures with incision through oral / nasal / pharyngeal mucosa";
           // c9_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           //BDNode c9_1_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c9_1_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c9_1_1.DisplayOrder = 0;
           // c9_1_1.SetParent(c9_1);
           // c9_1_1.Name = "Low risk";
           // c9_1_1.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges();

           // BDNode c9_1_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c9_1_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c9_1_2.DisplayOrder = 1;
           // c9_1_2.SetParent(c9_1);
           // c9_1_2.Name = "High risk";
           // c9_1_2.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges();

           // BDNode c9_2_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c9_2_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c9_2_1.DisplayOrder = 0;
           // c9_2_1.SetParent(c9_2);
           // c9_2_1.Name = "Low risk";
           // c9_2_1.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges();

           // BDNode c9_2_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryClassification);
           // c9_2_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification;
           // c9_2_2.DisplayOrder = 1;
           // c9_2_2.SetParent(c9_2);
           // c9_2_2.Name = "High risk";
           // c9_2_2.nodeKeyName = BDConstants.BDNodeType.BDSurgeryClassification.ToString();
           // pContext.SaveChanges(); 

           // // Ophthalmology
           // BDNode c10 = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("f44eea82-567b-4244-aa17-28d5be89e39a"));
           // BDNode c10_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgeryGroup);
           // c10_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c10_1.DisplayOrder = 0;
           // c10_1.SetParent(c10);
           // c10_1.Name = "";
           // c10_1.nodeKeyName = BDConstants.BDNodeType.BDSurgeryGroup.ToString();
           // pContext.SaveChanges();

           // BDNode c10_1_1 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c10_1_1.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c10_1_1.DisplayOrder = 0;
           // c10_1_1.SetParent(c10_1);
           // c10_1_1.Name = "Cataract extraction";
           // c10_1_1.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c10_1_2 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c10_1_2.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c10_1_2.DisplayOrder = 1;
           // c10_1_2.SetParent(c10_1);
           // c10_1_2.Name = "Corneal transplant";
           // c10_1_2.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c10_1_3 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c10_1_3.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c10_1_3.DisplayOrder = 2;
           // c10_1_3.SetParent(c10_1);
           // c10_1_3.Name = "Retinal detachment";
           // c10_1_3.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c10_1_4 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c10_1_4.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c10_1_4.DisplayOrder = 3;
           // c10_1_4.SetParent(c10_1);
           // c10_1_4.Name = "Vitrectomy";
           // c10_1_4.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c10_1_5 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c10_1_5.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c10_1_5.DisplayOrder = 4;
           // c10_1_5.SetParent(c10_1);
           // c10_1_5.Name = "Dacryocystorhinostomy";
           // c10_1_5.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c10_1_6 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c10_1_6.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c10_1_6.DisplayOrder = 5;
           // c10_1_6.SetParent(c10_1);
           // c10_1_6.Name = "Eyelid surgery";
           // c10_1_6.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

           // BDNode c10_1_7 = BDNode.CreateBDNode(pContext, BDConstants.BDNodeType.BDSurgery);
           // c10_1_7.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries;
           // c10_1_7.DisplayOrder = 6;
           // c10_1_7.SetParent(c10_1);
           // c10_1_7.Name = "Enucleation";
           // c10_1_7.nodeKeyName = BDConstants.BDNodeType.BDSurgery.ToString();
           // pContext.SaveChanges();

            #endregion
            #endregion

            #region v1.6.76
            /*
            // remove subsection from hierarchy
            
            BDNode bLactamSection = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("5b51a8e7-8003-4f52-b038-7f8d4917de8b"));
            BDNode subsection = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("e11afc50-d0e3-4b4a-98fd-1f8ca19ae9cc"));

            List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, subsection);
            foreach (IBDNode child in children)
            {
                child.SetParent(bLactamSection);
                pContext.SaveChanges();
            }
            */
            #endregion

            #region for v1.6.78 - adjust layout variant of selected tables to allow centering specific table columns
           // Desired Trough Level:  Table UUID: d633e893-d76a-48d7-8a26-024b5f3d3b4c
            //BDNode tableNode = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("d633e893-d76a-48d7-8a26-024b5f3d3b4c"));

            //BDUtilities.ResetLayoutVariantWithChildren(pContext, tableNode, BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin,true);

            //// Desired Serum Levels - Conventional (not High Dose) Aminoglycoside Monitoring
            //BDNode serumTableNode = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("bfb101e7-5aaa-40c0-a29f-572e07426636"));
            //BDUtilities.ResetLayoutVariantWithChildren(pContext, serumTableNode, BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional, true);
            #endregion

        }

        private static void deleteConfiguredEntryChildrenForNode(Entities pContext, Guid pNodeUuid)
        {
            List<BDConfiguredEntry> children = BDConfiguredEntry.RetrieveListForParentId(pContext, pNodeUuid);

            foreach (BDConfiguredEntry child in children)
                BDConfiguredEntry.Delete(pContext, child.uuid);

            pContext.SaveChanges();
        }

        [Obsolete]
        private static void updateSchema(String pFileName)
        {
            // could not get this to work - not successful in reading connectionString from app.config and opening the connection.

           // String currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
           // String fileWithPath = currentPath + @"\Resources\" + pFileName; 

           //if (!File.Exists(fileWithPath))
           //{
           //    Console.WriteLine("{0} does not exist.", pFileName);
           //    return;
           //}
           // String sqlScript = String.Empty;

           // using (StreamReader sr = File.OpenText(fileWithPath))
           // {
           //     sqlScript = sr.ReadToEnd();
           // }

           // if (sqlScript.Length > 0)
           // {
           //     using (SqlConnection cxn = new SqlConnection(ConfigurationManager.ConnectionStrings["Entities"].ConnectionString))
           //     {
           //         cxn.Open();
           //         SqlCommand cmd = new SqlCommand(sqlScript, cxn);
           //         cmd.ExecuteNonQuery();
           //     }
           // }
           // else
           //     Console.WriteLine("File contents not read");
        }

        #region data repair for V2 implementation
        public static void RepairSearchEntryAssociationsForMissingData(Entities pContext)
        {
            List<IBDNode> chapters = new List<IBDNode>();
            StringBuilder editorContext = new StringBuilder();
            chapters.AddRange(BDNode.RetrieveNodesForType(pContext, BDConstants.BDNodeType.BDChapter));
            processNodeList(pContext, chapters, editorContext);
        }

        private static void processNodeList(Entities pDataContext, List<IBDNode> pNodes, StringBuilder pNodeContext)
        {
            string resolvedName = string.Empty;
            foreach (IBDNode ibdNode in pNodes)
            {
                if (ibdNode.Name.IndexOf("DO NOT INCLUDE", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    resolvedName = string.Empty;
                    switch (ibdNode.NodeType)
                    {
                        case BDConstants.BDNodeType.BDAttachment:
                            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDAttachment.PROPERTYNAME_NAME);
                            break;
                        case BDConstants.BDNodeType.BDCombinedEntry:
                            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDCombinedEntry.PROPERTYNAME_NAME);
                            break;
                        case BDConstants.BDNodeType.BDConfiguredEntry:
                            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDConfiguredEntry.PROPERTYNAME_NAME);
                            break;
                        case BDConstants.BDNodeType.BDDosage:
                            // no valid properties
                            break;
                        case BDConstants.BDNodeType.BDLinkedNote:
                            break;
                        case BDConstants.BDNodeType.BDPrecaution:
                            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDPrecaution).Description, BDPrecaution.PROPERTYNAME_ORGANISM_1);
                            break;
                        case BDConstants.BDNodeType.BDTableCell:
                            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTableCell).value, BDTableCell.PROPERTYNAME_CONTENTS);
                            break;
                        case BDConstants.BDNodeType.BDTherapy:
                            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTherapy).Description, BDTherapy.PROPERTYNAME_THERAPY);
                            break;
                        case BDConstants.BDNodeType.BDTherapyGroup:
                            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTherapyGroup).Description, BDTherapyGroup.PROPERTYNAME_NAME);
                            break;
                        default:
                            // process all BDNodes, any type
                            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDNode.PROPERTYNAME_NAME);
                            break;
                    }

                    List<IBDNode> childnodes = BDFabrik.GetChildrenForParent(pDataContext, ibdNode);
                    Guid htmlPageId = BDHtmlPageMap.RetrieveHtmlPageIdForOriginalIBDNodeId(pDataContext, ibdNode.Uuid);
                    StringBuilder newContext = new StringBuilder();

                    // build a string representation of the search entry's location in the hierarchy
                    if (!string.IsNullOrEmpty(resolvedName))
                    {
                        if (!string.IsNullOrEmpty(pNodeContext.ToString()))
                            newContext.AppendFormat("{0} : {1}", pNodeContext, resolvedName);
                        else
                            newContext.Append(resolvedName);
                    }
                    else
                        newContext.Append(pNodeContext);

                    // recurse to process the next child layer
                    if (childnodes.Count > 0)
                        processNodeList(pDataContext, childnodes, newContext);

                    editSearchEntryLink(pDataContext, htmlPageId, ibdNode, resolvedName, pNodeContext.ToString());
                }
            }
        }

        private static void editSearchEntryLink(Entities pDataContext, Guid pOriginalNodeId, IBDNode pNode, string pResolvedName, string pDisplayContext)
        {
            List<string> allSearchEntries = BDSearchEntry.RetrieveSearchEntryNames(pDataContext);
            string entryName = pNode.Name.Trim();
            List<BDSearchEntry> matchingSearchEntries = new List<BDSearchEntry>();
            if (!string.IsNullOrEmpty(pResolvedName))
            {
                pDisplayContext = pDisplayContext.Replace(":  :", ":");

                foreach (string searchEntryTerm in allSearchEntries)
                {
                    if (pResolvedName.IndexOf(searchEntryTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        BDSearchEntry matchedSearchEntry = BDSearchEntry.RetrieveWithName(pDataContext, searchEntryTerm);
                        matchingSearchEntries.Add(matchedSearchEntry);
                        matchedSearchEntry.show = true;
                    }
                    else
                    {
                        string shortName = pResolvedName.Replace(" ", "");
                        string shortSearchTerm = searchEntryTerm.Replace(" ", "");
                        if (shortName.IndexOf(shortSearchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            BDSearchEntry matchedSearchEntry = BDSearchEntry.RetrieveWithName(pDataContext, searchEntryTerm);
                            matchingSearchEntries.Add(matchedSearchEntry);
                            matchedSearchEntry.show = true;
                        }
                    }
                }
                pDataContext.SaveChanges();
            }
            
            foreach (BDSearchEntry entry in matchingSearchEntries)
            {
                Guid htmlPageId = BDHtmlPageMap.RetrieveHtmlPageIdForOriginalIBDNodeId(pDataContext, pNode.Uuid);
                List<BDSearchEntryAssociation> associations = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryIdAndDisplayParentid(pDataContext, entry.uuid, pNode.Uuid);
                associations.AddRange(BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryIdAndDisplayParentid(pDataContext, entry.uuid, htmlPageId));

                if (associations.Count() == 0)
                {
                    BDSearchEntryAssociation assn = BDSearchEntryAssociation.CreateBDSearchEntryAssociation(pDataContext, entry.Uuid, pNode.Uuid, pDisplayContext);
                    associations.Add(assn);
                }
                    foreach (BDSearchEntryAssociation assn in associations)
                    {
                        if(!assn.anchorNodeId.HasValue) assn.anchorNodeId = pNode.Uuid;
                        if(string.IsNullOrEmpty(assn.editorContext)) assn.editorContext = string.Format("{0} : {1}",pDisplayContext, pResolvedName);
                        BDSearchEntryAssociation.Save(pDataContext, assn);
                }
            }
        }

        private static string buildResolvedNameForNode(Entities pContext, IBDNode pNode, string pPropertyValue, string pPropertyName)
        {
            List<Guid> immedAssociations;
            List<BDLinkedNote> immediate = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.Immediate, out immedAssociations);

            //ks: added "New " prefix to permit the use of terms like "Table A" to appear in the name of a BDTable instance
            string namePlaceholderText = string.Format(@"New {0}", BDUtilities.GetEnumDescription(pNode.NodeType));
            if (pPropertyValue.Contains(namePlaceholderText) || pPropertyValue == "SINGLE PRESENTATION" || pPropertyValue == "(Header)")
                pPropertyValue = string.Empty;

            if (pNode.NodeType == BDConstants.BDNodeType.BDConfiguredEntry && (pNode.Name.Length >=5 && pNode.Name.Substring(0, 5) == "Entry"))
                pPropertyValue = string.Empty;

            string immediateText = BDUtilities.BuildTextFromInlineNotes(immediate);

            string resolvedName = string.Format("{0}{1}",pPropertyValue.Trim(), immediateText.Trim());

            if (resolvedName.Length == 0) resolvedName = null;

            return BDUtilities.ProcessTextToPlainText(pContext, resolvedName);
        }
        #endregion
    }
}
