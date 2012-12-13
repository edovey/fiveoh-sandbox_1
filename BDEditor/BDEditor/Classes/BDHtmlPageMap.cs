using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Classes
{
    public class BDHtmlPageMap
    {
        private Guid originalIBDObjectId;
        private Guid htmlPageId;

        public Guid HtmlPageId
        {
            get { return htmlPageId; }
            set { htmlPageId = value; }
        }

        public Guid OriginalIBDObjectId
        {
            get { return originalIBDObjectId; }
            set { originalIBDObjectId = value; }
        }

        private BDHtmlPageMap() { }

        public BDHtmlPageMap(Guid pHtmlPageId, Guid pOriginalIBDObjectId)
        {
            HtmlPageId = pHtmlPageId;
            OriginalIBDObjectId = pOriginalIBDObjectId;
        }

        public override string ToString()
        {
            return string.Format("O={0} H={1}", originalIBDObjectId.ToString(), htmlPageId.ToString());
        }
    }
}
