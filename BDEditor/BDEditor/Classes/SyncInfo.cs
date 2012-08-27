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
        public string RemoteProductionEntityName { get; set; }
        public string RemoteDevelopmentEntityName { get; set; }
        public int RowsPulled { get; set; }
        public int RowsPushed { get; set; }
        public bool ExistsOnRemote { get; set; }
        public bool ExistsOnRemoteProduction { get; set; }
        public bool ExistsOnRemoteDevelopment { get; set; }
        public string ModifiedDatePropertyName { get; set; }
        public Exception Exception { get; set; }
        public List<BDEditor.DataModel.IBDObject> PushList { get; set; }

        public List<AttributeDictionary> SyncConflictList { get; set; }

        private SyncInfo(){}

        /// <summary>
        /// Class specific information needed for synchronizing with the remote cloud
        /// </summary>
        /// <param name="pRemoteEntityName"></param>
        /// <param name="pModifiedDatePropertyName"></param>
        /// <param name="pRemoteProductionEntityName"></param>
        /// <param name="pRemoteDevelopmentPropertyName"></param>
        public SyncInfo(string pRemoteEntityName, string pModifiedDatePropertyName, string pRemoteProductionEntityName, string pRemoteDevelopmentPropertyName) 
        {
            FriendlyName = pRemoteEntityName;
            RemoteEntityName = pRemoteEntityName;
            RemoteProductionEntityName = pRemoteProductionEntityName;
            RemoteDevelopmentEntityName = pRemoteDevelopmentPropertyName;
            ModifiedDatePropertyName = pModifiedDatePropertyName;
            RowsPulled = 0;
            RowsPushed = 0;
            ExistsOnRemoteProduction = false;
            ExistsOnRemoteDevelopment = false;
            ExistsOnRemote = false;
            SyncConflictList = new List<AttributeDictionary>();
            Exception = null;
            PushList = new List<DataModel.IBDObject>();
        }

        public SyncInfo(string pRemoteEntityName, string pRemoteProductionEntityName, string pRemoteDevelopmentPropertyName)
        {
            FriendlyName = pRemoteEntityName;
            RemoteEntityName = pRemoteEntityName;
            RemoteProductionEntityName = pRemoteProductionEntityName;
            RemoteDevelopmentEntityName = pRemoteDevelopmentPropertyName;
            RowsPulled = 0;
            RowsPushed = 0;
            ExistsOnRemoteProduction = false;
            ExistsOnRemoteDevelopment = false;
            ExistsOnRemote = false;
            SyncConflictList = new List<AttributeDictionary>();
            Exception = null;
            PushList = new List<DataModel.IBDObject>();
        }
    }
}
