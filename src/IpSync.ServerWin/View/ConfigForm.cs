using System;
using System.Windows.Forms;
using Ipsync.Controller;
using Ipsync.Model;

namespace Ipsync.View
{
    public partial class ConfigForm : Form
    {
        private IpsyncController _controller;

        public ConfigForm(IpsyncController controller)
        {
            _controller = controller;
            InitializeComponent();

            LoadConfiguration();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            _controller.SaveConfig(txtPath.Text, (int)DelaySecondsNum.Value);
            Close();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderDialog.SelectedPath;
            }
        }

        private void LoadConfiguration()
        {
            var config = Configuration.Load();
            txtPath.Text = config.DropbopxPath;
            DelaySecondsNum.Value = config.DelaySeconds;
        }
    }
}
