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
        BDConstants.BDNodeType NodeType { get; }
        BDConstants.LayoutVariantType LayoutVariant { get; set; }
        int? DisplayOrder { get; set; }
        Guid? ParentId { get; }
        BDConstants.BDNodeType ParentType { get; }
        void SetParent(IBDNode pParent);
        void SetParent(BDConstants.BDNodeType pParentType, Guid? pParentId);
    }
}
