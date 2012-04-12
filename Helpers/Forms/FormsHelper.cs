using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Helpers.Forms
{
    public class FormsHelper
    {
        public static void ShowExceptionInfo(string message, Exception ex)
        {
            MessageBox.Show(String.Format("{0} {1}{2}{3}", message, ex.Message, Environment.NewLine, ex.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
