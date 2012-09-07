using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.Classes;
using BDEditor.DataModel;

namespace BDEditor.Views
{
    public interface IBDControl 
    {
        event EventHandler<NodeEventArgs> RequestItemAdd;
        event EventHandler<NodeEventArgs> RequestItemDelete;
        event EventHandler<NodeEventArgs> ReorderToPrevious;
        event EventHandler<NodeEventArgs> ReorderToNext;
        event EventHandler<NodeEventArgs> NotesChanged;
        event EventHandler<NodeEventArgs> NameChanged;

        void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType);
        void AssignDataContext(Entities pDataContext);
        void AssignScopeId(Guid? pScopeId);

        bool Save();
        void Delete();
        bool CreateCurrentObject();
        void RefreshLayout();
        void RefreshLayout(bool pShowChildren);
        void ShowLinksInUse(bool pPropagateToChildren);

        BDConstants.BDNodeType DefaultNodeType {get; set;}
        BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }
        IBDNode CurrentNode { get; set; }
        int? DisplayOrder { get; set; }
        bool ShowAsChild { get; set; }
        bool ShowChildren { get; set; }
    }
}
