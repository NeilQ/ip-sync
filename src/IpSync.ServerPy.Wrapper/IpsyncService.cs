using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace IpSync.ServerPy.Wrapper
{
    public partial class IpsyncService : ServiceBase
    {
        public IpsyncService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Process.Start("ipsync.exe");
        }

        protected override void OnStop()
        {
            var processes = Process.GetProcessesByName("ipsync");
            if (processes.Length == 1)
            {
                try
                {
                    processes[0].Kill();
                    processes[0].WaitForExit();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}
