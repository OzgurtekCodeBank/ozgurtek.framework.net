using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;

namespace ozgurtek.framework.driver.postgres
{
    internal class GdPgRowBuffer : GdRowBuffer
    {
        public override Geometry GetAsGeometry(string key)
        {
            return (Geometry)Row[key].Value;
        }
    }
}
