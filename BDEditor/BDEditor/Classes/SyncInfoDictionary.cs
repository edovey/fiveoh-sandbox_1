using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Classes
{
    public class SyncInfoDictionary: Dictionary<string, SyncInfo>
    {
        public void Add(SyncInfo pSyncInfo)
        {
            base.Add(pSyncInfo.RemoteEntityName, pSyncInfo);
        }
    }
}
