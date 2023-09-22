using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;

namespace ozgurtek.framework.driver.gdal
{
    public class GdOgrRowBuffer : GdRowBuffer
    {
        private Geometry _geometry;
        private long? _id;
        private string _style;

        public void SetGeometryDirectly(Geometry geometry)
        {
            _geometry = geometry;
        }

        public void SetFeatureIdDirectly(long id)
        {
            _id = id;
        }

        public void SetStyleStringDirectly(string style)
        {
            _style = style;
        }

        public Geometry GetGeometryDirectly()
        {
            return _geometry;
        }

        public long? GetFeatureIdDirectly()
        {
            return _id;
        }

        public string GetStyleStringDirectly()
        {
            return _style;
        }
    }
}
