using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.Classes.HtmlGeneration
{
    public class BDHtmlProcessPackage
    {
        public List<BDHtmlSegment> SegmentList { get; set; }
        public List<Guid> ObjectsOnPage { get; set; }

        public BDHtmlProcessPackage()
        {
            SegmentList = new List<BDHtmlSegment>();
            ObjectsOnPage = new List<Guid>();
        }
    }
}
