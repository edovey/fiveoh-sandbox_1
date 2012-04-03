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
        void ShowLinksInUse(bool pPropagateToChildren);

        BDConstants.BDNodeType DefaultNodeType;
        BDConstants.LayoutVariantType DefaultLayoutVariantType;
        IBDNode CurrentNode;
        int? DisplayOrder;
        bool ShowAsChild;
    }
}
