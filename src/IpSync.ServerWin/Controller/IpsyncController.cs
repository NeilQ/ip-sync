
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ipsync.Model;
using Simple.Json;

namespace Ipsync.Controller
{
    public class IpsyncController
    {
        private string _currentIp;
        private Configuration _config;

        private Task _syncTask;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();


        public void Start()
        {
            Reload();
        }

        public void Reload()
        {
            _config = Configuration.Load();
            if (!_config.Enabled) return;
            if (_syncTask == null)
            {
                _syncTask = new Task(() => { SyncProcess(_cancellationTokenSource.Token); },
                    _cancellationTokenSource.Token);
                _syncTask.Start();
            }
            else
            {
                _cancellationTokenSource.Cancel();
                _syncTask.Start();
            }
        }

        public void Stop()
        {
            if (_syncTask != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private void SyncProcess(CancellationToken ct)
        {
            if (_config == null) return;
            if (!Directory.Exists(_config.DropbopxPath)) return;
            var ipsyncFolder = $"{_config.DropbopxPath}\\ipsync-{Environment.MachineName}";
            var ipsyncFile = $"{ipsyncFolder}\\ip.txt";
            if (!Directory.Exists(ipsyncFolder)) Directory.CreateDirectory(ipsyncFolder);

            while (true)
            {
                ct.ThrowIfCancellationRequested();
                var newIp = RequestIp();
                if (string.IsNullOrEmpty(newIp))
                {
                    Thread.Sleep(1000);
                    continue;
                }
                if (newIp != _currentIp)
                {
                    _currentIp = newIp;
                    File.WriteAllText(ipsyncFile, $"{newIp}{Environment.NewLine}{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                }
            }
        }

        private string RequestIp()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var content = client.DownloadString(Constants.IP_API_URL);
                    var ipModel = JsonSerializer.Default.ParseJson<IpModel>(content);
                    var ip = ipModel?.Ip;
                    return ip;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected void SaveConfig(Configuration newConfig)
        {
            Configuration.Save(newConfig);
            Reload();
        }

        public void ToggoleEnable(bool enabled)
        {
            _config.Enabled = enabled;
            SaveConfig(_config);
        }

        public void SaveDropboxPath(string path)
        {
            _config.DropbopxPath = path;
            SaveConfig(_config);
        }

        public class IpModel
        {
            public string Ip { get; set; }
        }
    }
}
