using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public interface IBDControl
    {
        void AssignParentInfo(Guid? pParentId, Constants.BDNodeType pParentType);
        bool Save();
        void Delete();
        bool CreateCurrentObject();
        void RefreshLayout();
        void ShowLinksInUse(bool pPropagateToChildren);
    }
}
