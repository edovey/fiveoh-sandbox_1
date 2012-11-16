using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.Classes.HtmlGeneration
{
    public class BDHtmlLinkedNote
    {
        public Guid Uuid { get; set; }
        public string DocumentText { get; set; }

        static BDHtmlLinkedNote CopyFromLinkedNote(BDLinkedNote pOriginal)
        {
            BDHtmlLinkedNote entry = null;

            if (null != pOriginal)
            {
                entry = new BDHtmlLinkedNote();
                entry.Uuid = pOriginal.Uuid;
                entry.DocumentText = pOriginal.documentText;
            }
            return entry;
        }
    }
}
