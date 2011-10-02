using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Views
{
    public interface IBDControl
    {
        void AssignDataContext(BDEditor.DataModel.Entities pDataContext);
        void AssignParentId(Guid? pParentId);
        bool Save();
    }
}
