using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Views
{
    public interface IBDControl
    {
        void AssignParentId(Guid? pParentId);
        bool Save();
        void Delete();
        bool CreateCurrentObject();
        void RefreshLayout();
        void ShowLinksInUse(bool pPropagateToChildren);
    }
}
