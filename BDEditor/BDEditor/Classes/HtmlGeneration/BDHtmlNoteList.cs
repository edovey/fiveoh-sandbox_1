using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Classes.HtmlGeneration
{
    public class BDHtmlNoteList: List<BDHtmlLinkedNote>
    {
        public enum BDHtmlNoteListType
        {
            Inline = 0,
            Footnote = 1,
            Legend = 2,
            Reference = 3,
            Page = 4,
            InternalLink = 5
        }

        public BDHtmlNoteListType ListType = BDHtmlNoteListType.Inline;

        public BDHtmlNoteList(BDHtmlNoteListType pListType)
        {
            ListType = pListType;
        }
    }
}
