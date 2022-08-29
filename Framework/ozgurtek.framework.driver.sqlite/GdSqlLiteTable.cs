using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ozgurtek.framework.driver.sqlite
{
    public class GdSqlLiteTable : IGdDbTable
    {
        private readonly IGdFilter _baseFilter; //baseFilter
        private IGdFilter _sqlFilter; //sql Filter

        private string _description;
        private string _alias;
        private readonly string _name;
        private string _columnFilter;
        private string _orderBy;
        private long _limit = -1;
        private long _offset = -1;
        private IGdGeometryFilter _geometryFilter;

        private readonly GdSqlLiteConnection _connection;
        private readonly GdSqlLiteDataSource _dataSource;
        private readonly GdSqliteTableMetaData _metedata;
        private readonly GdSqliteIndexManager _index;

        internal GdSqlLiteTable(GdSqlLiteDataSource dataSource, GdSqlLiteConnection connection, string name, IGdFilter baseFilter)
        {
            _name = name;
            _dataSource = dataSource;
            _connection = connection;
            _baseFilter = baseFilter;
            _metedata = new GdSqliteTableMetaData(this, connection);
            _index = new GdSqliteIndexManager(this, _connection);
        }

        public virtual string Name
        {
            get { return _name; }
        }

        public virtual string Address
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_alias))
                    return $"{_name} @ {_dataSource.ConnectionString}";

                return _alias;
            }
            set { _alias = value; }
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public int Srid
        {
            get { return _metedata.SRID; }
            set { _metedata.SRID = value; }
        }

        public GdGeometryType? GeometryType
        {
            get { return _metedata.GeometryType; }
            set { _metedata.GeometryType = value; }
        }

        public virtual string KeyField
        {
            get { return _metedata.KeyField; }
            set { _metedata.KeyField = value; }
        }

        public virtual string GeometryField
        {
            get { return _metedata.GeometryFieldName; }
            set { _metedata.GeometryFieldName = value; }
        }

        public virtual IGdFilter SqlFilter
        {
            get { return _sqlFilter; }
            set { _sqlFilter = value; }
        }

        //todo: enis bu filitreyi verince schema işini düşün
        public virtual string ColumnFilter
        {
            get { return _columnFilter; }
            set { _columnFilter = value; }
        }

        public virtual string OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = value; }
        }

        public virtual long Limit
        {
            get { return _limit; }
            set { _limit = value; }
        }

        public virtual long Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public virtual IGdGeometryFilter GeometryFilter
        {
            get { return _geometryFilter; }
            set { _geometryFilter = value; }
        }

        public bool CanEditRow
        {
            get { return true; }
        }

        public bool CanEditField
        {
            get { return true; }
        }

        public virtual bool CanSupportTransaction
        {
            get { return true; }
        }

        public long RowCount
        {
            get
            {
                if (GeometryFilter == null)
                {
                    GdSqliteQueryBuilder builder = new GdSqliteQueryBuilder(this);
                    string query = $"SELECT COUNT(*) FROM ({builder.CreateQuery()}) A";
                    return _connection.ExecuteScalar<int>(query);
                }

                int count = 0;
                foreach (IGdRow unused in Rows)
                    count++;

                return count;
            }
        }

        public Envelope Envelope
        {
            get
            {
                if (string.IsNullOrEmpty(GeometryField))
                    throw new Exception($"Please specify geometry field of {Name} table");

                Envelope result = new Envelope();

                string query = null;
                if (GeometryFilter == null && SqlFilter == null)
                {
                    query =
                        "select min(xmin) as minx, " +
                        "min(ymin) as miny, " +
                        "max(xmax) as maxx, " +
                        "max(ymax) as maxy " +
                        "from geometry_index where table_column = " +
                        $"'{Name + "_" + GeometryField}'";
                }
                else if (GeometryFilter == null && SqlFilter != null)
                {
                    GdSqliteQueryBuilder builder = new GdSqliteQueryBuilder(this);
                    query = "select min(xmin) as minx, " +
                            "min(ymin) as miny, " +
                            "max(xmax) as maxx, " +
                            "max(ymax) as maxy " +
                            "from " +
                            "geometry_index, " +
                            $"({builder.CreateQuery()}) ain " +
                            "where ain.ogc_fid = geometry_index.pkid and " +
                            "table_column =" +
                            $"'{Name + "_" + GeometryField}'";
                }

                if (!string.IsNullOrWhiteSpace(query))
                {
                    IEnumerable<GdSqliteRow> rows = _connection.ExecuteReader(query, this);
                    GdSqliteRow row = rows.FirstOrDefault();
                    if (row == null)
                        return result;

                    double minX = row.GetAsReal("minx");
                    double minY = row.GetAsReal("miny");
                    double maxX = row.GetAsReal("maxx");
                    double maxY = row.GetAsReal("maxy");

                    return new Envelope(minX, maxX, minY, maxY);
                }

                foreach (IGdRow row in Rows)
                {
                    if (row.IsNull(GeometryField))
                        continue;

                    Geometry geometry = row.GetAsGeometry(GeometryField);
                    if (geometry == null)
                        continue;

                    Envelope envelope = geometry.EnvelopeInternal;
                    if (envelope.IsNull)
                        continue;

                    result.ExpandToInclude(envelope);
                }

                return result;
            }
        }

        public IEnumerable<IGdRow> Rows
        {
            get
            {
                GdSqliteQueryBuilder builder = new GdSqliteQueryBuilder(this);
                string commandText = builder.CreateQuery();
                IEnumerable<GdSqliteRow> rows = _connection.ExecuteReader(commandText, this);

                if (string.IsNullOrWhiteSpace(GeometryField) || GeometryFilter == null ||
                    GeometryFilter.Envelope != null)
                {
                    foreach (GdSqliteRow row in rows)
                        yield return row;
                }
                else
                {
                    foreach (GdSqliteRow row in rows)
                    {
                        if (row.IsNull(GeometryField))
                            continue;

                        Geometry geometry = row.GetAsGeometry(GeometryField);
                        Geometry filter = GdProjection.Project(GeometryFilter.Geometry, Srid);
                        if (!GdGeometryUtil.Relate(geometry, filter, GeometryFilter.SpatialRelation))
                            continue;

                        yield return row;
                    }
                }
            }
        }

        public IGdSchema Schema
        {
            get
            {
                //todo: burada bir sorun var...
                return _metedata.Schema;
            }
        }

        public IGdRow FindRow(long rowId)
        {
            if (string.IsNullOrEmpty(KeyField))
                throw new Exception($"Please specify key field of {Name} table");

            if (GeometryFilter == null)
            {
                GdSqliteQueryBuilder builder = new GdSqliteQueryBuilder(this);
                string query = $"SELECT * FROM ({builder.CreateQuery()}) A WHERE {KeyField} = {rowId}";
                IEnumerable<IGdRow> rows = _connection.ExecuteReader(query, this);
                return rows?.FirstOrDefault();
            }

            foreach (IGdRow row in Rows)
            {
                if (row.IsNull(KeyField))
                    continue;

                long key = row.GetAsInteger(KeyField);
                if (key == rowId)
                    return row;
            }

            return null;
        }

        public IEnumerable<object> GetDistinctValues(string fieldName)
        {
            if (GeometryFilter == null)
            {
                GdSqliteQueryBuilder builder = new GdSqliteQueryBuilder(this);
                string query = $"SELECT DISTINCT({fieldName}) FROM ({builder.CreateQuery()}) A";
                IEnumerable<GdSqliteRow> rows = _connection.ExecuteReader(query, this);
                foreach (GdSqliteRow row in rows)
                    yield return row.Get(fieldName);
            }
            else
            {
                HashSet<object> unique = new HashSet<object>();
                foreach (IGdRow row in Rows)
                    unique.Add(((GdSqliteRow) row).Get(fieldName));

                yield return unique;
            }
        }

        public long Insert(IGdRowBuffer row)
        {
            List<string> keys = new List<string>();
            List<object> values = new List<object>();
            foreach (IGdParamater paramater in row.Paramaters)
            {
                keys.Add(paramater.Name);
                values.Add(paramater.Value);
            }

            for (int i = 0; i < keys.Count; i++)
                values[i] = ConvertBufferObject(keys[i], values[i]);

            char[] charArray = new string('?', keys.Count()).ToCharArray();
            string questions = string.Join(",", charArray);
            string fields = string.Join(",", keys);

            string sql = $"INSERT INTO {Name} ({fields}) VALUES({questions})";
            int result = _connection.ExecuteNonQuery(sql, values.ToArray());
            OnRowChanged("Insert", row);

            if (string.IsNullOrEmpty(KeyField))
                return result;

            sql = $"select max({KeyField}) from {Name}";
            int maxId = _connection.ExecuteScalar<int>(sql);

            //insert index...
            if (!string.IsNullOrWhiteSpace(GeometryField) && !row.IsNull(GeometryField))
            {
                Geometry geometry = row.GetAsGeometry(GeometryField);
                geometry = GdProjection.Project(geometry, Srid);
                if (geometry != null)
                {
                    Envelope envelope = geometry.EnvelopeInternal;
                    if (envelope != null)
                    {
                        _index.Insert(maxId, envelope);
                    }
                }
            }

            return maxId;
        }

        public long Update(IGdRowBuffer row)
        {
            if (string.IsNullOrEmpty(KeyField))
                throw new Exception("Can not find keyfield defination of table");

            if (!row.ContainsKey(KeyField))
                throw new Exception($"Can not find keyfield {KeyField} of row buffer");

            List<string> keys = new List<string>();
            List<object> values = new List<object>();
            foreach (IGdParamater paramater in row.Paramaters)
            {
                keys.Add(paramater.Name);
                values.Add(paramater.Value);
            }

            List<string> list = new List<string>();
            for (int i = 0; i < keys.Count; i++)
            {
                values[i] = ConvertBufferObject(keys[i], values[i]);
                list.Add(keys[i] + "=?");
            }

            string setStr = string.Join(",", list);
            long key = row.GetAsInteger(KeyField);
            string sql = $"UPDATE {Name} SET {setStr} WHERE {KeyField} = {key}";
            int result = _connection.ExecuteNonQuery(sql, values.ToArray());

            OnRowChanged("update", row);

            //update index...
            if (!string.IsNullOrWhiteSpace(GeometryField) && !row.IsNull(GeometryField) &&
                !string.IsNullOrWhiteSpace(KeyField) && !row.IsNull(KeyField))
            {
                Geometry geometry = row.GetAsGeometry(GeometryField);
                geometry = GdProjection.Project(geometry, Srid);
                if (geometry != null)
                {
                    Envelope envelope = geometry.EnvelopeInternal;
                    if (envelope != null)
                    {
                        long id = row.GetAsInteger(KeyField);
                        _index.Update(id, envelope);
                    }
                }
            }

            return result;
        }

        //todo: index tablosu
        public long Delete(long id)
        {
            if (string.IsNullOrEmpty(KeyField))
                throw new Exception("Can not find keyfield defination of table");

            string sql = $"DELETE FROM {Name} WHERE {KeyField} = {id}";

            //backup row
            IGdRow row = null;
            if (RowChanged != null)
                row = FindRow((int) id);

            //delete row
            int result = _connection.ExecuteNonQuery(sql);

            //send row, mark as delete
            if (row != null)
                OnRowChanged("delete", (IGdRowBuffer) row);

            //delete index...
            _index.Delete(id);

            return result;
        }

        public void Truncate()
        {
            string sql = $"DELETE FROM {Name}";
            _connection.ExecuteNonQuery(sql);
        }

        public void CreateField(IGdField field)
        {
            _metedata.CreateField(field);
        }

        public void DeleteField(IGdField field)
        {
            _metedata.DeleteField(field);
        }

        public string ToGeojson(GdGeoJsonSeralizeType type)
        {
            GdGeoJsonSerializer serializer = new GdGeoJsonSerializer();
            serializer.SerializeType = type;
            return serializer.Serialize(this);
        }

        public DataTable ToDataTable()
        {
            return GdMemoryTable.LoadFromTable(this).ToDataTable();
        }

        public IGdTable Clone()
        {
            GdSqlLiteTable table = new GdSqlLiteTable(_dataSource, _connection, _name, _baseFilter);
            return table;
        }

        public event EventHandler<GdRowChangedEventArgs> RowChanged;


        public void BeginTransaction()
        {
            _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _connection.CommitTransaction();
        }

        public void RollBackTransaction()
        {
            _connection.RollbackTransaction();
        }

        protected virtual void OnRowChanged(string type, IGdRowBuffer buffer)
        {
            RowChanged?.Invoke(this, new GdRowChangedEventArgs(this, type, buffer));
        }

        internal IGdFilter BaseFilter
        {
            get { return _baseFilter; }
        }

        private object ConvertBufferObject(string key, object value)
        {
            Geometry geometry = value as Geometry;
            if (geometry == null)
                return value;

            geometry = GdProjection.Project(geometry, Srid);
            GdSqliteField field = (GdSqliteField) Schema.GetFieldByName(key);
            if (field.GeometryFormat == GdSqliteGeometryFormat.WKB)
                return geometry.AsBinary();

            return geometry.AsText();
        }

        public void ReIndex(IGdTrack track)
        {
            _index.ReIndex(track);
        }

        public GdSqlLiteDataSource DataSource
        {
            get { return _dataSource; }
        }
    }
}