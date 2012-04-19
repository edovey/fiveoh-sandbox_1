using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BDEditor.Classes
{
    /// <summary>
    /// From : http://stackoverflow.com/questions/487661/how-do-i-suspend-painting-for-a-control-and-its-children
    /// </summary>
    public static class ControlHelper
    {
        #region Redraw Suspend/Resume
        [DllImport("user32.dll", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private const int WM_SETREDRAW = 0xB;

        public static void SuspendDrawing(this Control target)
        {
            if(BDCommon.Settings.SuspendDrawingCounter == 0)
                SendMessage(target.Handle, WM_SETREDRAW, 0, 0);
            BDCommon.Settings.SuspendDrawingCounter++;
        }

        public static void ResumeDrawing(this Control target) { ResumeDrawing(target, true); }
        public static void ResumeDrawing(this Control target, bool redraw)
        {
            BDCommon.Settings.SuspendDrawingCounter--;
            if(BDCommon.Settings.SuspendDrawingCounter == 0)
                SendMessage(target.Handle, WM_SETREDRAW, 1, 0);

            if (redraw)
            {
                target.Refresh();
            }
        }
        #endregion
    }
}
