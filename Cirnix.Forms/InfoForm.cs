﻿using Cirnix.Global;

using MetroFramework.Forms;

using Newtonsoft.Json.Linq;

using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

using static Cirnix.Global.Globals;

namespace Cirnix.Forms
{
    internal sealed partial class InfoForm : MetroForm
    {

        BackgroundWorker VersionChecker;
        internal Action historyForm;
        internal Action licenceForm;
        internal static int[] Current;
        internal static int[] Recommanded;
        internal static int[] Latest;
        internal static string RecommandedURL;
        internal static string LatestURL;
        private bool IsUpdating = false;

        internal InfoForm()
        {
            InitializeComponent();
            Icon = Global.Properties.Resources.CirnixIcon;
            IsUpdating = true;
            Toggle_BetaUser.Checked = Settings.BetaUser;
            IsUpdating = false;
            VersionChecker = new BackgroundWorker();
            VersionChecker.DoWork += new DoWorkEventHandler(VersionChecker_DoWork);
            VersionChecker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(VersionChecker_RunWorkerCompleted);

            Version ver = Assembly.GetEntryAssembly().GetName().Version;
            Current = new int[4] { ver.Major, ver.Minor, ver.Build, ver.Revision };
            Recommanded = new int[4];
            Latest = new int[4];
            CurrentVersion.Text = $"{Current[0]}.{Current[1]}.{Current[2]}.{Current[3]}";
        }

        private void InfoForm_Shown(object sender, EventArgs e)
        {
            UpdateButton.Text = "업데이트";
            UpdateButton.Enabled = false;
            VersionChecker.RunWorkerAsync();
        }

        private void VersionChecker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(infoURL)) LatestVersion.Text = "서버 없음";
            else if (e.Error != null) LatestVersion.Text = "연결 실패";
            else VersionUpdate();
        }

        private void VersionUpdate()
        {
            int[] version = Settings.BetaUser ? Latest : Recommanded;
            LatestVersion.Text = $"{version[0]}.{version[1]}.{version[2]}.{version[3]}";
            for (int i = 0; i < 4; i++)
            {
                if (i != 3 && Current[i] == version[i]) continue;
                if (Current[i] >= version[i])
                {
                    UpdateButton.Text = "최신 버전";
                    UpdateButton.Enabled = false;
                }
                else
                {
                    UpdateButton.Text = "업데이트";
                    UpdateButton.Enabled = true;
                }
                break;
            }
        }

        private void VersionChecker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(infoURL)) return;
            JObject json = JObject.Parse(GetDataFromServer(infoURL));
            string[] latestTemp = json["Recommanded_Version"].ToString().Split('.');
            for (int i = 0; i < 4; i++)
                Recommanded[i] = int.Parse(latestTemp[i]);
            RecommandedURL = json["Recommanded_URL"].ToString();
            latestTemp = json["Latest_Version"].ToString().Split('.');
            for (int i = 0; i < 4; i++)
                Latest[i] = int.Parse(latestTemp[i]);
            LatestURL = json["Latest_URL"].ToString();
        }

        private void Update_Click(object sender, EventArgs e)
        {
            new UpdateForm(Global.Theme.Title, Settings.BetaUser ? LatestURL : RecommandedURL).ShowDialog();
        }

        private void CurrentVersion_Click(object sender, EventArgs e)
        {
            historyForm();
            
        }

        private void Toggle_BetaUser_CheckedChanged(object sender, EventArgs e)
        {
            if (IsUpdating) return;
            Settings.BetaUser = Toggle_BetaUser.Checked;
            if (string.IsNullOrWhiteSpace(infoURL)) LatestVersion.Text = "서버 없음";
            else VersionUpdate();
        }

        private void CopyRight_Click(object sender, EventArgs e)
        {

        }

        private void InfoForm_Load(object sender, EventArgs e)
        {

        }

        private void Picture_Click(object sender, EventArgs e)
        {

        }

        private void LicenceButton_Click(object sender, EventArgs e)
        {
            licenceForm();
        }
    }
}
