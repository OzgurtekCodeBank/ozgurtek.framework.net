using System;
using System.Drawing;
using System.IO;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;
using Point = NetTopologySuite.Geometries.Point;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    public abstract class AbstractTableTest
    {
        protected double GetDouble()
        {
            return new Random().NextDouble();
        }

        protected int GetInt()
        {
            return (int)new Random().NextDouble() * 10;
        }

        protected byte[] GetBlob()
        {
            using (var stream = new MemoryStream())
            {
                Image img = new Bitmap(Properties.Resources.Image1);
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        protected Geometry GetGeoemetry()
        {
            string wkt =
                "POLYGON ((515325.117 4249061.9133, 515316.7958 4249063.8482, 515315.1865 4249062.8072, 515311.3131 4249051.8749, " +
                "515319.2943 4249049.8298, 515320.3645 4249049.5604, 515321.7947 4249049.1913, 515325.117 4249061.9133));";
            Geometry geometry = DbConvert.ToGeometry(wkt);
            geometry.SRID = 5253;

            return geometry;
        }

        protected string GetString()
        {
            return Guid.NewGuid().ToString();
        }

        protected DateTime GetDateTime()
        {
            return DateTime.Now;
        }

        protected bool GetBoolean()
        {
            Random rng = new Random();
            return rng.Next(0, 2) > 0;
        }

        protected GdMemoryTable GetMemoryTable(int rowcount = 0)
        {
            GdMemoryTable table = new GdMemoryTable();

            table.Name = "test_table";
            table.KeyField = "objectid";
            table.GeometryField = "geom_field";

            //int
            GdField field = new GdField();
            field.FieldName = "int_field";
            field.FieldType = GdDataType.Integer;
            field.PrimaryKey = true;
            field.NotNull = true;
            table.CreateField(field);

            //string
            field = new GdField();
            field.FieldName = "str_field";
            field.FieldType = GdDataType.String;
            table.CreateField(field);

            //double
            field = new GdField();
            field.FieldName = "double_field";
            field.FieldType = GdDataType.Real;
            table.CreateField(field);

            //blob
            field = new GdField();
            field.FieldName = "blob_field";
            field.FieldType = GdDataType.Blob;
            table.CreateField(field);

            //boolean
            field = new GdField();
            field.FieldName = "bool_field";
            field.FieldType = GdDataType.Boolean;
            table.CreateField(field);

            //date
            field = new GdField();
            field.FieldName = "date_field";
            field.FieldType = GdDataType.Date;
            table.CreateField(field);

            //geom
            field = new GdField();
            field.FieldName = "geom_field";
            field.FieldType = GdDataType.Geometry;
            table.CreateField(field);

            //shape
            field = new GdField();
            field.FieldName = "shape";
            field.FieldType = GdDataType.Geometry;
            table.CreateField(field);

            for (int i = 1; i <= rowcount; i++)
                table.Insert(GetOneRow());

            return table;
        }

        protected IGdRowBuffer GetOneRow()
        {
            Point geometry = new Point(GetDouble(), GetDouble());
            geometry.SRID = 5253;

            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put("int_field", GetInt());
            buffer.Put("str_field", Guid.NewGuid().ToString());
            buffer.Put("double_field", GetDouble());
            buffer.Put("blob_field", GetBlob());
            buffer.Put("bool_field", true);
            buffer.Put("date_field", DateTime.Now);
            buffer.Put("geom_field", geometry);
            buffer.Put("shape", geometry);
            return buffer;
        }
    }
}
