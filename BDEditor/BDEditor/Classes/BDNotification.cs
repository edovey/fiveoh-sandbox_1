using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Classes
{
    class BDNotification
    {
        public static event EventHandler<BDNotificationEventArgs> Notify = delegate { }; 

        static protected void OnNotification(BDNotificationEventArgs e)
        {
            EventHandler<BDNotificationEventArgs> handler = Notify;
            if (null != handler) { handler(null, e); }
        }

        public static void SendNotification(BDNotificationEventArgs e)
        {
            OnNotification(e);
        }
    }
}
