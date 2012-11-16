using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.Classes.HtmlGeneration
{
    public class BDHtmlSegment
    {
        public string HtmlSegment { get; set; }

        public BDHtmlSegment(string pSegment)
        {
            HtmlSegment = pSegment;
        }
    }
}
