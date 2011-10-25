using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Classes
{
    public class Common
    {
        static private readonly Common settings = new Common();

        private Boolean isSyncLoad = false;

        private Common() { }


        static public Common Settings
        {
            get { return settings; }
        }

        public Boolean IsSyncLoad
        {
            get { return isSyncLoad; }
            set { isSyncLoad = value; }
        }
    }
}
