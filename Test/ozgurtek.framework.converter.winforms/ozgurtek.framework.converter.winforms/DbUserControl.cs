using System;
using System.Windows.Forms;
using Microsoft.Win32;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.driver.postgres;

namespace ozgurtek.framework.converter.winforms
{
    public partial class DbUserControl : UserControl
    {
        public DbUserControl()
        {
            InitializeComponent();
        }

        public void Start()
        {
            RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ozgurtek.framework.converter.winforms");
            ConnectionStringText.Text = DbConvert.ToString(key.GetValue("connection_string", ""));
            QueryTextBox.Text = DbConvert.ToString(key.GetValue("query", ""));
            OutPutFolderTextBox.Text = DbConvert.ToString(key.GetValue("output_folder", ""));
            EpsgTextBox.Text = DbConvert.ToString(key.GetValue("epsg", ""));
        }

        public void End()
        {
            RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ozgurtek.framework.converter.winforms");
            key.SetValue("connection_string", ConnectionStringText.Text);
            key.SetValue("query", QueryTextBox.Text);
            key.SetValue("output_folder", OutPutFolderTextBox.Text);
            key.SetValue("epsg", EpsgTextBox.Text);
        }

        private void folderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            OutPutFolderTextBox.Text = dialog.SelectedPath;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                CheckUi();
                Export();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Export()
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;

            GdTrack track = new GdTrack();
            track.ProgressChanged += ProgressChanged;

            //connect db and get table
            GdPgDataSource dataSource = GdPgDataSource.Open(ConnectionStringText.Text);
            GdSqlFilter filter = new GdSqlFilter(QueryTextBox.Text);
            GdPgTable table = dataSource.ExecuteSql("sql", filter);

            GdExt3dModelExportEngine engine = new GdExt3dModelExportEngine();
            engine.Export(table, OutPutFolderTextBox.Text, 
                DbConvert.ToInt32(XyTileCountTextBox.Text),
                DbConvert.ToInt32(EpsgTextBox.Text), track);
        }

        private void ProgressChanged(object sender, double e)
        {
            int c = DbConvert.ToInt32(e);
            progressBar.Value = c;
        }

        private void CheckUi()
        {
            if (string.IsNullOrWhiteSpace(ConnectionStringText.Text))
                throw new SystemException("ConnectionStringText Missing");

            if (string.IsNullOrWhiteSpace(QueryTextBox.Text))
                throw new SystemException("QueryTextBox Missing");

            if (string.IsNullOrWhiteSpace(XyTileCountTextBox.Text))
                throw new SystemException("XY Tile Size");

            if (string.IsNullOrWhiteSpace(OutPutFolderTextBox.Text))
                throw new SystemException("OutPutFolderTextBox Missing");

            if (string.IsNullOrWhiteSpace(EpsgTextBox.Text))
                throw new SystemException("EpsgTextBox Missing");
        }
    }
}