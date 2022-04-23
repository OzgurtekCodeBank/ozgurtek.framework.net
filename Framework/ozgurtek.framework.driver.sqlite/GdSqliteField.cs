using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.driver.sqlite
{
    internal class GdSqliteField : GdField
    {
        private GdSqliteGeometryFormat _geometryFormat = GdSqliteGeometryFormat.None;
        private IGdKeyValueSet _keyValueSet;

        public GdSqliteGeometryFormat GeometryFormat
        {
            get { return _geometryFormat; }
            set { _geometryFormat = value; }
        }

        public override IGdKeyValueSet Domain
        {
            get
            {
                if (_keyValueSet != null)
                    return _keyValueSet;

                string domainName = Table.DataSource.GetDomainName(Table.Name, FieldName);
                if (string.IsNullOrWhiteSpace(domainName))
                    return null;

                IGdKeyValueSet keyValueSet = Table.DataSource.GetDomain(domainName);
                return _keyValueSet = keyValueSet;
            }
            set
            {
                _keyValueSet = value;
            }
        }

        public GdSqlLiteTable Table { get; set; }
    }
}
