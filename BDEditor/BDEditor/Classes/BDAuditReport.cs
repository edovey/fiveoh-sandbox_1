using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.DataModel;
using System.IO;

namespace BDEditor.Classes
{
    class BDAuditReport
    {
        private Entities dataContext;

        public static void ReadAuditLog(Entities dataContext)
        {
            string filename = "BDHTMLPageReview.txt";
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = (Path.Combine(mydocpath, filename));

            if (!File.Exists(fullPath))
            {
                Console.WriteLine("{0} does not exist.", filename);
                return;
            }

            char[] delimiters = new char[] { '\t' };

            BDAuditLogEntry.ClearFile("BDAuditLog.txt");
            BDAuditLogEntry.AppendToFile("BDAuditLog.txt", string.Format("Start: {0} -------------------------------", DateTime.Now));
            //List<BDHtmlPage> pagesToAudit = new List<BDHtmlPage>();

            string uuidData = string.Empty;

            using (StreamReader sr = File.OpenText(fullPath))
            {
                String input;

                while ((input = sr.ReadLine()) != null)
                {
                    string[] elements = input.Split(delimiters, StringSplitOptions.None);
                    // elements[0] = datetime of record
                    if (elements.Length > 1) uuidData = elements[1];
                    // elements[2] = audit comment   ... sometimes UUID in old data

                        Guid nodeUuid = Guid.Empty;
                    bool isGuid = Guid.TryParse(uuidData, out nodeUuid);
                    if (!isGuid && elements.Length > 2)
                    {
                        uuidData = elements[2];
                        isGuid = Guid.TryParse(uuidData, out nodeUuid);
                    }
                    if (!isGuid)
                    {
                        Console.WriteLine("Guid not found in input string:{0}", input);
                        continue;  // skip to processing next line
                    }

                    if (null != uuidData)
                    {
                        
                        IBDNode tmpNode = BDFabrik.RetrieveNode(dataContext, nodeUuid);

                        if (null == tmpNode)
                            Console.WriteLine(string.Format("Node could not be found.\t{0}", tmpNode.Uuid.ToString()));
                        else
                        {
                            string locationString = BDUtilities.BuildHierarchyString(dataContext, tmpNode.Uuid, " : ");

                            List<BDHtmlPage> htmlPages = new List<BDHtmlPage>();
                            string uuidString = tmpNode.Uuid.ToString();
                            htmlPages = BDAuditReport.retrievePageForNode(tmpNode, dataContext);
                            if (htmlPages.Count == 0)
                            {
                                IBDNode parent = BDFabrik.RetrieveNode(dataContext, tmpNode.ParentId);
                                htmlPages = BDAuditReport.retrievePageForNode(parent, dataContext);

                                if (htmlPages.Count == 0)
                                {
                                    IBDNode grandparent = BDFabrik.RetrieveNode(dataContext, parent.ParentId);
                                    htmlPages = BDAuditReport.retrievePageForNode(grandparent, dataContext);
                                }
                            }

                            foreach (BDHtmlPage page in htmlPages)
                            {
                                if (page.HtmlPageType == BDConstants.BDHtmlPageType.Data || page.HtmlPageType == BDConstants.BDHtmlPageType.Navigation)
                                    BDAuditLogEntry.AppendToFile("BDAuditLog.txt", string.Format("HTML page\t{0}\t{1}", page.Uuid, locationString));
                            }
                        }
                    }
                }
            }
                    Console.WriteLine("Audit log generation complete.");
        }

        private static List<BDHtmlPage> retrievePageForNode(IBDNode tmpNode, Entities dataContext)
        {
            List<BDHtmlPage> htmlPages = new List<BDHtmlPage>();

            htmlPages = BDHtmlPage.RetrieveHtmlPageForDisplayParentIdOfPageType(dataContext, tmpNode.Uuid, BDConstants.BDHtmlPageType.Data);

            if (htmlPages.Count == 0)
            {
                // the node is not the topmost on the page, so check the pagemap
                List<BDHtmlPageMap> pageMaps = BDHtmlPageMap.RetrieveHtmlPageIdsForOriginalIBDNodeId(dataContext, tmpNode.Uuid);
                foreach (BDHtmlPageMap pageMap in pageMaps)
                {
                    BDHtmlPage page = BDHtmlPage.RetrieveWithId(dataContext, pageMap.htmlPageId);

                    if (page.HtmlPageType == BDConstants.BDHtmlPageType.Data)
                        htmlPages.Add(page);
                }
            }

            if (htmlPages.Count == 0)
            {
                Guid pageUuid = BDNodeToHtmlPageIndex.RetrieveHtmlPageIdForIBDNodeId(dataContext, tmpNode.Uuid, BDConstants.BDHtmlPageType.Data);
                if (pageUuid != Guid.Empty)
                    htmlPages.Add(BDHtmlPage.RetrieveWithId(dataContext, pageUuid));
            }

            return htmlPages;
        }
    }


    public class BDAuditLogEntry
    {
        public Guid Uuid { get; set; }
        public String Label { get; set; }
        public string Tag { get; set; }

        public BDAuditLogEntry(Guid? pUuid, String pLabel)
        {
            Uuid = (null == pUuid) ? Guid.Empty : pUuid.Value;
            Label = pLabel;
        }

        public BDAuditLogEntry(Guid? pUuid, String pLabel, string pTag)
        {
            Uuid = (null == pUuid) ? Guid.Empty : pUuid.Value;
            Label = pLabel;
            Tag = pTag;
        }

        public void AppendToFile(String pFilename)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outfile = new StreamWriter(Path.Combine(mydocpath, pFilename), true)) // overwrite the file if it exists
            {
                outfile.Write(this.ToString());
            }
        }

        static public void ClearFile(String pFilename)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullDocPath = Path.Combine(mydocpath, pFilename);
            File.WriteAllText(fullDocPath, string.Empty);
        }
        
        public override string ToString()
        {
            return string.Format(@"{0}\t{1}\t{2}", this.Uuid, this.Label, this.Tag);
        }

        static public void AppendToFile(string pFilename, string pStringData)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outfile = new StreamWriter(Path.Combine(mydocpath, pFilename), true)) // overwrite the file if it exists
            {
                outfile.WriteLine(pStringData);
            }
        }

        /// <summary>
        /// Write the log to disk, either appending to or overwriting existing
        /// </summary>
        /// <param name="pList"></param>
        /// <param name="pFolder"></param>
        /// <param name="pFilename"></param>
        /// <param name="pAppend">False: Overwrite</param>
        static public void WriteToFile(List<BDHtmlPageGeneratorLogEntry> pList, string pFolder, String pFilename, bool pAppend)
        {
            StringBuilder sbReferencePages = new StringBuilder();
            foreach (BDHtmlPageGeneratorLogEntry entry in pList)
            {
                sbReferencePages.AppendLine(entry.ToString());
            }

            using (StreamWriter outfile = new StreamWriter(Path.Combine(pFolder, pFilename), pAppend)) // overwrite the file if it exists
            {
                outfile.Write(sbReferencePages.ToString());
            }
        }

        /// <summary>
        /// Write the log to disk. Overwrite existing
        /// </summary>
        /// <param name="pList"></param>
        /// <param name="pFolder"></param>
        /// <param name="pFilename"></param>
        static public void WriteToFile(List<BDHtmlPageGeneratorLogEntry> pList, string pFolder, String pFilename)
        {
            BDHtmlPageGeneratorLogEntry.WriteToFile(pList, pFolder, pFilename, false);
        }

        /// <summary>
        /// Write list to filename. Use myDocuments folder
        /// </summary>
        /// <param name="pList"></param>
        /// <param name="pFilename"></param>
        static public void WriteToFile(List<BDHtmlPageGeneratorLogEntry> pList, String pFilename)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            BDHtmlPageGeneratorLogEntry.WriteToFile(pList, mydocpath, pFilename, false);
        }
    }
}
