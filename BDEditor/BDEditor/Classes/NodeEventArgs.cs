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

        public NodeEventArgs() { }
        public NodeEventArgs(Entities pDataContext, Guid? pUuid, String pText)
        {
            DataContext = pDataContext;
            Uuid = pUuid;
            Text = pText;
        }
    }
}
