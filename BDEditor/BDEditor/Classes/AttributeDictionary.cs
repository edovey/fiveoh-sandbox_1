using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Classes
{
    public class AttributeDictionary: Dictionary<string, string>
    {
        /// <summary>
        /// Add the attribute to the dictionary. Does nothing if the attribute IsSetName() method returns false
        /// </summary>
        /// <param name="pAttribute"></param>
        public void Add(Amazon.SimpleDB.Model.Attribute pAttribute)
        {
            if (pAttribute.IsSetName())
                base.Add(pAttribute.Name, (pAttribute.IsSetValue()) ? pAttribute.Value : null);
        }
    }
}
