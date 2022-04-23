using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.driver.oledb
{
    public class GdOleDbDataSource : GdAbstractDbDataSource
    {
        private readonly string _connectionString;

        public override string ConnectionString
        {
            get { return _connectionString; }
        }

        public override IDbConnection GetConnection()
        {
            OleDbConnection connection = new OleDbConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public override string Name
        {
            get { return "OleDb Data Source"; }
        }

        internal GdOleDbDataSource(string source)
        {
            _connectionString = source;
        }

        public static GdOleDbDataSource Open(string source)
        {
            return new GdOleDbDataSource(source);
        }

        public int TableCount
        {
            get
            {
                List<GdOleDbTable> tables = new List<GdOleDbTable>(GetTable());
                return tables.Count;
            }
        }

        public GdOleDbTable GetTable(string name)
        {
            foreach (GdOleDbTable table in GetTable())
            {
                if (!table.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                return new GdOleDbTable(this, name, new GdSqlFilter("SELECT * FROM " + name));
            }

            return null;
        }

        public IEnumerable<GdOleDbTable> GetTable()
        {
            DataTable dataTable = GetSchema();
            foreach (DataRow row in dataTable.Rows)
            {
                string name = DbConvert.ToString(row["TABLE_NAME"]);
                string type = DbConvert.ToString(row["TABLE_TYPE"]);
                if (!type.Equals("TABLE", StringComparison.OrdinalIgnoreCase))
                    continue;

                GdOleDbTable table = new GdOleDbTable(this, name, new GdSqlFilter("SELECT * FROM " + name));
                yield return table;
            }
        }

        public GdOleDbTable ExecuteSql(string name, IGdFilter filter)
        {
            return new GdOleDbTable(this, name, filter);
        }

        private DataTable GetSchema()
        {
            using (OleDbConnection connection = (OleDbConnection) GetConnection())
            {
                return connection.GetSchema("tables");
            }
        }
    }
}