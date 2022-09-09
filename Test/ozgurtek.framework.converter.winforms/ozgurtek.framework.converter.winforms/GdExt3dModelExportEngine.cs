using System;
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

        public void Export(IGdTable table, string outputFolder, long entityPerFile, int epsgCode, IGdTrack track)
        {
            PrepareMemTable();

            double current = 0;
            long count = 0;
            long tableCount = table.RowCount;
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

                if (++count % entityPerFile == 0)
                {
                    Flush(outputFolder);
                    PrepareMemTable();
                    count = 0;

                    if (track != null)
                        track.ReportProgress(DbConvert.ToDouble(++current * entityPerFile * 100 / tableCount));
                }
            }

            if (track != null)
                track.ReportProgress(100);

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

        private void Flush(string outputFolder)
        {
            string geojson = _table.ToGeojson(GdGeoJsonSeralizeType.OnlyData, 3);
            string file = Path.Combine(outputFolder, Guid.NewGuid() + ".geojson");
            File.WriteAllText(file, geojson);
        }
    }
}
