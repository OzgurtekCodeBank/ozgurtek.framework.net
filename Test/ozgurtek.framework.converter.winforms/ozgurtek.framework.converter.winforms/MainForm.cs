using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.gdal;
using Geometry = NetTopologySuite.Geometries.Geometry;

namespace ozgurtek.framework.converter.winforms
{
    public partial class MainForm : Form
    {
        private string _source = @"C:\Users\eniso\Desktop\work\testdata\test.kmz";

        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> geomsString = new List<string>();

            IEnumerable<IGdTable> tables = GetTable();
            foreach (IGdTable table in tables)
            {
                table.GeometryField = "gd_geom";
                if (table.RowCount == 0)
                    continue;

                string geojson = table.ToGeojson(GdGeoJsonSeralizeType.All, 3);

                foreach (IGdRow row in table.Rows)
                {
                    foreach (IGdParamater paramater in row.Paramaters)
                    {
                        if (row.IsNull(paramater.Name))
                            continue;

                        if (paramater.Value is Geometry geometry)
                        {
                            geomsString.Add(geometry.AsText());
                        }
                    }
                }
            }
        }

        private IEnumerable<IGdTable> GetTable()
        {
            GdOgrDataSource dataSource = GdOgrDataSource.Open(_source);
            return dataSource.GetTable();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            GdFileLogger.Current.LogFolder = strWorkPath;

            dbUserControl.Start();
            fileUserControl.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dbUserControl.End();
            fileUserControl.End();
        }
    }
}