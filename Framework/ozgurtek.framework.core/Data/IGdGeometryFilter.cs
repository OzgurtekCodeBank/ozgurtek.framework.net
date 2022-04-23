using NetTopologySuite.Geometries;

namespace ozgurtek.framework.core.Data
{
    public interface IGdGeometryFilter
    {
        Geometry Geometry { get; }

        Envelope Envelope { get; }

        GdSpatialRelation SpatialRelation { get; }
    }
}
