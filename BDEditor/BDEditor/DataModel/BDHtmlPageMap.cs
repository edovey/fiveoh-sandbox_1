﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    /// <summary>
    /// BDHtmlPageMap identifies the HTML page where each IBDNode is represented
    /// </summary>
    public partial class BDHtmlPageMap 
    {
        public const string ENTITYNAME = @"BDHtmlPageMap";
        public const string KEY_NAME = @"BDHtmlPageMap";
        public const string DEFAULTPARENTKEYPROPERTYNAME = @"< no key >";

        public static BDHtmlPageMap CreateBDHtmlPageMap(Entities pContext, Guid pHtmlPageId, Guid pOriginalIBDObjectId)
        {
            BDHtmlPageMap pageMap = CreateBDHtmlPageMap(pHtmlPageId, Guid.NewGuid(), pOriginalIBDObjectId);
            pContext.AddObject(ENTITYNAME, pageMap);

            BDHtmlPageMap.Save(pContext, pageMap);
            return pageMap;
        }

        public static BDHtmlPageMap RetrieveByHtmlPageIdAndOriginalObjectId(Entities pContext, Guid pHtmlPageId, Guid pOriginalIBDObjectId)
        {
            if (pOriginalIBDObjectId != null && pOriginalIBDObjectId != Guid.Empty)
            {
                IQueryable<BDHtmlPageMap> entries;

                entries = (from entry in pContext.BDHtmlPageMap
                           where (entry.originalIbdObjectId == pOriginalIBDObjectId && entry.htmlPageId == pHtmlPageId)
                           select entry);

                BDHtmlPageMap indexEntry = entries.FirstOrDefault<BDHtmlPageMap>();
                if (null != indexEntry)
                    return indexEntry;
            }
            return null;
        }

        public static BDHtmlPageMap CreateOrRetrieveBDHtmlPageMap(Entities pContext, Guid pHtmlPageId, Guid pPageMapId, Guid pOriginalIBDObjectId)
        {
            BDHtmlPageMap pageMap = RetrieveByHtmlPageIdAndOriginalObjectId(pContext, pHtmlPageId, pOriginalIBDObjectId);
            if (pageMap == null)
            {
                BDHtmlPageMap entry = null;

                if (null != pPageMapId)
                {
                    IQueryable<BDHtmlPageMap> entries = (from pageMaps in pContext.BDHtmlPageMap
                                                         where pageMaps.uuid == pPageMapId
                                                         select pageMaps);
                    if (entries.Count<BDHtmlPageMap>() > 0)
                        entry = entries.AsQueryable().First<BDHtmlPageMap>();
                }
                if (entry != null)
                {
                    pPageMapId = Guid.NewGuid();
                }
                //  CreateBDHtmlPageMap(global::System.Guid htmlPageId, global::System.Guid uuid, global::System.Guid originalIbdObjectId)
                pageMap = CreateBDHtmlPageMap(pHtmlPageId, pPageMapId, pOriginalIBDObjectId);
                pContext.AddObject(ENTITYNAME, pageMap);

                    pContext.SaveChanges();
            }
            return pageMap;
        }

        public static void Save(Entities pContext, BDHtmlPageMap pPageMap)
        {
            if (pPageMap.EntityState != EntityState.Unchanged)
                pContext.SaveChanges();
        }

        public static void DeleteAll(Entities pContext)
        {
            pContext.ExecuteStoreCommand("DELETE FROM BDHtmlPageMap");
        }

        /// <summary>
        /// Internal Links can only be targeted at nodes (not other links), so this list is filtered to include only
        /// data pages.  (Navigation pages are checked through another path.)
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pOriginalIBDObjectId"></param>
        /// <returns></returns>
        public static Guid RetrieveHtmlPageIdForInternalLinkByOriginalIBDNodeId(Entities pContext, Guid pOriginalIBDObjectId)
        {
            List<Guid> pageList = new List<Guid>();
            if (pOriginalIBDObjectId != null && pOriginalIBDObjectId != Guid.Empty)
            {
                IQueryable<BDHtmlPageMap> entries;

                entries = (from entry in pContext.BDHtmlPageMap
                           where (entry.originalIbdObjectId == pOriginalIBDObjectId)
                           select entry);

                List<BDHtmlPageMap> indexEntries = entries.Distinct<BDHtmlPageMap>().ToList<BDHtmlPageMap>();
                foreach (BDHtmlPageMap mapEntry in indexEntries)
                {
                    BDHtmlPage page = BDHtmlPage.RetrieveWithId(pContext, mapEntry.htmlPageId);
                    if (null != page)
                    {
                        if (page.HtmlPageType == BDConstants.BDHtmlPageType.Data
                            && !pageList.Contains(mapEntry.htmlPageId))
                            pageList.Add(mapEntry.htmlPageId);
                        else if (page.HtmlPageType == BDConstants.BDHtmlPageType.Navigation
                            && page.displayParentId == pOriginalIBDObjectId
                            && !pageList.Contains(mapEntry.htmlPageId))
                            pageList.Add(mapEntry.htmlPageId);
                    }
                }
            }

            if (pageList.Count > 1)
                throw new BDException("Multiple matching HTML pages were found.");
            else
                return pageList.FirstOrDefault<Guid>();
        }

        public static Guid RetrieveHtmlPageIdForOriginalIBDNodeId(Entities pContext, Guid pOriginalIBDObjectId)
        {
            Guid returnValue = Guid.Empty;
            if (pOriginalIBDObjectId != null && pOriginalIBDObjectId != Guid.Empty)
            {
                IQueryable<BDHtmlPageMap> entries;

                entries = (from entry in pContext.BDHtmlPageMap
                           where (entry.originalIbdObjectId == pOriginalIBDObjectId)
                           select entry);

                BDHtmlPageMap indexEntry = entries.FirstOrDefault<BDHtmlPageMap>();
                if (null != indexEntry)
                    returnValue = indexEntry.htmlPageId;
            }
            return returnValue;
        }

        public static List<BDHtmlPageMap> RetrieveHtmlPageIdsForOriginalIBDNodeId(Entities pContext, Guid pOriginalIBDObjectId)
        {
            List<BDHtmlPageMap> resultList = new List<BDHtmlPageMap>();
            if (pOriginalIBDObjectId != null && pOriginalIBDObjectId != Guid.Empty)
            {
                IQueryable<BDHtmlPageMap> entries;

                entries = (from entry in pContext.BDHtmlPageMap
                           where (entry.originalIbdObjectId == pOriginalIBDObjectId)
                           select entry);

                resultList = entries.ToList<BDHtmlPageMap>();
            }
            return resultList;
        }

        public static Guid RetrieveHtmlPageIdForOriginalIBDNodeId(Entities pContext, Guid pOriginalIBDObjectId, BDConstants.BDHtmlPageType pPageType )
        {
            Guid returnValue = Guid.Empty;
            if (pOriginalIBDObjectId != null && pOriginalIBDObjectId != Guid.Empty)
            {
                IQueryable<BDHtmlPageMap> entries;

                entries = (from entry in pContext.BDHtmlPageMap
                           from page in pContext.BDHtmlPages
                           where (entry.originalIbdObjectId == pOriginalIBDObjectId && page.uuid == entry.htmlPageId) &&
                           page.htmlPageType == (int)pPageType
                           select entry);

                BDHtmlPageMap indexEntry = entries.FirstOrDefault<BDHtmlPageMap>();
                if (null != indexEntry)
                    returnValue = indexEntry.htmlPageId;
            }
            return returnValue;
        }
                 
        public static Guid RetrieveOriginaIBDNodeIdForHtmlPageId(Entities pContext, Guid pHtmlPageId)
        {
            Guid returnValue = Guid.Empty;
            if (pHtmlPageId != null && pHtmlPageId != Guid.Empty)
            {
                IQueryable<BDHtmlPageMap> entries;

                entries = (from entry in pContext.BDHtmlPageMap
                           where (entry.htmlPageId == pHtmlPageId) 
                           select entry);

                BDHtmlPageMap indexEntry = entries.FirstOrDefault<BDHtmlPageMap>();
                if (null != indexEntry)
                    returnValue = indexEntry.originalIbdObjectId;
            }
            return returnValue;
        }

        public override string ToString()
        {
            return string.Format("O={0} H={1}", originalIbdObjectId.ToString(), htmlPageId.ToString());
        }
    }
}
