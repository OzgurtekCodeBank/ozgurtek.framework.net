using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetTopologySuite.Geometries;

using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.gdal;
using Geometry = NetTopologySuite.Geometries.Geometry;

namespace ozgurtek.framework.converter.winforms
{
    public partial class Form1 : Form
    {
        private string _source = @"C:\Users\eniso\Desktop\work\testdata\test.kmz";

        public Form1()
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

                string geojson = table.ToGeojson(GdGeoJsonSeralizeType.All);

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
    }


}
