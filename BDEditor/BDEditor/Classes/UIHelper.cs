using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BDEditor.Classes
{
    public class UIHelper
    {

        static public void InsertText(System.Windows.Forms.TextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }
    }


}
