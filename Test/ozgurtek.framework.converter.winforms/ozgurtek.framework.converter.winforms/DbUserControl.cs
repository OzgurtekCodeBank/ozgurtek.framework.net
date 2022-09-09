using System;
using System.Windows.Forms;
using Microsoft.Win32;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;
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

        private void ConnectButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                IGdTable table = GetTable();
                IGdSchema schema = table.Schema;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
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
            GdExt3dModelExportEngine engine = new GdExt3dModelExportEngine();
            engine.Export(GetTable(), OutPutFolderTextBox.Text, DbConvert.ToInt64(EntityPerPageTextBox.Text), DbConvert.ToInt32(EpsgTextBox.Text));
        }

        private void CheckUi()
        {
            if (string.IsNullOrWhiteSpace(EntityPerPageTextBox.Text))
                throw new SystemException("EntityPerPageTextBox Missing");

            if (string.IsNullOrWhiteSpace(OutPutFolderTextBox.Text))
                throw new SystemException("OutPutFolderTextBox Missing");

            if (string.IsNullOrWhiteSpace(EpsgTextBox.Text))
                throw new SystemException("EpsgTextBox Missing");
        }

        private IGdTable GetTable()
        {
            GdPgDataSource dataSource = GdPgDataSource.Open(ConnectionStringText.Text);
            GdSqlFilter filter = new GdSqlFilter(QueryTextBox.Text);
            return dataSource.ExecuteSql("sql", filter);
        }
    }
}