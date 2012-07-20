using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public class NodeEventArgs: EventArgs
    {
        public Entities DataContext { get; set; }
        public Guid? Uuid { get; set; }
        public string Text { get; set; }
        public string ContextInfo { get; set; }

        public BDConstants.BDNodeType NodeType { get; set; }
        public BDConstants.LayoutVariantType LayoutVariant { get; set; }

        public NodeEventArgs() { }

        public NodeEventArgs(Entities pDataContext, Guid? pUuid)
        {
            DataContext = pDataContext;
            Uuid = pUuid;
        }

        public NodeEventArgs(Entities pDataContext, Guid? pUuid, String pText)
        {
            DataContext = pDataContext;
            Uuid = pUuid;
            Text = pText;
        }

        public NodeEventArgs(Entities pDataContext, Guid? pUuid, String pText, string pContextInfo)
        {
            DataContext = pDataContext;
            Uuid = pUuid;
            Text = pText;
            ContextInfo = pContextInfo;
        }

        public NodeEventArgs(Entities pDataContext, BDConstants.BDNodeType pNodeType, BDConstants.LayoutVariantType pLayoutVariant)
        {
            DataContext = pDataContext;
            NodeType = pNodeType;
            LayoutVariant = pLayoutVariant;
        }
    }
}
