using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FhirProfilePublisher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
            Application.Run(new FhirProfilePublisherDialog());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Exception:  " + e.Exception.GetType().FullName + Environment.NewLine
                + Environment.NewLine
                + "Message:  " + e.Exception.Message + Environment.NewLine
                + Environment.NewLine
                + "Stack trace:  " + Environment.NewLine
                + e.Exception.StackTrace,
                "Exception",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

    }
}
