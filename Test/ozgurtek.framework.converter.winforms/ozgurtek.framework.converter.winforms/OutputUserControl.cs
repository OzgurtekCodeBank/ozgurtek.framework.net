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
            SuppressBlankTileCheck.Checked = DbConvert.ToBoolean(key.GetValue(RegisteryPrefix + "SuppressBlankTileCheck", false));
            TileTypeComboBox.SelectedIndex = DbConvert.ToInt32(key.GetValue(RegisteryPrefix + "TileTypeComboBox", 0));
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
            key.SetValue(RegisteryPrefix + "SuppressBlankTileCheck", SuppressBlankTileCheck.Checked);
            key.SetValue(RegisteryPrefix + "TileTypeComboBox", TileTypeComboBox.SelectedIndex);
        }

        private void folderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            OutPutFolderTextBox.Text = dialog.SelectedPath;
        }

        public void SetParameters(GdExtrudedModelExportEngine engine)
        {
            engine.EpsgCode = DbConvert.ToInt32(EpsgTextBox.Text);
            engine.SuppressBlankTile = DbConvert.ToBoolean(SuppressBlankTileCheck.Checked);
            engine.FidFieldName = FidFieldTextBox.Text;
            engine.GeomFieldName = GeomFieldTextBox.Text;
            engine.StyleFieldName = StyleFieldTextBox.Text;
            engine.DescFieldName = DescTextBox.Text;
            engine.OutputFolder = OutPutFolderTextBox.Text;
            engine.ExtFieldName = ExtFieldTextBox.Text;
            if (TileTypeComboBox.SelectedIndex == 0)
                engine.XyTileCount = DbConvert.ToInt32(XyTileCountTextBox.Text);
            else
                engine.TileSizeInMeter = DbConvert.ToDouble(XyTileCountTextBox.Text);
        }
    }
}

