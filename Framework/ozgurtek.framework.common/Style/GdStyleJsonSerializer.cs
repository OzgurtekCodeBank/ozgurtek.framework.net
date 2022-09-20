using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.common.Style
{
    public class GdStyleJsonSerializer
    {
        public string Serialize(IGdStyle style)
        {
            if (style is IGdPointStyle pointStyle)
                return Serialize(pointStyle);
            
            if (style is IGdPolygonStyle polygonStyle)
                return Serialize(polygonStyle);
            
            if (style is IGdLineStyle lineStyle)
                return Serialize(lineStyle);

            return null;
        }

        private string Serialize(IGdPointStyle style)
        {
            GdMemoryTable table = new GdMemoryTable();
            table.Name = "PointStyle";
            table.CreateField(new GdField("Stroke", GdDataType.String));
            table.CreateField(new GdField("Fill", GdDataType.String));
            table.CreateField(new GdField("Size", GdDataType.Integer));
            table.CreateField(new GdField("Type", GdDataType.String));

            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put("Stroke", Serialize(style.Stroke));
            buffer.Put("Fill", Serialize(style.Fill));
            buffer.Put("Size", style.Size);
            buffer.Put("Type", style.PointStleType.ToString());

            table.Insert(buffer);

            return table.ToGeojson(GdGeoJsonSeralizeType.OnlyData);
        }

        private string Serialize(IGdLineStyle lineStyle)
        {
            GdMemoryTable table = new GdMemoryTable();
            table.Name = "LineStyle";
            table.CreateField(new GdField("Stroke", GdDataType.String));

            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put("Stroke", Serialize(lineStyle.Stroke));
            table.Insert(buffer);

            return table.ToGeojson(GdGeoJsonSeralizeType.OnlyData);
        }

        private string Serialize(IGdPolygonStyle polygonStyle)
        {
            GdMemoryTable table = new GdMemoryTable();
            table.Name = "PolygonStyle";
            table.CreateField(new GdField("Stroke", GdDataType.String));
            table.CreateField(new GdField("Fill", GdDataType.String));

            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put("Stroke", Serialize(polygonStyle.Stroke));
            buffer.Put("Fill", Serialize(polygonStyle.Fill));
            table.Insert(buffer);

            return table.ToGeojson(GdGeoJsonSeralizeType.OnlyData);
        }

        private string Serialize(IGdFill fill)
        {
            GdMemoryTable table = new GdMemoryTable();
            table.Name = "Fill";
            table.CreateField(new GdField("Color", GdDataType.String));
            table.CreateField(new GdField("Material", GdDataType.Blob));

            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put("Color", Serialize(fill.Color));
            buffer.Put("Material", fill.Material);

            table.Insert(buffer);

            return table.ToGeojson(GdGeoJsonSeralizeType.OnlyData);
        }

        private string Serialize(IGdStroke stroke)
        {
            GdMemoryTable table = new GdMemoryTable();
            table.Name = "Stroke";
            table.CreateField(new GdField("Width", GdDataType.Integer));
            table.CreateField(new GdField("Color", GdDataType.String));

            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put("Width", stroke.Width);
            buffer.Put("Color", Serialize(stroke.Color));
            table.Insert(buffer);

            return table.ToGeojson(GdGeoJsonSeralizeType.OnlyData);
        }

        private string Serialize(GdColor color)
        {
            GdMemoryTable table = new GdMemoryTable();
            table.Name = "Color";
            table.CreateField(new GdField("R", GdDataType.Integer));
            table.CreateField(new GdField("G", GdDataType.Integer));
            table.CreateField(new GdField("B", GdDataType.Integer));
            table.CreateField(new GdField("A", GdDataType.Integer));

            GdRowBuffer buffer = new GdRowBuffer();
            buffer.Put("R", color.R);
            buffer.Put("G", color.G);
            buffer.Put("B", color.B);
            buffer.Put("A", color.A);
            table.Insert(buffer);

            return table.ToGeojson(GdGeoJsonSeralizeType.OnlyData);
        }
    }
}
