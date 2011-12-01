using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Classes
{
    public class SyncInfo
    {
        public DateTime SyncDateTime { get; set; }
        public string FriendlyName { get; set; }
        public string RemoteEntityName { get; set; }
        public int RowsPulled { get; set; }
        public int RowsPushed { get; set; }
        public bool ExistsOnRemote { get; set; }
        public string ModifiedDatePropertyName { get; set; }
        public Exception Exception { get; set; }
        public List<BDEditor.DataModel.IBDObject> PushList { get; set; }

        public List<AttributeDictionary> SyncConflictList { get; set; }

        private SyncInfo(){}

        public SyncInfo(string pRemoteEntityName, string pModifiedDatePropertyName) 
        {
            FriendlyName = pRemoteEntityName;
            RemoteEntityName = pRemoteEntityName;
            ModifiedDatePropertyName = pModifiedDatePropertyName;
            RowsPulled = 0;
            RowsPushed = 0;
            ExistsOnRemote = false;
            SyncConflictList = new List<AttributeDictionary>();
            Exception = null;
            PushList = new List<DataModel.IBDObject>();
        }

        public string GetLatestRemoteSelectString(DateTime? pLastSyncDate)
        {
            string selectStatement = string.Empty; ;

            if (null == pLastSyncDate)
            {
                selectStatement = string.Format(@"Select * from {0}", RemoteEntityName);
            }
            else
            {
                selectStatement = string.Format(@"Select * from {0} where {1} > '{2}'", RemoteEntityName, ModifiedDatePropertyName, pLastSyncDate.Value.ToString(Constants.DATETIMEFORMAT));
            }

            return selectStatement;
        }

    }
}
