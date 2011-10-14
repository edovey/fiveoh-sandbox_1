using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Classes
{
    public class SyncInfo
    {
        public DateTime SyncDateTime { get; set; }
        public string EntityName { get; set; }
        public int RowsPulled { get; set; }
        public int RowsPushed { get; set; }
        public bool ExistsOnRemote { get; set; }
        public string ModifiedDatePropertyName { get; set; }
        public Exception Exception { get; set; }

        public List<AttributeDictionary> SyncConflictList { get; set; }

        public SyncInfo(string pEntityName, string pModifiedDatePropertyName) 
        {
            EntityName = pEntityName;
            ModifiedDatePropertyName = pModifiedDatePropertyName;
            RowsPulled = 0;
            RowsPushed = 0;
            ExistsOnRemote = false;
            SyncConflictList = new List<AttributeDictionary>();
            Exception = null;
        }

        public string GetLatestSelectString(DateTime? pLastSyncDate)
        {
            string selectStatement = string.Empty; ;

            if (null == pLastSyncDate)
            {
                selectStatement = string.Format(@"Select * from {0}", EntityName);
            }
            else
            {
                selectStatement = string.Format(@"Select * from {0} where {1} > '{2}'", EntityName, ModifiedDatePropertyName, pLastSyncDate.Value.ToString(Constants.DATETIMEFORMAT));
            }

            return selectStatement;
        }

    }
}
