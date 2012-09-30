using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BDEditor.Classes
{
    public class BDCommon
    {
        private static volatile BDCommon instance;
        private static object syncRoot = new object();

        private const string MAJICPHRASE = "dontbugme";
        private const string EXTRAMAJIKPHRASE = "xyzzy";

        private Boolean isSyncLoad = false;
        private Boolean syncPushEnabled = false;
        private Boolean repositoryOverwriteEnabled = false;
        private int suspendDrawingCounter = 0;

        private Boolean isUpdating = false;
        private Boolean waitingForEvent = false;  // waiting for validated 

        private BDCommon() { }

        #region Singleton

        static public BDCommon Settings
        {
            get
            {
                if (null == instance)
                {
                    lock (syncRoot)
                    {
                        if (null == instance)
                        {
                            instance = new BDCommon();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        public Boolean IsUpdating
        {
            get { return isUpdating; }
            set { isUpdating = value; }
        }

        public Boolean WaitingForEvent
        {
            get { return waitingForEvent; }
            set { waitingForEvent = value; }
        }

        public Boolean IsSyncLoad
        {
            get { return isSyncLoad; }
            set { isSyncLoad = value; }
        }

        public Boolean Validate(string pMajicPhrase)
        {
            return (pMajicPhrase == MAJICPHRASE) || (pMajicPhrase == EXTRAMAJIKPHRASE);
        }

        public Boolean Authenticate(string pMajicPhrase)
        {
            syncPushEnabled = (pMajicPhrase == MAJICPHRASE) || (pMajicPhrase == EXTRAMAJIKPHRASE);
            repositoryOverwriteEnabled = (pMajicPhrase == EXTRAMAJIKPHRASE);
            return syncPushEnabled;
        }

        public Boolean SyncPushEnabled
        {
            get { return syncPushEnabled; }
        }

        public Boolean RepositoryOverwriteEnabled
        {
            get { return repositoryOverwriteEnabled; }
        }

        public int SuspendDrawingCounter
        {
            get { return suspendDrawingCounter; }
            set { suspendDrawingCounter = value; }
        }
    }
}
