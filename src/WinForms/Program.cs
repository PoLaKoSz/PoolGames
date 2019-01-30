using CSharpSnooker.WinForms.Components;
using System;
using System.Windows.Forms;

namespace CSharpSnooker.WinForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GameEngine ge = new GameEngine();

            Application.Run(ge.View);
        }
    }
}