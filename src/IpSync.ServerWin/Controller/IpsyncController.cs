
using System;
using Ipsync.Model;

namespace Ipsync.Controller
{
    public class IpsyncController
    {
        private Configuration _config;
        public void Start()
        {
            Reload();
        }

        public void Reload()
        {
            _config = Configuration.Load();
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
    }
}
