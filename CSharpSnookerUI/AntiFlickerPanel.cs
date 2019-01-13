using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnookerUI
{
    class AntiFlickerPanel : System.Windows.Forms.Panel
    {
        public AntiFlickerPanel()
        {
            SetStyle(System.Windows.Forms.ControlStyles.DoubleBuffer, true);
            SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);
        }
    }
}
