using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CSharpSnookerUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.2
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmTable());
        }
    }
}