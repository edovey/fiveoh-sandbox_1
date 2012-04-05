using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public class BDNotificationEventArgs : EventArgs
    {
        public enum BDNotificationType
        {
            Refresh = 0,
            Addition = 1,
            Deletion = 2,
        }

        public Entities DataContext { get; set; }
        public Guid? Uuid { get; set; }

        public BDNotificationType NotificationType { get; set; }
        public BDConstants.BDNodeType NodeType { get; set; }
        public BDConstants.LayoutVariantType LayoutVariant { get; set; }

        public BDNotificationEventArgs(BDNotificationType pNotificationType) { }

        public BDNotificationEventArgs(BDNotificationType pNotificationType, Entities pDataContext, Guid? pUuid)
        {
            NotificationType = pNotificationType;
            DataContext = pDataContext;
            Uuid = pUuid;
        }

        public BDNotificationEventArgs(BDNotificationType pNotificationType, Entities pDataContext, BDConstants.BDNodeType pNodeType, BDConstants.LayoutVariantType pLayoutVariant, Guid? pUuid)
        {
            NotificationType = pNotificationType;
            DataContext = pDataContext;
            NodeType = pNodeType;
            LayoutVariant = pLayoutVariant;
            Uuid = pUuid;
        }
    }
}
