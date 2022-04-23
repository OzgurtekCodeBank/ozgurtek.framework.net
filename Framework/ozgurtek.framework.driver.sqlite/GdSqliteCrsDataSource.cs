using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;
using System.Collections.Generic;

namespace ozgurtek.framework.driver.sqlite
{
    public class GdSqliteCrsDataSource : IGdCrsDataSource
    {
        private const string Tablename = "epsg";
        private const string WktFieldName = "wkt";
        private readonly GdSqlLiteTable _table;

        internal GdSqliteCrsDataSource(string connection)
        {
            GdSqlLiteDataSource dataSource = GdSqlLiteDataSource.OpenOrCreate(connection);
            _table = dataSource.GetTable(Tablename);
            if (_table != null)
                return;

            _table = dataSource.CreateTable(Tablename, null, 0, null);
            _table.CreateField(new GdField("wkt", GdDataType.String));
        }

        public static GdSqliteCrsDataSource OpenOrCreate(string connection)
        {
            return new GdSqliteCrsDataSource(connection);
        }

        public bool CanEdit
        {
            get { return true; }
        }

        public void Add(int code, string defination)
        {
            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put(_table.KeyField, code);
            buffer.Put(WktFieldName, defination);
            _table.Insert(buffer);
        }

        public IEnumerable<IGdKeyValue> GetDefination()
        {
            foreach (IGdRow row in _table.Rows)
            {
                GdKeyValue keyValue = new GdKeyValue();
                keyValue.Key = row.GetAsInteger(_table.KeyField);
                keyValue.Value = row.GetAsString(WktFieldName);
                yield return keyValue;
            }
        }

        public IGdKeyValue GetDefination(int key)
        {
            IGdRow row = _table.FindRow(key);
            if (row == null)
                return null;

            GdKeyValue keyValue = new GdKeyValue();
            keyValue.Key = key;
            keyValue.Value = row.GetAsString(WktFieldName);
            return keyValue;
        }

        public string CrsType
        {
            get { return "EPSG"; }
        }

        public void Add(IEnumerable<IGdKeyValue> keyValues)
        {
            _table.BeginTransaction();
            foreach (IGdKeyValue keyValue in keyValues)
                Add(keyValue.Key, keyValue.Value);
            _table.CommitTransaction();
        }
    }
}