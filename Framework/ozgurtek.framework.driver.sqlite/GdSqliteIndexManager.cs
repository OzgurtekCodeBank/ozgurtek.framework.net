using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.driver.sqlite
{
    internal class GdSqliteIndexManager
    {
        private readonly GdSqlLiteConnection _connection;
        private readonly GdSqlLiteTable _table;

        public GdSqliteIndexManager(GdSqlLiteTable table, GdSqlLiteConnection connection)
        {
            _connection = connection;
            _table = table;
        }

        public void Insert(int id, Envelope envelope)
        {
            object[] values = new object[6];
            values[0] = id;
            values[1] = _table.Name.Trim() + "_" + _table.GeometryField.Trim();
            values[2] = envelope.MinX;
            values[3] = envelope.MinY;
            values[4] = envelope.MaxX;
            values[5] = envelope.MaxY;
            string sql = "insert into geometry_index (pkid,table_column,xmin,ymin,xmax,ymax) VALUES(?,?,?,?,?,?)";
            _connection.ExecuteNonQuery(sql, values);
        }

        public void Delete(long id)
        {
            string key = _table.Name.Trim() + "_" + _table.GeometryField.Trim();
            string sql = $"delete from geometry_index where table_column = '{key}' and pkid = {id}";
            _connection.ExecuteNonQuery(sql);
        }

        public void Update(int id, Envelope envelope)
        {
            object[] values = new object[4];
            string key = _table.Name.Trim() + "_" + _table.GeometryField.Trim();
            values[0] = envelope.MinX;
            values[1] = envelope.MinY;
            values[2] = envelope.MaxX;
            values[3] = envelope.MaxY;
            string sql = $"update geometry_index set xmin=?,ymin=?,xmax=?,ymax=? where pkid = {id} and table_column = '{key}'";
            _connection.ExecuteNonQuery(sql, values);
        }

        public void Truncate()
        {
            string index = _table.Name.Trim() + "_" + _table.GeometryField.Trim();
            string sql = $"delete from geometry_index where table_column = '{index}'";
            _connection.ExecuteNonQuery(sql);
        }

        public void ReIndex(IGdTrack track)
        {
            _connection.BeginTransaction();

            Truncate();

            IGdGeometryFilter filter = _table.GeometryFilter;
            IGdFilter sqlFilter = _table.SqlFilter;
            string columnFilter = _table.ColumnFilter;

            _table.GeometryFilter = null;
            _table.SqlFilter = null;
            _table.ColumnFilter = $"{_table.KeyField},{_table.GeometryField}";

            double featureCount = _table.RowCount;
            double counter = 0;
            foreach (IGdRow row in _table.Rows)
            {
                if (track != null && track.CancellationPending)
                    break;

                if (row.IsNull(_table.KeyField))
                    continue;

                if (row.IsNull(_table.GeometryField))
                    continue;

                Geometry geometry = row.GetAsGeometry(_table.GeometryField);
                if (geometry == null)
                    continue;

                Envelope envelope = geometry.EnvelopeInternal;
                if (envelope == null || envelope.IsNull)
                    continue;

                int fid = row.GetAsInteger(_table.KeyField);
                Insert(fid, envelope);

                if (track != null && featureCount > 0)
                {
                    track.ReportProgress(++counter / featureCount);
                }
            }

            _table.GeometryFilter = filter;
            _table.SqlFilter = sqlFilter;
            _table.ColumnFilter = columnFilter;

            _connection.CommitTransaction();
        }
    }
}
