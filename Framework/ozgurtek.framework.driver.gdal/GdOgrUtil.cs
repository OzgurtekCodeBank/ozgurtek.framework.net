﻿using System;
using System.Text;
using GeoAPI.CoordinateSystems;
using OSGeo.OGR;
using OSGeo.OSR;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.driver.gdal
{
    internal class GdOgrUtil
    {
        internal static wkbGeometryType GetGeometryType(GdGeometryType? type, bool allowMultigeom)
        {
            if (!type.HasValue)
                return wkbGeometryType.wkbNone;

            if (!allowMultigeom)
            {
                switch (type)
                {
                    case GdGeometryType.Line:
                        return wkbGeometryType.wkbLineString;
                    case GdGeometryType.Polygon:
                        return wkbGeometryType.wkbPolygon;
                    case GdGeometryType.Point:
                        return wkbGeometryType.wkbPoint;
                    default:
                        return wkbGeometryType.wkbUnknown;
                }
            }

            switch (type)
            {
                case GdGeometryType.Line:
                    return wkbGeometryType.wkbMultiLineString;
                case GdGeometryType.Polygon:
                    return wkbGeometryType.wkbMultiPolygon;
                case GdGeometryType.Point:
                    return wkbGeometryType.wkbMultiPoint;
                default:
                    return wkbGeometryType.wkbUnknown;
            }
        }

        internal static GdGeometryType? GetGeometryType(wkbGeometryType type)
        {
            switch (type)
            {
                case wkbGeometryType.wkbPoint:
                case wkbGeometryType.wkbMultiPoint:
                case wkbGeometryType.wkbPoint25D:
                case wkbGeometryType.wkbPointM:
                case wkbGeometryType.wkbMultiPointM:
                case wkbGeometryType.wkbPointZM:
                case wkbGeometryType.wkbMultiPointZM:
                case wkbGeometryType.wkbMultiPoint25D:
                    return GdGeometryType.Point;

                case wkbGeometryType.wkbPolygon:
                case wkbGeometryType.wkbPolygon25D:
                case wkbGeometryType.wkbCurvePolygon:
                case wkbGeometryType.wkbMultiPolygon25D:
                case wkbGeometryType.wkbSurface:
                case wkbGeometryType.wkbPolyhedralSurface:
                case wkbGeometryType.wkbTIN:
                case wkbGeometryType.wkbTriangle:
                case wkbGeometryType.wkbCurvePolygonZ:
                case wkbGeometryType.wkbMultiSurfaceZ:
                case wkbGeometryType.wkbSurfaceZ:
                case wkbGeometryType.wkbPolyhedralSurfaceZ:
                case wkbGeometryType.wkbTINZ:
                case wkbGeometryType.wkbTriangleZ:
                case wkbGeometryType.wkbMultiPolygonM:
                case wkbGeometryType.wkbCurvePolygonM:
                case wkbGeometryType.wkbMultiSurfaceM:
                case wkbGeometryType.wkbSurfaceM:
                case wkbGeometryType.wkbPolyhedralSurfaceM:
                case wkbGeometryType.wkbTINM:
                case wkbGeometryType.wkbTriangleM:
                case wkbGeometryType.wkbMultiPolygonZM:
                case wkbGeometryType.wkbCurvePolygonZM:
                case wkbGeometryType.wkbMultiSurfaceZM:
                case wkbGeometryType.wkbSurfaceZM:
                case wkbGeometryType.wkbTriangleZM:
                case wkbGeometryType.wkbTINZM:
                    return GdGeometryType.Polygon;

                case wkbGeometryType.wkbLineString:
                case wkbGeometryType.wkbLineString25D:
                case wkbGeometryType.wkbMultiLineString25D:
                case wkbGeometryType.wkbCircularString:
                case wkbGeometryType.wkbCompoundCurve:
                case wkbGeometryType.wkbMultiCurve:
                case wkbGeometryType.wkbCurve:
                case wkbGeometryType.wkbLinearRing:
                case wkbGeometryType.wkbCircularStringZ:
                case wkbGeometryType.wkbCompoundCurveZ:
                case wkbGeometryType.wkbMultiCurveZ:
                case wkbGeometryType.wkbCurveZ:
                case wkbGeometryType.wkbLineStringM:
                case wkbGeometryType.wkbCircularStringM:
                case wkbGeometryType.wkbCompoundCurveM:
                case wkbGeometryType.wkbMultiLineStringM:
                case wkbGeometryType.wkbMultiCurveM:
                case wkbGeometryType.wkbCurveM:
                case wkbGeometryType.wkbLineStringZM:
                case wkbGeometryType.wkbCircularStringZM:
                case wkbGeometryType.wkbCompoundCurveZM:
                case wkbGeometryType.wkbMultiLineStringZM:
                case wkbGeometryType.wkbMultiCurveZM:
                case wkbGeometryType.wkbCurveZM:
                    return GdGeometryType.Line;
                default:
                    return null;
            }
        }

        internal static SpatialReference GetSpatialReference(int? srid)
        {
            if (!srid.HasValue)
                return null;

            ICoordinateSystem coordinateSystem = GdProjection.GetCrs(srid.Value);
            if (coordinateSystem == null)
                return null;

            SpatialReference reference = new SpatialReference(coordinateSystem.WKT);
            return reference;
        }

        internal static GdDataType GetDataType(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.OFTDate:
                case FieldType.OFTDateTime:
                case FieldType.OFTTime:
                    return GdDataType.Date;

                case FieldType.OFTBinary:
                    return GdDataType.Blob;

                case FieldType.OFTInteger:
                case FieldType.OFTInteger64:
                    return GdDataType.Integer;

                case FieldType.OFTReal:
                    return GdDataType.Real;

                default:
                    return GdDataType.String;
            }
        }

        internal static FieldType GetDataType(GdDataType fieldType)
        {
            switch (fieldType)
            {
                case GdDataType.Integer:
                case GdDataType.Boolean:
                    return FieldType.OFTInteger;

                case GdDataType.Real:
                    return FieldType.OFTReal;

                case GdDataType.String:
                    return FieldType.OFTString;

                case GdDataType.Date:
                    return FieldType.OFTDateTime;

                case GdDataType.Blob:
                    return FieldType.OFTBinary;
                
                case GdDataType.Geometry:
                    throw new Exception("Use Geometry Defination");
            }

            throw new Exception("Can not convert GdDataType to OgrDataType");
        }

        public static string ToOgrString(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return null;

                byte[] bytes = Encoding.Default.GetBytes(value);
                string result = Encoding.UTF8.GetString(bytes);
                return result;
            }
            catch
            {
                return value;
            }
        }
    }
}
