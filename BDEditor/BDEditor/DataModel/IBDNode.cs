using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.Classes;

namespace BDEditor.DataModel
{
    public interface IBDNode : IBDObject
    {
        string Name { get; set; }
        Constants.BDNodeType NodeType { get; }
        Constants.LayoutVariantType LayoutVariant { get; set; }
        int? DisplayOrder { get; set; }
        Guid? ParentId { get; }
        Constants.BDNodeType ParentType { get; }
        void SetParent(IBDNode pParent);
        void SetParent(Constants.BDNodeType pParentType, Guid? pParentId);
    }
}
