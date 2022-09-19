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
            outputUserControl.RegisteryPrefix = "Db";
        }

        public void Start()
        {
            RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ozgurtek.framework.converter.winforms");
            ConnectionStringText.Text = DbConvert.ToString(key.GetValue("connection_string", ""));
            QueryTextBox.Text = DbConvert.ToString(key.GetValue("query", ""));
            outputUserControl.Start();
        }

        public void End()
        {
            RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ozgurtek.framework.converter.winforms");
            key.SetValue("connection_string", ConnectionStringText.Text);
            key.SetValue("query", QueryTextBox.Text);
            outputUserControl.Stop();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                CheckUi();
                Export();
                
                MessageBox.Show("Finish...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message ,"Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            //GdExtrudedModelExportEngine engine = new GdExtrudedModelExportEngine();
            //engine.Export(table, outputUserControl.OutPutFolderTextBox.Text,
            //    DbConvert.ToInt32(outputUserControl.XyTileCountTextBox.Text),
            //    DbConvert.ToInt32(outputUserControl.EpsgTextBox.Text),
            //    DbConvert.ToBoolean(outputUserControl.SuppressBlankTileCheck.Checked),
            //    track);
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
        }
    }
}