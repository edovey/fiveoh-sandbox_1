using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public class BDException : Exception
    {
        public IBDNode Node { get; set; }

        public BDException()
        {
        }

        public BDException(string message)
            : base(message)
        {
        }

        public BDException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public BDException(string message, IBDNode pNode)
            : base(message)
        {
            Node = pNode;
        }
    }
}
