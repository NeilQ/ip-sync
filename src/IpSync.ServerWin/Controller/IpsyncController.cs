
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
        private CancellationTokenSource _cancellationTokenSource;


        public void Start()
        {
            Reload();
        }

        public void Reload()
        {
            _config = Configuration.Load();

            if (_syncTask != null && !_syncTask.IsCanceled && !_syncTask.IsCompleted)
            {
                _cancellationTokenSource.Cancel();
            }

            if (!_config.Enabled) return;

            _cancellationTokenSource = new CancellationTokenSource();
            _syncTask = new Task(() => { SyncProcess(_cancellationTokenSource.Token); },
                   _cancellationTokenSource.Token);
            _syncTask.Start();
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
            if (string.IsNullOrEmpty(_config?.DropbopxPath))
            {
                Logging.Log("Error: Dropbox path not configured.");
                return;
            }
            if (!Directory.Exists(_config.DropbopxPath))
            {
                Logging.Log("Error: Dropbox path not exists.");
                return;
            };
            var ipsyncFolder = $"{_config.DropbopxPath}\\ipsync-{Environment.MachineName}";
            var ipsyncFile = $"{ipsyncFolder}\\ip.txt";
            if (!Directory.Exists(ipsyncFolder)) Directory.CreateDirectory(ipsyncFolder);

            Logging.Log("Started.");
            while (true)
            {
                try
                {
                    ct.ThrowIfCancellationRequested();
                    var newIp = RequestIp();
                    if (string.IsNullOrEmpty(newIp))
                    {
                        Thread.Sleep(_config.DelaySeconds * 1000);
                        continue;
                    }
                    if (newIp != _currentIp)
                    {
                        Logging.Log($"ip: {newIp}");
                        _currentIp = newIp;
                        File.WriteAllText(ipsyncFile,
                            $"{newIp}{Environment.NewLine}{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }
                }
                catch (OperationCanceledException)
                {
                    Logging.Log("Stopped");
                    _currentIp = string.Empty;
                    return;
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

        public void SaveConfig(string path, int delaySeconds)
        {
            _config.DropbopxPath = path;
            _config.DelaySeconds = delaySeconds;
            SaveConfig(_config);
        }

        public class IpModel
        {
            public string Ip { get; set; }
        }
    }
}
