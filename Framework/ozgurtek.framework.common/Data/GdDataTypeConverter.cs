using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;
using System;

namespace ozgurtek.framework.common.Data
{
    public class GdDataTypeConverter
    {
        public GdDataType ToGdDataType(Type type)
        {
            if (type == typeof(Geometry))
                return GdDataType.Geometry;

            TypeCode typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return GdDataType.Boolean;

                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return GdDataType.Integer;

                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return GdDataType.Real;

                case TypeCode.String:
                case TypeCode.Char:
                    return GdDataType.String;

                case TypeCode.DateTime:
                    return GdDataType.Date;

                case TypeCode.Object:
                case TypeCode.SByte:
                    return GdDataType.Blob;

                default:
                    throw new NotSupportedException();
            }
        }

        public Type ToDotNetType(GdDataType dataType)
        {
            switch (dataType)
            {
                case GdDataType.Integer:
                    return typeof(int);
                case GdDataType.Boolean:
                    return typeof(bool);
                case GdDataType.Blob:
                    return typeof(byte[]);
                case GdDataType.Date:
                    return typeof(DateTime);
                case GdDataType.String:
                    return typeof(string);
                case GdDataType.Real:
                    return typeof(double);
                case GdDataType.Geometry:
                    return typeof(Geometry);

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
