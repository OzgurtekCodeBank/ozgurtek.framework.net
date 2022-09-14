using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.common.Mapping;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.driver.postgres;
using ozgurtek.framework.driver.sqlite;

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

            GdPgDataSource dataSource = GdPgDataSource.Open(ConnectionStringText.Text);
            GdSqlFilter filter = new GdSqlFilter(QueryTextBox.Text);
            GdPgTable gdPgTable = dataSource.ExecuteSql("sql", filter);
            gdPgTable.GeometryField = "gd_geometry";
            Envelope envelope = new Envelope(587404.656000137, 605976.384, 4519140.6319  , 4532563.38566273);
            Envelope project = GdProjection.Project(envelope, DbConvert.ToInt32(EpsgTextBox.Text), 4326);
            List<GdTileIndex> tileIndices = Divide(project, project.Width / 10, project.Height / 10);

            string path = Path.Combine(OutPutFolderTextBox.Text, "index.sqlite");
            GdSqlLiteDataSource sqlLiteDataSource = GdSqlLiteDataSource.OpenOrCreate(path);
            GdSqlLiteTable table = sqlLiteDataSource.CreateTable("gd_index", null, null, null);
            table.CreateField(new GdField("min_x", GdDataType.Real));
            table.CreateField(new GdField("min_y", GdDataType.Real));
            table.CreateField(new GdField("max_x", GdDataType.Real));
            table.CreateField(new GdField("max_y", GdDataType.Real));

            foreach (GdTileIndex index in tileIndices)
            {
                GdRowBuffer buffer = new GdRowBuffer();
                buffer.Put("min_x", index.Envelope.MinX);
                buffer.Put("min_y", index.Envelope.MinY);
                buffer.Put("max_x", index.Envelope.MaxX);
                buffer.Put("max_y", index.Envelope.MaxY);
                table.Insert(buffer);
            }

            GdExt3dModelExportEngine engine = new GdExt3dModelExportEngine();
            engine.Export(gdPgTable, OutPutFolderTextBox.Text, tileIndices, DbConvert.ToInt32(EpsgTextBox.Text), track);
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

            if (string.IsNullOrWhiteSpace(EntityPerPageTextBox.Text))
                throw new SystemException("EntityPerPageTextBox Missing");

            if (string.IsNullOrWhiteSpace(OutPutFolderTextBox.Text))
                throw new SystemException("OutPutFolderTextBox Missing");

            if (string.IsNullOrWhiteSpace(EpsgTextBox.Text))
                throw new SystemException("EpsgTextBox Missing");
        }

        
        public List<GdTileIndex> Divide(Envelope viewport, double tileWidth, double tileHeight)
        {
            List<GdTileIndex> worldArray = new List<GdTileIndex>();

            long xindex = 0;
            long yindex = 0;
            for (double x = viewport.MinX; x < viewport.MaxX; x+= tileWidth)
            {
                for (double y = viewport.MinY; y < viewport.MaxY; y+= tileHeight)
                {
                    GdTileIndex index = new GdTileIndex(xindex, yindex);
                    Coordinate coordinateMin = new Coordinate(x, y);
                    Coordinate coordinateMax = new Coordinate(x + tileWidth, y + tileHeight);
                    Envelope env = new Envelope(coordinateMin, coordinateMax);
                    index.Envelope = env;
                    worldArray.Add(index);
                    yindex++;
                }

                xindex++;
            }

            return worldArray;
        }
    }
}