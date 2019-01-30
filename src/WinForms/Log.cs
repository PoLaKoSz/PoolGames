using System;

namespace CSharpSnooker.WinForms
{
    static class Log
    {
        public static void Debug(string message, params object[] param)
        {
            message = string.Format(message, param);

            message = string.Format("{0}\t{1}", DateTime.Now.ToString("HH:mm:ss:fff"), message);

            System.Diagnostics.Trace.WriteLine(message);
        }
    }
}
