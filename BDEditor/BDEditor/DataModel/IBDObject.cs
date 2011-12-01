using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.DataModel
{
    public interface IBDObject
    {
        Guid Uuid { get; }
        string Description { get; }
        string DescriptionForLinkedNote { get; }
        Amazon.SimpleDB.Model.PutAttributesRequest PutAttributes();
    }
}
