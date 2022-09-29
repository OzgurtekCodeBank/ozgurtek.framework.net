using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.driver.gdal;

namespace ozgurtek.framework.converter.winforms
{
    public partial class FileUserControl : UserControl
    {
        public FileUserControl()
        {
            InitializeComponent();
            outputUserControl.RegisteryPrefix = "gdal";
        }

        public void Start()
        {
            RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ozgurtek.framework.converter.winforms");
            
            ConnectionStringText.Text = DbConvert.ToString(key.GetValue("file_connection_string", ""));
            outputUserControl.Start();
        }

        public void End()
        {
            RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ozgurtek.framework.converter.winforms");
            key.SetValue("file_connection_string", ConnectionStringText.Text);
            
            outputUserControl.Stop();
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
                return;

            ConnectionStringText.Text = openFileDialog.FileName;
        }

        private void FolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                ConnectionStringText.Text = dialog.SelectedPath;
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
                MessageBox.Show(exception.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Export()
        {
            GdTrack track = new GdTrack();
            track.ProgressChanged += ProgressChanged;

            string fileName = ConnectionStringText.Text;
            GdOgrDataSource dataSource = GdOgrDataSource.Open(fileName);

            progressBar.Minimum = 1;
            progressBar.Maximum = dataSource.TableCount;

            int current = 1;
            IEnumerable<GdOgrTable> table = dataSource.GetTable();
            foreach (GdOgrTable ogrTable in table)
            {
                string path = Path.Combine(outputUserControl.OutPutFolderTextBox.Text, ogrTable.Name);
                Directory.CreateDirectory(path);

                try
                {
                    GdExtrudedModelExportEngine engine = new GdExtrudedModelExportEngine();
                    outputUserControl.SetParameters(engine);
                    engine.OutputFolder = path;

                    engine.Export(ogrTable, null);
                }
                catch (Exception e)
                {
                    GdFileLogger.Current.Log($"writting {ogrTable.Name} ....", LogType.Info);
                    GdFileLogger.Current.LogException(e);
                }
                track.ReportProgress(current++);
            }
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
        }
    }
}
