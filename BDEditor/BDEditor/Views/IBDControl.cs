using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public interface IBDControl
    {
        void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType);
        bool Save();
        void Delete();
        bool CreateCurrentObject();
        void RefreshLayout();
        void ShowLinksInUse(bool pPropagateToChildren);

        event EventHandler<NodeEventArgs> RequestItemAdd;
        event EventHandler<NodeEventArgs> RequestItemDelete;
        event EventHandler<NodeEventArgs> ReorderToPrevious;
        event EventHandler<NodeEventArgs> ReorderToNext;
        event EventHandler<NodeEventArgs> NotesChanged;
        event EventHandler<NodeEventArgs> NameChanged;
    }
}
