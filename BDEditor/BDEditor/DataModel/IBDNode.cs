using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.Classes;

namespace BDEditor.DataModel
{
    public interface IBDNode
    {
        Constants.BDNodeType NodeType { get; }
    }
}
