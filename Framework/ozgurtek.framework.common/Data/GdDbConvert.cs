using NetTopologySuite.Geometries;
using System;
using System.IO;
using System.Text;
using NetTopologySuite.IO;
using NetTopologySuite.IO.GML2;
using Newtonsoft.Json;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data
{
    public static class DbConvert
    {
        /// <summary>Converts the value of a specified object to an equivalent Boolean value.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>true or false, which reflects the value returned by invoking the <see cref="M:System.IConvertible.ToBoolean(System.IFormatProvider)"></see> method for the underlying type of <paramref name="value">value</paramref>. If <paramref name="value">value</paramref> is null, the method returns false.</returns>
        public static bool ToBoolean(object value)
        {
            return Convert.ToBoolean(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to an 8-bit unsigned integer.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value">value</paramref>, or zero if <paramref name="value">value</paramref> is null.</returns>
        public static byte ToByte(object value)
        {
            return Convert.ToByte(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to a Unicode character.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface.</param>
        /// <returns>A Unicode character that is equivalent to value, or <see cref="F:System.Char.MinValue"></see> if <paramref name="value">value</paramref> is null.</returns>
        public static char ToChar(object value)
        {
            return Convert.ToChar(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to a <see cref="T:System.DateTime"></see> object.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>The date and time equivalent of the value of <paramref name="value">value</paramref>, or a date and time equivalent of <see cref="F:System.DateTime.MinValue"></see> if <paramref name="value">value</paramref> is null.</returns>
        public static DateTime ToDateTime(object value)
        {
            return Convert.ToDateTime(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to a double-precision floating-point number.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>A double-precision floating-point number that is equivalent to <paramref name="value">value</paramref>, or zero if <paramref name="value">value</paramref> is null.</returns>
        public static double ToDouble(object value)
        {
            return Convert.ToDouble(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to an equivalent decimal number.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>A decimal number that is equivalent to <paramref name="value">value</paramref>, or 0 (zero) if <paramref name="value">value</paramref> is null.</returns>
        public static decimal ToDecimal(object value)
        {
            return Convert.ToDecimal(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to a 16-bit signed integer.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>A 16-bit signed integer that is equivalent to <paramref name="value">value</paramref>, or zero if <paramref name="value">value</paramref> is null.</returns>
        public static short ToInt16(object value)
        {
            return Convert.ToInt16(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to a 32-bit signed integer.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>A 32-bit signed integer equivalent to <paramref name="value">value</paramref>, or zero if <paramref name="value">value</paramref> is null.</returns>
        public static int ToInt32(object value)
        {
            return Convert.ToInt32(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to a 64-bit signed integer.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>A 64-bit signed integer that is equivalent to <paramref name="value">value</paramref>, or zero if <paramref name="value">value</paramref> is null.</returns>
        public static long ToInt64(object value)
        {
            return Convert.ToInt64(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to a single-precision floating-point number.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>A single-precision floating-point number that is equivalent to <paramref name="value">value</paramref>, or zero if <paramref name="value">value</paramref> is null.</returns>
        public static float ToSingle(object value)
        {
            return Convert.ToSingle(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to a 16-bit unsigned integer.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>A 16-bit unsigned integer that is equivalent to <paramref name="value">value</paramref>, or zero if <paramref name="value">value</paramref> is null.</returns>
        public static ushort ToUInt16(object value)
        {
            return Convert.ToUInt16(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to a 32-bit unsigned integer.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>A 32-bit unsigned integer that is equivalent to <paramref name="value">value</paramref>, or 0 (zero) if <paramref name="value">value</paramref> is null.</returns>
        public static uint ToUInt32(object value)
        {
            return Convert.ToUInt32(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to a 64-bit unsigned integer.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>A 64-bit unsigned integer that is equivalent to <paramref name="value">value</paramref>, or zero if <paramref name="value">value</paramref> is null.</returns>
        public static ulong ToUInt64(object value)
        {
            return Convert.ToUInt64(ToObject(value));
        }

        /// <summary>Converts the value of the specified object to a Guid.</summary>
        /// <param name="value">An object that implements the <see cref="T:System.IConvertible"></see> interface, or null.</param>
        /// <returns>returns guid, if null returns Guid.Empty</returns>
        public static Guid ToGuid(object value)
        {
            return IsDbNull(value) ? Guid.Empty : new Guid(Convert.ToString(value));
        }

        /// <summary>Converts the value of the specified object to its equivalent string representation.</summary>
        /// <param name="value">An object that supplies the value to convert, or null.</param>
        /// <returns>The string representation of <paramref name="value">value</paramref>, or <see cref="F:System.String.Empty"></see> if <paramref name="value">value</paramref> is null.</returns>
        public static string ToString(object value)
        {
            value = ToObject(value);
            if (value == null)
                return null;

            if (value is string str)
                return str;

            if (value is Geometry geom)
            {
                try
                {
                    return geom.ToText();
                }
                catch
                {
                    // ignored
                }
            }

            if (value is byte[] bytes)
            {
                try
                {
                    return Convert.ToBase64String(bytes);
                }
                catch
                {
                    // ignored
                }
            }

            try
            {
                return Convert.ToString(value);
            }
            catch
            {
                // ignored
            }

            return null;
        }

        public static Geometry ToGeometry(object value)
        {
            value = ToObject(value);
            if (value == null)
                return null;

            if (value is Geometry geom)
                return geom;

            if (value is string strVal)
            {
                try
                {
                    return FromJson(strVal);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    return FromWkt(strVal);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    return FromGml(strVal);
                }
                catch(Exception e)
                {
                    // ignored
                }
            }

            if (value is byte[] bytes)
            {
                try
                {
                    return FromWkb(bytes);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }

        public static byte[] ToBytes(object value)
        {
            value = ToObject(value);
            if (value == null)
                return null;

            if (value is byte[] bytes)
                return bytes;

            if (value is string str)
            {
                try
                {
                    return Convert.FromBase64String(str);
                }
                catch
                {
                    // ignored
                }
            }

            if (value is char[] chrs)
            {
                try
                {
                    string strval = new string(chrs);
                    return Convert.FromBase64String(strval);
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }

        public static object ConvertValue(object value, GdDataType type)
        {
            if (IsDbNull(value))
                return null;

            switch (type)
            {
                case GdDataType.Blob:
                    return ToBytes(value);
                case GdDataType.Geometry:
                    return ToGeometry(value);
                case GdDataType.String:
                    return ToString(value);
                case GdDataType.Boolean:
                    return ToBoolean(value);
                case GdDataType.Date:
                    return ToDateTime(value);
                case GdDataType.Integer:
                    return ToInt32(value);
                case GdDataType.Real:
                    return ToDouble(value);
                default:
                    return null;
            }
        }

        public static object FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return DBNull.Value;

            return value;
        }

        public static bool IsDbNull(object value)
        {
            return value == null || value == DBNull.Value;
        }

        public static string ToJson(Geometry geometry, GeometryFactory geomFactory = null, int dimension = 2)
        {
            GeometryFactory factory = geomFactory;
            if (factory == null)
                factory = GeometryFactory.Default;
            
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            JsonSerializer jsonSerializer = GeoJsonSerializer.Create(factory, dimension);
            jsonSerializer.Serialize(writer, geometry);
            writer.Flush();

            return sb.ToString();
        }

        public static byte[] ToWkb(Geometry geometry)
        {
            WKBWriter radeReader = new WKBWriter();
            radeReader.HandleOrdinates = Ordinates.XY;
            return radeReader.Write(geometry);
        }

        public static string ToWkt(Geometry geometry)
        {
            return geometry.ToText();
        }

        public static Geometry FromJson(string geometry)
        {
            GeoJsonReader reader = new GeoJsonReader();
            return reader.Read<Geometry>(geometry);
        }

        public static Geometry FromWkb(byte[] bytes)
        {
            WKBReader reader = new WKBReader();
            return reader.Read(bytes);
        }

        public static Geometry FromGml(string geometry)
        {
            GMLReader reader = new GMLReader();
            return reader.Read(geometry);
        }

        public static Geometry FromWkt(string geometry)
        {
            WKTReader reader = new WKTReader { IsOldNtsCoordinateSyntaxAllowed = false };
            return reader.Read(geometry);
        }

        public static string ToSqlSafeString(string value)
        {
            if (value == null)
                return null;

            string result = value;
            if (result.Contains("'"))
                result = result.Replace("'", "''");
            return result;
        }

        private static object ToObject(object value)
        {
            if (IsDbNull(value))
                return null;

            return value;
        }

        //public static bool IsDbNullOrWhiteSpace(object value)
        //{
        //    return IsDbNull(value) || value is string && string.IsNullOrWhiteSpace((string)value);
        //}
    }
}
