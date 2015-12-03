using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Ipsync.Controller;
using Ipsync.Model;
using Ipsync.Properties;
using Ipsync.Util;

namespace Ipsync.View
{
    public class MenuViewController
    {
        private IpsyncController _controller;

        private ContextMenu _contenxMenu;
        private NotifyIcon _notifyIcon;

        private MenuItem _enableItem;
        private MenuItem _configItem;
        private MenuItem _autoStartupItem;
        private MenuItem _aboutItem;
        private MenuItem _exitItem;
        private MenuItem _showLogsItem;

        private ConfigForm configForm;

        public MenuViewController(IpsyncController controller)
        {
            _controller = controller;
            LoadMenu();
            var config = Configuration.Load();

            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                ContextMenu = _contenxMenu
            };
            UpdateTrayIcon(config);
            _notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;

            LoadConfiguration(config);
            UpdateTrayText(config);

            controller.ConfigChanged += Controller_ConfigChanged;
            controller.IpChanged += Controller_IpChanged;
        }

        private void Controller_IpChanged(object sender, EventArgs e)
        {
            UpdateTrayText(Configuration.Load());
        }

        private void Controller_ConfigChanged(object sender, EventArgs e)
        {
            UpdateTrayText(Configuration.Load());
        }

        private void LoadConfiguration(Configuration config)
        {
            _enableItem.Checked = config.Enabled;
            _autoStartupItem.Checked = AutoStartup.Check();
            if (!config.Initialized || string.IsNullOrEmpty(config.DropbopxPath))
            {
                ShowConfigForm();
            }
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowConfigForm();
            }
        }

        private void LoadMenu()
        {
            _contenxMenu = new ContextMenu(new[]
            {
                _enableItem = CreateMenuItem("Enable Sync", Enable_click),
                new MenuItem("-"),
                _configItem = CreateMenuItem("Config", Config_click),
                _autoStartupItem = CreateMenuItem("Start on Boot", AutoStartup_click),
                _showLogsItem = CreateMenuItem("Show logs...", ShowLogs_click),
                new MenuItem("-"),
                _aboutItem = CreateMenuItem("About...", About_click),
                _exitItem = CreateMenuItem("Exit", Exit_click)
            });
        }

        private static MenuItem CreateMenuItem(string text, EventHandler click)
        {
            return new MenuItem(text, click);
        }

        private void UpdateTrayIcon(Configuration config)
        {
            var icon = Resources.icon76;
            if (!config.Enabled)
            {
                var iconCopy = new Bitmap(icon);
                for (var x = 0; x < iconCopy.Width; x++)
                {
                    for (var y = 0; y < iconCopy.Height; y++)
                    {
                        var color = icon.GetPixel(x, y);
                        iconCopy.SetPixel(x, y, Color.FromArgb((byte)(color.A / 1.25), color.R, color.G, color.B));
                    }
                }
                icon = iconCopy;
            }
            _notifyIcon.Icon = Icon.FromHandle(icon.GetHicon());
        }

        private void UpdateTrayText(Configuration config)
        {
            var ip = string.IsNullOrEmpty(_controller.CurrentIp) ? "loading..." : _controller.CurrentIp;
            _notifyIcon.Text = $"Ipsync \n{(config.Enabled ? ("Stared: " + ip) : "Stopped")}";
        }

        private void ShowConfigForm()
        {
            if (configForm != null)
            {
                configForm.Activate();
            }
            else
            {
                configForm = new ConfigForm(_controller);
                configForm.Show();
                configForm.FormClosed += configForm_FormClosed;
            }
        }

        private void configForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            configForm = null;
            Utils.ReleaseMemory(true);
        }

        private void Enable_click(object sender, EventArgs e)
        {
            _enableItem.Checked = !_enableItem.Checked;
            _controller.ToggoleEnable(_enableItem.Checked);
            UpdateTrayIcon(Configuration.Load());
        }

        private void ShowLogs_click(object sender, EventArgs e)
        {
            new LogForm(Logging.LogFile).Show();
        }

        private void Exit_click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Config_click(object sender, EventArgs e)
        {
            ShowConfigForm();
        }

        private void AutoStartup_click(object sender, EventArgs e)
        {
            _autoStartupItem.Checked = !_autoStartupItem.Checked;
            if (!AutoStartup.Set(_autoStartupItem.Checked))
            {
                MessageBox.Show("Failed to update registry");
            }
        }

        private void About_click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/NeilQ/ip-sync");
        }

    }
}