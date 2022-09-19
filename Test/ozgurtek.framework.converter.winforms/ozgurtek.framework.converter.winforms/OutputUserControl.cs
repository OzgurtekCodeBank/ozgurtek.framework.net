using System;
using System.Windows.Forms;
using Microsoft.Win32;
using ozgurtek.framework.common.Data;

namespace ozgurtek.framework.converter.winforms
{
    public partial class OutputUserControl : UserControl
    {
        private string _registeryPrefix;
        
        public OutputUserControl()
        {
            InitializeComponent();
        }

        public string RegisteryPrefix
        {
            get => _registeryPrefix;
            set => _registeryPrefix = value;
        }

        public void Start()
        {
            RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ozgurtek.framework.converter.winforms");
            
            XyTileCountTextBox.Text = DbConvert.ToString(key.GetValue(RegisteryPrefix + "XyTileCountTextBox", ""));
            EpsgTextBox.Text = DbConvert.ToString(key.GetValue(RegisteryPrefix + "EpsgTextBox", ""));
            OutPutFolderTextBox.Text = DbConvert.ToString(key.GetValue(RegisteryPrefix + "OutPutFolderTextBox", ""));
            FidFieldTextBox.Text = DbConvert.ToString(key.GetValue(RegisteryPrefix + "FidFieldTextBox", ""));
            GeomFieldTextBox.Text = DbConvert.ToString(key.GetValue(RegisteryPrefix + "GeomFieldTextBox", ""));
            ExtFieldTextBox.Text = DbConvert.ToString(key.GetValue(RegisteryPrefix + "ExtFieldTextBox", ""));
            StyleFieldTextBox.Text = DbConvert.ToString(key.GetValue(RegisteryPrefix + "StyleFieldTextBox", ""));
            DescTextBox.Text = DbConvert.ToString(key.GetValue(RegisteryPrefix + "DescTextBox", ""));
        }

        public void Stop()
        {
            RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ozgurtek.framework.converter.winforms");
            
            key.SetValue(RegisteryPrefix + "XyTileCountTextBox", XyTileCountTextBox.Text);
            key.SetValue(RegisteryPrefix + "EpsgTextBox", EpsgTextBox.Text);
            key.SetValue(RegisteryPrefix + "OutPutFolderTextBox", OutPutFolderTextBox.Text);
            key.SetValue(RegisteryPrefix + "FidFieldTextBox", FidFieldTextBox.Text);
            key.SetValue(RegisteryPrefix + "GeomFieldTextBox", GeomFieldTextBox.Text);
            key.SetValue(RegisteryPrefix + "ExtFieldTextBox", ExtFieldTextBox.Text);
            key.SetValue(RegisteryPrefix + "StyleFieldTextBox", StyleFieldTextBox.Text);
            key.SetValue(RegisteryPrefix + "DescTextBox", DescTextBox.Text);
        }

        private void folderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            OutPutFolderTextBox.Text = dialog.SelectedPath;
        }
    }
}

