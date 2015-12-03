using System;
using System.IO;
using System.Windows.Forms;

namespace Ipsync.View
{
    public partial class LogForm : Form
    {
        private long _lastOffset;
        private readonly string _filename;
        private Timer _timer;
        private const int BACK_OFFSET = 65536;

        public LogForm(string filename)
        {
            this._filename = filename;
            InitializeComponent();
            //this.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());

            UpdateTexts();
        }

        private void UpdateTexts()
        {
            FileMenuItem.Text = @"&File";
            OpenLocationMenuItem.Text = @"&Open Location";
            ExitMenuItem.Text = @"&Exit";
            CleanLogsButton.Text = @"&Clean logs";
            ChangeFontButton.Text = @"&Font";
            WrapTextCheckBox.Text = @"&Wrap text";
            TopMostCheckBox.Text = @"&Top most";
            this.Text = @"Log Viewer";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateContent();
        }

        private void InitContent()
        {
            using (StreamReader reader = new StreamReader(new FileStream(_filename,
                     FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                if (reader.BaseStream.Length > BACK_OFFSET)
                {
                    reader.BaseStream.Seek(-BACK_OFFSET, SeekOrigin.End);
                    reader.ReadLine();
                }

                string line;
                while ((line = reader.ReadLine()) != null)
                    LogMessageTextBox.AppendText(line + "\r\n");

                LogMessageTextBox.ScrollToCaret();

                _lastOffset = reader.BaseStream.Position;
            }
        }

        private void UpdateContent()
        {
            using (StreamReader reader = new StreamReader(new FileStream(_filename,
                     FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                reader.BaseStream.Seek(_lastOffset, SeekOrigin.Begin);

                string line;
                bool changed = false;
                while ((line = reader.ReadLine()) != null)
                {
                    changed = true;
                    LogMessageTextBox.AppendText(line + "\r\n");
                }

                if (changed)
                {
                    LogMessageTextBox.ScrollToCaret();
                }

                _lastOffset = reader.BaseStream.Position;
            }
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
            InitContent();
            _timer = new Timer { Interval = 300 };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timer.Stop();
        }

        private void OpenLocationMenuItem_Click(object sender, EventArgs e)
        {
            string argument = "/select, \"" + _filename + "\"";
            Console.WriteLine(argument);
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LogForm_Shown(object sender, EventArgs e)
        {
            LogMessageTextBox.ScrollToCaret();
        }

        private void WrapTextCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LogMessageTextBox.WordWrap = WrapTextCheckBox.Checked;
            LogMessageTextBox.ScrollToCaret();
        }

        private void CleanLogsButton_Click(object sender, EventArgs e)
        {
            LogMessageTextBox.Clear();
        }

        private void ChangeFontButton_Click(object sender, EventArgs e)
        {
            var fd = new FontDialog { Font = LogMessageTextBox.Font };
            if (fd.ShowDialog() == DialogResult.OK)
            {
                LogMessageTextBox.Font = fd.Font;
            }
        }

        private void TopMostCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = TopMostCheckBox.Checked;
        }
    }
}
