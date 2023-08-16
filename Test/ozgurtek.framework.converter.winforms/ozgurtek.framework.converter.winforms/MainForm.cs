using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common;
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            //GdOgrDataSource dataSource = GdOgrDataSource.Open("C:\\Users\\eniso\\Desktop\\abc.xml");
            //IEnumerable<GdOgrTable> table = dataSource.GetTable();
            //foreach (GdOgrTable ogrTable in table)
            //{
            //    try
            //    {
            //        string geojson = ogrTable.ToGeojson(GdGeoJsonSeralizeType.All);
            //        File.WriteAllText("C:\\Users\\eniso\\Desktop\\json\\" + ogrTable.Name + ".json", geojson);
            //    }
            //    catch (Exception exception)
            //    {
            //        Console.WriteLine(exception);
            //    }
                
            //}
            abc();
        }

        public static GdMemoryTable LoadFromTable(GdOgrTable table, bool geom)
        {
            GdMemoryTable result = new GdMemoryTable();

            //todo: aşağıdakilerden emin değilim.
            //result.Name = table.Name;
            //result.KeyField = table.KeyField;
            //result.GeometryField = table.GeometryField;
            //result.Description = table.Description;

            IGdSchema schema = table.Schema;
            foreach (IGdField otherSchemaField in schema.Fields)
                result.CreateField(otherSchemaField);

            result.CreateField(new GdField("gd_geom", GdDataType.Geometry));
            result.GeometryField = "gd_geom";
            foreach (IGdRow row in table.Rows)
            {
                try
                {
                    GdRowBuffer buffer = new GdRowBuffer(row);
                    if (geom)
                    {
                        Geometry asGeometry = row.GetAsGeometry("gd_geom");
                        buffer.Put("gd_geom", asGeometry);
                    }

                    result.Insert(buffer);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }


            return result;
        }

        private void abc()
        {
            try
            {
                IEnumerable<GdOgrTable> tables;
                GdOgrDataSource dataSource = GdOgrDataSource.Open("C:\\Users\\eniso\\Desktop\\asd.xml");
                tables = dataSource.GetTable();

                GdOgrTable routeTableOgr = tables.ElementAt(38);
                GdOgrTable segmentsTableOgr = tables.ElementAt(37);
                GdOgrTable designatedTableOgr = tables.ElementAt(14);
                GdOgrTable navaidTableOgr = tables.ElementAt(25);

                GdMemoryTable routeTable = LoadFromTable(routeTableOgr, false);
                GdMemoryTable segmentsTable = LoadFromTable(segmentsTableOgr, false);
                GdMemoryTable designatedTable = LoadFromTable(designatedTableOgr, true);
                GdMemoryTable navaidTable = LoadFromTable(navaidTableOgr, true);

                GeometryFactory fact = GdFactoryFinder.Instance.GeometryServices.CreateGeometryFactory(4326);

                GdMemoryTable resultTable = new GdMemoryTable();
                resultTable.Name = "route";
                resultTable.CreateField(new GdField("geometry", GdDataType.Geometry));
                resultTable.CreateField(new GdField("gd_fid", GdDataType.Integer));
                foreach (IGdField paramater in routeTable.Schema.Fields)
                {
                    resultTable.CreateField(paramater);
                }

                foreach (IGdRow row in routeTable.Rows)
                {
                    string routeId = row.GetAsString("gml_id");
                    segmentsTable.SqlFilter = new GdSqlFilter("routeFormed_href='#" + routeId + "'");
                    List<LineString> lineArray = new List<LineString>();

                    foreach (IGdRow segmentsTableRow in segmentsTable.Rows)
                    {
                        string startId;
                        string endId;
                        Geometry startGeo = null;
                        Geometry endGeo = null;

                        endId = segmentsTableRow.GetAsString("timeSlice|RouteSegmentTimeSlice|end|EnRouteSegmentPoint|pointChoice_fixDesignatedPoint_href");
                        startId = segmentsTableRow.GetAsString("pointChoice_fixDesignatedPoint_href");

                        navaidTable.SqlFilter = new GdSqlFilter("gml_id='" + endId.Replace("#", "") + "'");
                        IGdRow navaRow = navaidTable.Rows.FirstOrDefault();
                        if (navaRow != null)
                        {
                            endGeo = navaRow.GetAsGeometry("gd_geom");
                        }
                        designatedTable.SqlFilter = new GdSqlFilter("gml_id='" + endId.Replace("#", "") + "'");
                        IGdRow desigantedRow = designatedTable.Rows.FirstOrDefault();
                        if (desigantedRow != null)
                        {
                            endGeo = desigantedRow.GetAsGeometry("gd_geom");
                        }

                        designatedTable.SqlFilter = new GdSqlFilter("gml_id='" + startId.Replace("#", "") + "'");
                        IGdRow designatedStartRow = designatedTable.Rows.FirstOrDefault();

                        if (designatedStartRow != null)
                        {
                            startGeo = designatedStartRow.GetAsGeometry("gd_geom");
                        }

                        navaidTable.SqlFilter = new GdSqlFilter("gml_id='" + startId.Replace("#", "") + "'");
                        IGdRow navaStartRow = navaidTable.Rows.FirstOrDefault();

                        if (navaStartRow != null)
                        {
                            startGeo = navaStartRow.GetAsGeometry("gd_geom");
                        }

                        if (startGeo == null || endGeo == null)
                            continue;

                        Coordinate[] coords = { startGeo.Coordinate, endGeo.Coordinate };
                        lineArray.Add(fact.CreateLineString(coords));
                    }

                    Geometry multiLine = fact.CreateMultiLineString(lineArray.ToArray());
                    IGdRowBuffer buffer = new GdRowBuffer();
                    foreach (IGdParamater paramater in row.Paramaters)
                    {
                        if (!paramater.DataType.HasValue)
                            continue;
                        buffer.Put(paramater.Name, paramater.Value, paramater.DataType.Value);

                    }
                    buffer.Put("geometry", multiLine);
                    resultTable.Insert(buffer);

                }

                string geojson = resultTable.ToGeojson(GdGeoJsonSeralizeType.OnlyData, 3);
                File.WriteAllText("C:\\Users\\eniso\\Desktop\\" + "route.json", geojson);

                navaidTable.SqlFilter = null;
                string navid = navaidTable.ToGeojson(GdGeoJsonSeralizeType.OnlyData, 3);
                File.WriteAllText("C:\\Users\\eniso\\Desktop\\" + "navid.json", navid);

                designatedTable.SqlFilter = null;
                string desgined = designatedTable.ToGeojson(GdGeoJsonSeralizeType.OnlyData, 3);
                File.WriteAllText("C:\\Users\\eniso\\Desktop\\" + "desgined.json", desgined);

                MessageBox.Show("Finish...Time:" + DateTime.Now, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                GdFileLogger.Current.LogException(exception);
                MessageBox.Show(exception.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}