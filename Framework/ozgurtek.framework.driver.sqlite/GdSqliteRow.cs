using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;

namespace ozgurtek.framework.driver.sqlite
{
    internal class GdSqliteRow : GdRowBuffer
    {
        public override Geometry GetAsGeometry(string key)
        {
            Geometry wkb = DbConvert.ToGeometry(Row[key].Value);
            wkb.SRID = _table.Srid;
            return wkb;
        }

        public void Put(string key, object value)
        {
            Row.Add(key, new GdParameter(key, value));
        }

        internal virtual object Get(string key)
        {
            return Row[key].Value;
        }
    }
}
