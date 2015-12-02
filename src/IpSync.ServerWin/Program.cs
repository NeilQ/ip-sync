using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Ipsync.Controller;
using Ipsync.Util;
using Ipsync.View;

namespace Ipsync
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Utils.ReleaseMemory(true);
            using (var mutex = new Mutex(false, "Global\\ipsync_" + Application.StartupPath.GetHashCode()))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (!mutex.WaitOne(0, false))
                {
                    var oldProcesses = Process.GetProcessesByName(Constants.APP_NAME);
                    if (oldProcesses.Length > 0)
                    {
                        var oldProcess = oldProcesses[0];
                    }
                    MessageBox.Show("Find Ipsync icon in your notify tray." + "\n" +
                        "If you want to start multiple Ipsync, make a copy in another directory.",
                        "Ipsync is already running.");
                    return;
                }
                Directory.SetCurrentDirectory(Application.StartupPath);

                //#if !DEBUG
                Logging.OpenLogFile();
                //#endif

                var controller = new IpsyncController();
                var viewController = new MenuViewController(controller);

                controller.Start();
                Application.Run();
            }
        }
    }
}
