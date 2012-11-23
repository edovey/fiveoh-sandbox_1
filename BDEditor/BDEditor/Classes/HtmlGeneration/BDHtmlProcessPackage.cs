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
        public List<Guid> ObjectsOnPage { get; set; }
        /// <summary>
        /// HTML segments to be rendered directly
        /// </summary>
        public List<BDHtmlSegment> InlineHtmlSegmentList { get; set; } 
        /// <summary>
        /// Notes to be rendered at the current level
        /// </summary>
        public BDHtmlNoteList InlineNoteList { get; set; }
        /// <summary>
        /// Notes to be rendered by the parent at the bottom of its page
        /// </summary>
        public BDHtmlNoteList FootnoteNoteList { get; set; }
        /// <summary>
        /// Notes to be rendered by the parent directly below the caller
        /// </summary>
        public BDHtmlNoteList LegendNoteList { get; set; }
        /// <summary>
        /// Notes to be rendered by the parent
        /// </summary>
        public BDHtmlNoteList ReferenceNoteList { get; set; }
        /// <summary>
        /// Notes that will rendered into a page prior to returning to the parent
        /// </summary>
        public BDHtmlNoteList CreatePageNoteList { get; set; }
        /// <summary>
        /// Notes that will be handled during post-processing
        /// </summary>
        public BDHtmlNoteList InternalLinkList { get; set; }
        /// <summary>
        /// The Guid (nullable) of the page that was created (if any)
        /// </summary>
        public Guid? CreatedHtmlPageUuid { get; set; }

        public BDHtmlProcessPackage()
        {
            ObjectsOnPage = new List<Guid>();
            InlineHtmlSegmentList = new List<BDHtmlSegment>();
            InlineNoteList = new BDHtmlNoteList(BDHtmlNoteList.BDHtmlNoteListType.Inline);
            FootnoteNoteList = new BDHtmlNoteList(BDHtmlNoteList.BDHtmlNoteListType.Footnote);
            LegendNoteList = new BDHtmlNoteList(BDHtmlNoteList.BDHtmlNoteListType.Legend);
            ReferenceNoteList = new BDHtmlNoteList(BDHtmlNoteList.BDHtmlNoteListType.Reference);
            CreatePageNoteList = new BDHtmlNoteList(BDHtmlNoteList.BDHtmlNoteListType.Page);
            InternalLinkList = new BDHtmlNoteList(BDHtmlNoteList.BDHtmlNoteListType.InternalLink);
            CreatedHtmlPageUuid = null;
        }

        public void AppendPackageTransients(BDHtmlProcessPackage pPackage)
        {
            if (null == pPackage) return;

            FootnoteNoteList.AddRange(pPackage.FootnoteNoteList);
            LegendNoteList.AddRange(pPackage.LegendNoteList);
            ReferenceNoteList.AddRange(pPackage.ReferenceNoteList);
            CreatePageNoteList.AddRange(pPackage.CreatePageNoteList);
            InlineNoteList.AddRange(pPackage.InlineNoteList);
            // Internal-Links don't stack
        }

        /// <summary>
        /// Append the lists from the processPackage parameter array into the instance lists. The following are currently excluded: CreatePageNoteList (marked and unmarked), InternalLinkList, ObjectsOnPage
        /// </summary>
        /// <param name="args"></param>
        public void AggregatePackages(params BDHtmlProcessPackage[] args)
        {
            foreach (BDHtmlProcessPackage pkg in args)
            {
                if (null != pkg)
                {
                    InlineHtmlSegmentList.AddRange(pkg.InlineHtmlSegmentList);
                    FootnoteNoteList.AddRange(pkg.FootnoteNoteList);
                    LegendNoteList.AddRange(pkg.LegendNoteList);
                    ReferenceNoteList.AddRange(pkg.ReferenceNoteList);
                    InlineNoteList.AddRange(pkg.InlineNoteList);

                    //Exclusions
                    /*
                    CreatePageNoteList
                    InternalLinkList
                    ObjectsOnPage
                    */
                }
            }
        }

        public string InlineHtml
        {
            get
            {
                StringBuilder html = new StringBuilder();

                foreach (BDHtmlSegment segment in InlineHtmlSegmentList)
                {
                    html.AppendFormat("<p>{0}</p>", segment.HtmlSegment);
                }
                return html.ToString();
            }
        }
    }
}
