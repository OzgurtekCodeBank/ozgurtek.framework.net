using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.converter.winforms
{
    public class GdExt3dModelExportEngine
    {
        private GdMemoryTable _table;
        private const string Id = "gd_id";
        private const string Geometry = "gd_geometry";
        private const string Height = "gd_ext_height";
        private const string Style = "gd_style";
        private const string Description = "gd_description";

        public void Export(IGdTable table, string outputFolder, List<GdTileIndex> tileIndex, int epsgCode,
            IGdTrack track)
        {
            PrepareMemTable();

            long fileName = 0;
            foreach (GdTileIndex index in tileIndex)
            {
                Envelope envelope = GdProjection.Project(index.Envelope, 4326, epsgCode);
                GeometryFactory factory = new GeometryFactory();
                Geometry geometry1 = factory.ToGeometry(envelope);
                table.GeometryFilter = new GdGeometryFilter(geometry1, GdSpatialRelation.Intersects);

                foreach (IGdRow row in table.Rows)
                {
                    GdRowBuffer buffer = new GdRowBuffer();
                    buffer.Put(Id, row.GetAsInteger(Id));

                    Geometry geometry = row.GetAsGeometry(Geometry);
                    geometry.SRID = epsgCode;
                    geometry = GdProjection.Project(geometry, 4326);

                    buffer.Put(Geometry, geometry);
                    buffer.Put(Height, row.GetAsReal(Height));

                    if (row.Table.Schema.GetFieldByName(Style) != null)
                        buffer.Put(Style, row.GetAsReal(Style));

                    if (row.Table.Schema.GetFieldByName(Description) != null)
                        buffer.Put(Description, row.GetAsString(Description));

                    _table.Insert(buffer);
                }

                string fullFileName = Path.Combine(outputFolder, DbConvert.ToString(++fileName) + ".json");
                Flush(fullFileName);
                PrepareMemTable();
            }


            MessageBox.Show("Finish");
        }

        private void PrepareMemTable()
        {
            _table = new GdMemoryTable();
            _table.CreateField(new GdField(Id, GdDataType.Integer));
            _table.CreateField(new GdField(Geometry, GdDataType.Geometry));
            _table.CreateField(new GdField(Height, GdDataType.Real));
            _table.CreateField(new GdField(Style, GdDataType.Real));
            _table.CreateField(new GdField(Description, GdDataType.Real));
        }

        private void Flush(string fileName)
        {
            string geojson = _table.ToGeojson(GdGeoJsonSeralizeType.OnlyData, 3);
            File.WriteAllText(fileName, geojson);
        }
    }
}