using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;

namespace ozgurtek.framework.driver.sqlserver
{
    internal class GdMsSqlRowBuffer : GdRowBuffer
    {
        public override Geometry GetAsGeometry(string key)
        {
            SqlGeometry sqlgeometry = (SqlGeometry) Row[key].Value;
            SqlBytes stAsBinary = sqlgeometry.STAsBinary();
            Geometry geometry = DbConvert.FromWkb(stAsBinary.Value);
            geometry.SRID = sqlgeometry.STSrid.Value;
            return geometry;
        }
    }
}
