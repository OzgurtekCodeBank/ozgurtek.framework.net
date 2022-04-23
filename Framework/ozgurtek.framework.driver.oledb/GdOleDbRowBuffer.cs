using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;

namespace ozgurtek.framework.driver.oledb
{
    internal class GdOleDbRowBuffer : GdRowBuffer
    {
        public override Geometry GetAsGeometry(string key)
        {
            return DbConvert.ToGeometry(Row[key].Value);
        }
    }
}
