using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ozgurtek.framework.driver.sqlite
{
    internal class GdSqliteDomain : IGdDomain
    {
        private readonly GdSqlLiteConnection _connection;
        private readonly GdSqlLiteDataSource _dataSource;

        public GdSqliteDomain(GdSqlLiteConnection connection, GdSqlLiteDataSource dataSource)
        {
            _connection = connection;
            _dataSource = dataSource;
        }

        public IEnumerable<IGdKeyValueSet> GetDomain()
        {
            List<GdKeyValueSet> result = new List<GdKeyValueSet>();

            GdSqlLiteTable domainTable = _dataSource.GetTable("domains");
            foreach (IGdRow domainRow in domainTable.Rows)
            {
                string domainName = domainRow.GetAsString("domain_name");

                GdSqlLiteTable codeValTable = _dataSource.GetTable("coded_values");
                codeValTable.SqlFilter = new GdSqlFilter($"domain_name = '{domainName}'");

                List<IGdKeyValue> keyValues = new List<IGdKeyValue>();
                foreach (IGdRow codeValRow in codeValTable.Rows)
                {
                    int code = codeValRow.GetAsInteger("code");
                    string value = codeValRow.GetAsString("value");

                    GdKeyValue keyValue = new GdKeyValue(code, value);
                    keyValues.Add(keyValue);
                }

                GdKeyValueSet set = new GdKeyValueSet(domainName, keyValues);
                result.Add(set);
            }

            return result;
        }

        public IGdKeyValueSet GetDomain(string domainName)
        {
            foreach (IGdKeyValueSet keyValueSet in GetDomain())
            {
                if (keyValueSet.Name.Equals(domainName, StringComparison.OrdinalIgnoreCase))
                    return keyValueSet;
            }
            return null;
        }

        public void AddDomain(string domainName)
        {
            GdSqlLiteTable table = _dataSource.GetTable("domains");
            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put("domain_name", domainName);
            table.Insert(buffer);
        }

        public void AddKeyValue(string domainName, int code, string value)
        {
            GdSqlLiteTable table = _dataSource.GetTable("coded_values");
            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put("domain_name", domainName);
            buffer.Put("code", code);
            buffer.Put("value", value);
            table.Insert(buffer);
        }

        public void DeleteDomain(string domainName)
        {
            //delete domain
            string sql = $"delete from domains where domain_name = '{domainName}'";
            _connection.ExecuteNonQuery(sql);

            //delete coded values
            sql = $"delete from coded_values where domain_name = '{domainName}'";
            _connection.ExecuteNonQuery(sql);

            //delete domain columns values
            sql = $"delete from domain_columns where f_domain_name = '{domainName}'";
            _connection.ExecuteNonQuery(sql);
        }

        public void UpdateDomain(string oldValue, string newValue)
        {
            //update domain table domain
            string sql = $"update domains set domain_name = '{newValue}' where domain_name = '{oldValue}'";
            _connection.ExecuteNonQuery(sql);

            //update domain columns
            sql = $"update domain_columns set f_domain_name = '{newValue}' where f_domain_name = '{oldValue}'";
            _connection.ExecuteNonQuery(sql);

            //update coded_values columns
            sql = $"update coded_values set domain_name = '{newValue}' where domain_name = '{oldValue}'";
            _connection.ExecuteNonQuery(sql);
        }

        public void DeleteKeyValue(string domain, int code)
        {
            string sql = $"delete from coded_values where domain_name = '{domain}' and code = {code}";
            _connection.ExecuteNonQuery(sql);
        }

        public void AddFieldAsDomain(string tableName, string fieldName, string domainName)
        {
            GdSqlLiteTable table = _dataSource.GetTable("domain_columns");
            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put("f_table_name", tableName);
            buffer.Put("f_domain_column", fieldName);
            buffer.Put("f_domain_name", domainName);
            table.Insert(buffer);
        }

        public string GetDomainName(string tableName, string fieldName)
        {
            string sql = $"select f_domain_name from domain_columns where f_table_name = '{tableName}' and f_domain_column = '{fieldName}'";
            IEnumerable<GdSqliteRow> rows = _connection.ExecuteReader(sql);
            IGdRow row = rows?.FirstOrDefault();
            if (row == null)
                return null;

            return row.GetAsString("f_domain_name");
        }
    }
}
