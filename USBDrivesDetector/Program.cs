using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace USBDrivesDetector
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // get the name of our process
            string proc = Process.GetCurrentProcess().ProcessName;
            // get the list of all processes by that name
            Process[] processes = Process.GetProcessesByName(proc);
            // if there is more than one process...
            if (processes.Length > 1)
            {
                MessageBox.Show("The agent '" + UsbDetectorForm.APP_TITLE + "' can be launched only once!");
                Application.Exit();
            } else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new UsbDetectorForm());
            }
        }
    }
}
