using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BDEditor.Classes
{
    public class Constants
    {
        public enum BDObjectType
        {
            None = -1,
            BDChapter = 1,
            BDSection = 2,
            BDCategory = 3,
            BDSubCategory = 4,
            BDDisease = 5,
            BDPresentation = 6,
            BDPathogenGroup = 7,
            BDPathogen = 8,
            BDTherapyGroup = 9,
            BDTherapy = 10,
            BDLinkedNote = 101
        }

        public const string DATETIMEFORMAT = @"yyyy-MM-dd'T'HH:mm:ss";
        public static readonly Color ACTIVELINK_COLOR = Color.LightSteelBlue;
        public static readonly Color INACTIVELINK_COLOR = SystemColors.Control;

        private Constants() { }

    }
}
