using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Util
{
    public static class GdGeometryUtil
    {
        public static bool Relate(Geometry geom1, Geometry geom2, GdSpatialRelation relation)
        {
            switch (relation)
            {
                case GdSpatialRelation.Contains:
                    return geom1.Contains(geom2);
                case GdSpatialRelation.Crosses:
                    return geom1.Crosses(geom2);
                case GdSpatialRelation.Disjoint:
                    return geom1.Disjoint(geom2);
                case GdSpatialRelation.Equals:
                    return geom1.EqualsExact(geom2);
                case GdSpatialRelation.Intersects:
                    return geom1.Intersects(geom2);
                case GdSpatialRelation.Overlaps:
                    return geom1.Overlaps(geom2);
                case GdSpatialRelation.Touches:
                    return geom1.Touches(geom2);
                case GdSpatialRelation.Within:
                    return geom1.Within(geom2);
                default:
                    return false;
            }
        }

        public static OgcGeometryType ConvertGeometryType(GdGeometryType geometryType)
        {
            switch (geometryType)
            {
                case GdGeometryType.Polygon:
                    return OgcGeometryType.Polygon;
                case GdGeometryType.Line:
                    return OgcGeometryType.LineString;
                case GdGeometryType.Point:
                    return OgcGeometryType.Point;
            }
            return OgcGeometryType.GeometryCollection;//todo: doğru mu acaba?
        }

        public static GdGeometryType? ConvertGeometryType(OgcGeometryType geometryType)
        {
            switch (geometryType)
            {
                case OgcGeometryType.Polygon:
                case OgcGeometryType.MultiPolygon:
                case OgcGeometryType.Surface:
                case OgcGeometryType.CurvePolygon:
                case OgcGeometryType.MultiSurface:
                case OgcGeometryType.PolyhedralSurface:
                case OgcGeometryType.TIN:
                    return GdGeometryType.Polygon;

                case OgcGeometryType.LineString:
                case OgcGeometryType.CircularString:
                case OgcGeometryType.CompoundCurve:
                case OgcGeometryType.Curve:
                case OgcGeometryType.MultiLineString:
                    return GdGeometryType.Line;

                case OgcGeometryType.Point:
                case OgcGeometryType.MultiPoint:
                case OgcGeometryType.MultiCurve:
                    return GdGeometryType.Point;
                default:
                    return null;
            }
        }

        public static bool IsGeometrySimple(OgcGeometryType geometryType)
        {
            switch (geometryType)
            {
                case OgcGeometryType.MultiPolygon:
                case OgcGeometryType.MultiSurface:
                case OgcGeometryType.MultiLineString:
                case OgcGeometryType.MultiPoint:
                case OgcGeometryType.MultiCurve:
                    return true;

                default:
                    return false;
            }
        }
    }
}
