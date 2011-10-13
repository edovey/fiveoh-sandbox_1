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

        public List<AttributeDictionary> SyncConflictList { get; set; }

        public SyncInfo(string pEntityName) 
        {
            EntityName = pEntityName;
            RowsPulled = 0;
            RowsPushed = 0;
            ExistsOnRemote = false;
            SyncConflictList = new List<AttributeDictionary>();
        }
    }
}
