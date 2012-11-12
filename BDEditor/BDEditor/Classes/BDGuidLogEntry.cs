using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BDEditor.Classes
{
    public class BDGuidLogEntry
    {
        public Guid Uuid { get; set; }
        public String Label { get; set; }
        public string Tag { get; set; }

        public BDGuidLogEntry(Guid pUuid, String pLabel)
        {
            Uuid = pUuid;
            Label = pLabel;
        }

        public BDGuidLogEntry(Guid pUuid, String pLabel, string pTag)
        {
            Uuid = pUuid;
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

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}", this.Uuid, this.Label, this.Tag);
        }

        /// <summary>
        /// Write the log to disk, either appending to or overwriting existing
        /// </summary>
        /// <param name="pList"></param>
        /// <param name="pFolder"></param>
        /// <param name="pFilename"></param>
        /// <param name="pAppend">False: Overwrite</param>
        static public void WriteToFile(List<BDGuidLogEntry> pList, string pFolder, String pFilename, bool pAppend)
        {
            StringBuilder sbReferencePages = new StringBuilder();
            foreach (BDGuidLogEntry entry in pList)
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
        static public void WriteToFile(List<BDGuidLogEntry> pList, string pFolder, String pFilename)
        {
            BDGuidLogEntry.WriteToFile(pList, pFolder, pFilename, false);
        }

        /// <summary>
        /// Write list to filename. Use myDocuments folder
        /// </summary>
        /// <param name="pList"></param>
        /// <param name="pFilename"></param>
        static public void WriteToFile(List<BDGuidLogEntry> pList, String pFilename)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            BDGuidLogEntry.WriteToFile(pList, mydocpath, pFilename, false);
        }
    }
}
