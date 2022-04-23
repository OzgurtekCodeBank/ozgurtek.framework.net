using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.driver.oracle.NetTopologySuit.IO.Oracle;
using ozgurtek.framework.driver.oracle.NetTopologySuit.IO.Oracle.Sdo;

namespace ozgurtek.framework.driver.oracle
{
    public class GdOracleRowBuffer : GdRowBuffer
    {
        public override Geometry GetAsGeometry(string key)
        {
            OracleGeometryReader reader = new OracleGeometryReader();
            Geometry geometry = reader.Read((SdoGeometry)Row[key].Value);
            return geometry;
        }
    }
}
