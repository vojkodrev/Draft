using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace Helpers.Diagnostics
{
    public class TH
    {
        public static void Message(string message)
        {
            Trace.WriteLine(String.Format("{0} - {1}", DateTime.Now, message));
        }
        public static void Warning(string message)
        {
            Message("WARNING - " + message);
        }
        public static void Exception(Exception e)
        {
            Message(String.Format("EXCEPTION - {0} - {1}", e.GetType(), e.Message));
        }
    }
}