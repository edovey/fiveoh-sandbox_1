using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Classes
{
    public class BDArchiveRecord : IComparable
    {
        public string Key { get; set; }
        public DateTime CreateDate { get; set; }
        public string Comment { get; set; }
        public string Filename { get; set; }
        public string Username { get; set; }

        public BDArchiveRecord() { }

        public int CompareTo(object obj)
        {
            if (obj is BDArchiveRecord)
            {
                BDArchiveRecord temp = (BDArchiveRecord)obj;

                return temp.CreateDate.CompareTo(this.CreateDate);
            }

            throw new ArgumentException("object is not a BDArchiveRecord");
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", CreateDate.ToString("ddd yyy MMM dd, HH:mm:ss"), Comment);
        }
    }
}
