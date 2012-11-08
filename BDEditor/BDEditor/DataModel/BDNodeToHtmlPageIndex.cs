using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;
using BDEditor.Classes;

namespace BDEditor.DataModel
{
    public partial class BDNodeToHtmlPageIndex
    {
        public const string ENTITYNAME = @"BDNodeToHtmlPageIndexes";
        public const string KEY_NAME = @"BDNodeToHtmlPageIndex";

        public static BDNodeToHtmlPageIndex CreateBDNodeToHtmlPageIndex(Entities pContext, Guid pNodeId, Guid pHtmlPageId, Guid pChapterId)
        {
            BDNodeToHtmlPageIndex nodeIndex = BDNodeToHtmlPageIndex.CreateBDNodeToHtmlPageIndex(pNodeId,true);
            nodeIndex.chapterId = pChapterId;
            nodeIndex.htmlPageId = pHtmlPageId;

            pContext.AddObject(ENTITYNAME, nodeIndex);
            return nodeIndex;
        }

        public static void Save(Entities pContext, BDNodeToHtmlPageIndex pIndexEntry)
        {
            if (pIndexEntry.EntityState != EntityState.Unchanged)
            {
                //System.Diagnostics.Debug.WriteLine(@"BDNodeToHtmlPageIndex Save");
                pContext.SaveChanges();
            }
        }

        public static Guid RetrieveHtmlPageIdForIBDNodeId(Entities pContext, Guid pIBDNodeId)
        {
            Guid returnValue = Guid.Empty;

            if (pIBDNodeId != null && pIBDNodeId != Guid.Empty)
            {
                IQueryable<BDNodeToHtmlPageIndex> entries = (from entry in pContext.BDNodeToHtmlPageIndexes
                                                             where entry.ibdNodeId == pIBDNodeId
                                                             select entry);
                returnValue = entries.FirstOrDefault<BDNodeToHtmlPageIndex>().htmlPageId.Value;
            }
            return returnValue;
        }

        public static BDNodeToHtmlPageIndex RetrieveIndexEntryForIBDNodeId(Entities pContext, Guid pIBDNodeId)
        {
            BDNodeToHtmlPageIndex returnValue = null;

            if (pIBDNodeId != null)
            {
                IQueryable<BDNodeToHtmlPageIndex> entries = (from entry in pContext.BDNodeToHtmlPageIndexes
                                                             where entry.ibdNodeId == pIBDNodeId
                                                             select entry);
                returnValue = entries.FirstOrDefault<BDNodeToHtmlPageIndex>();
            }
            return returnValue;
        }

        public static BDNodeToHtmlPageIndex RetrieveIndexEntryForHtmlPageId(Entities pContext, Guid pHtmlPageId)
        {
            BDNodeToHtmlPageIndex returnValue = null;

            if (pHtmlPageId != null)
            {
                IQueryable<BDNodeToHtmlPageIndex> entries = (from entry in pContext.BDNodeToHtmlPageIndexes
                                                             where entry.htmlPageId == pHtmlPageId
                                                             select entry);
                returnValue = entries.FirstOrDefault<BDNodeToHtmlPageIndex>();
            }
            return returnValue;
        }

        public static void WasGeneratedForChapter(Entities pContext, Guid pIBDNodeId, Guid pChapterId)
        {
            IQueryable<BDNodeToHtmlPageIndex> entries = (from entry in pContext.BDNodeToHtmlPageIndexes
                                                         where entry.ibdNodeId == pIBDNodeId
                                                         select entry);
            BDNodeToHtmlPageIndex entryToUpdate = entries.FirstOrDefault<BDNodeToHtmlPageIndex>();
            entryToUpdate.chapterId = pChapterId;
            entryToUpdate.wasGenerated = true;

            pContext.SaveChanges();
        }

        public static void ResetForRegeneration(Entities pContext)
        {
            List<BDNodeToHtmlPageIndex> allEntries = new List<BDNodeToHtmlPageIndex>();
            IQueryable<BDNodeToHtmlPageIndex> entries = from entry in pContext.BDNodeToHtmlPageIndexes
                                                        select entry;
            allEntries = entries.ToList<BDNodeToHtmlPageIndex>();

            foreach (BDNodeToHtmlPageIndex entry in allEntries)
                entry.wasGenerated = false;

            pContext.SaveChanges();
        }
    }
}
