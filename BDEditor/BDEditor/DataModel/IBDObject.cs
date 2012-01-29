using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    public interface IBDObject
    {
        Guid Uuid { get; }
        Constants.BDObjectType NodeType { get; }
        string Description { get; }
        string DescriptionForLinkedNote { get; }
        Amazon.SimpleDB.Model.PutAttributesRequest PutAttributes();
    }
}
