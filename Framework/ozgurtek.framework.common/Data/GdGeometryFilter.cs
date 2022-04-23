using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data
{
    public class GdGeometryFilter : IGdGeometryFilter
    {
        public GdGeometryFilter(Geometry geometry, GdSpatialRelation spatialRelation)
        {
            Geometry = geometry;
            SpatialRelation = spatialRelation;
        }

        public GdGeometryFilter(Envelope envelope)
        {
            Envelope = envelope;
        }

        public Geometry Geometry { get; }
        public Envelope Envelope { get; }
        public GdSpatialRelation SpatialRelation { get; }
    }
}
