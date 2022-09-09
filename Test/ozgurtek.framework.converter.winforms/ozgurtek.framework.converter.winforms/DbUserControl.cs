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
        }

        public void End()
        {
            RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ozgurtek.framework.converter.winforms");
            key.SetValue("connection_string", ConnectionStringText.Text);
            key.SetValue("query", QueryTextBox.Text);
            key.SetValue("output_folder", OutPutFolderTextBox.Text);
        }

        private void ConnectButton_Click(object sender, System.EventArgs e)
        {
            KeyFieldComboBox.Items.Clear();
            GeometryFieldComboBox.Items.Clear();
            ExtFieldComboBox.Items.Clear();

            try
            {
                GdPgDataSource dataSource = GdPgDataSource.Open(ConnectionStringText.Text);
                GdSqlFilter filter = new GdSqlFilter(QueryTextBox.Text);

                GdPgTable table = dataSource.ExecuteSql("sql", filter);
                IGdSchema schema = table.Schema;
                foreach (IGdField field in schema.Fields)
                {
                    KeyFieldComboBox.Items.Add(field);
                    GeometryFieldComboBox.Items.Add(field);
                    ExtFieldComboBox.Items.Add(field);
                }
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
        }

        private void CheckUi()
        {
            if (string.IsNullOrWhiteSpace(KeyFieldComboBox.Text))
                throw new SystemException("Key Field Missing");

            if (string.IsNullOrWhiteSpace(GeometryFieldComboBox.Text))
                throw new SystemException("Geometry Field Missing");

            if (string.IsNullOrWhiteSpace(ExtFieldComboBox.Text))
                throw new SystemException("ExtField Field Missing");

            if (string.IsNullOrWhiteSpace(EntityPerPageTextBox.Text))
                throw new SystemException("EntityPerPageTextBox Missing");

            if (string.IsNullOrWhiteSpace(OutPutFolderTextBox.Text))
                throw new SystemException("OutPutFolderTextBox Missing");
        }
    }
}