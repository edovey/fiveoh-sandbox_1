﻿using System;
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
            if (pResetParent)
            {
                pStartNode.LayoutVariant = pNewLayoutVariant;
                BDFabrik.SaveNode(pContext, pStartNode);
            }

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pStartNode);
            if (childNodes.Count > 0)
                foreach (IBDNode child in childNodes)
                {
                    ResetLayoutVariantWithChildren(pContext, child, pNewLayoutVariant, true);
                }
        }
    }
}
