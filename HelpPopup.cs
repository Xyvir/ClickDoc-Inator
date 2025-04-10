﻿
using System.Reflection;


namespace Better_Steps_Recorder
{
    public partial class HelpPopup : Form
    {
        public HelpPopup()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/Xyvir/ClickDoc-Inator",
                UseShellExecute = true
            });
        }

        private void button_CloseHelp_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version != null ? version.ToString() : "Unknown Version";
        }

        private void HelpPopup_Load(object sender, EventArgs e)
        {
            VersionLabel.Text = $"Version: {GetVersion()}";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
