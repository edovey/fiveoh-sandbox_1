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

        private Boolean isSyncLoad = false;

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

        public Boolean IsSyncLoad
        {
            get { return isSyncLoad; }
            set { isSyncLoad = value; }
        }
    }
}
