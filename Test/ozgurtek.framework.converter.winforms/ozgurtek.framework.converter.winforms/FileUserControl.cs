using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.gdal;

namespace ozgurtek.framework.converter.winforms
{
    public partial class FileUserControl : UserControl
    {
        private const string Id = "gd_id";
        private const string Geometry = "gd_geometry";
        private const string Height = "gd_ext_height";
        private const string Style = "gd_style";
        private const string Description = "gd_description";

        public FileUserControl()
        {
            InitializeComponent();
        }

        public void Start()
        {
            RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ozgurtek.framework.converter.winforms");
            ConnectionStringText.Text = DbConvert.ToString(key.GetValue("file_connection_string", ""));
            OutPutFolderTextBox.Text = DbConvert.ToString(key.GetValue("file_output_folder", ""));
            EpsgTextBox.Text = DbConvert.ToString(key.GetValue("file_epsg", ""));
        }

        public void End()
        {
            RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ozgurtek.framework.converter.winforms");
            key.SetValue("file_connection_string", ConnectionStringText.Text);
            key.SetValue("file_output_folder", OutPutFolderTextBox.Text);
            key.SetValue("file_epsg", EpsgTextBox.Text);
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "kmz files (*.kmz)|*.kmz|kml files (*.kml)|*.kml|city gml  files 3.0(*.gml)|*.gml";
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
                return;

            ConnectionStringText.Text = openFileDialog.FileName;
        }

        private void folderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            OutPutFolderTextBox.Text = dialog.SelectedPath;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            GdTrack track = new GdTrack();
            GdExtrudedModelExportEngine engine = new GdExtrudedModelExportEngine();
            string fileName = ConnectionStringText.Text;
            GdOgrDataSource dataSource = GdOgrDataSource.Open(fileName);
            IEnumerable<GdOgrTable> table = dataSource.GetTable();
            foreach (GdOgrTable ogrTable in table)
            {
                string path = Path.Combine(OutPutFolderTextBox.Text, ogrTable.Name);
                Directory.CreateDirectory(path);
                GdMemoryTable gdMemoryTable = GetTable(ogrTable);

                engine.Export(gdMemoryTable, path,
                    DbConvert.ToInt32(XyTileCountTextBox.Text),
                    DbConvert.ToInt32(EpsgTextBox.Text),
                    DbConvert.ToBoolean(SuppressBlankTileCheck.Checked),
                    track);
            }
        }

        private GdMemoryTable GetTable(GdOgrTable table)
        {
            GdMemoryTable memTable = new GdMemoryTable();
            memTable.Name = table.Name;
            memTable.CreateField(new GdField(Id, GdDataType.Integer));
            memTable.CreateField(new GdField(Geometry, GdDataType.Geometry));
            //memTable.CreateField(new GdField(Height, GdDataType.Real));
            //memTable.CreateField(new GdField(Style, GdDataType.Real));
            //memTable.CreateField(new GdField(Description, GdDataType.Real));
            
            foreach (IGdRow row in table.Rows)
            {   
                GdRowBuffer buffer = new GdRowBuffer();
                buffer.Put(Geometry, row.GetAsGeometry("gd_geom"));
                buffer.Put(Id, row.GetAsInteger("gd_fid"));
                memTable.Insert(buffer);
            }

            return memTable;
        }
    }
}
