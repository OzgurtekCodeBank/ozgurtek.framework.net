using System.Linq;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.common.Style
{
    public class GdStyleJsonDeSerializer
    {
        public IGdStyle DeSerialize(string value)
        {
            GdMemoryTable memoryTable = GdMemoryTable.LoadFromJson(value);
            if (memoryTable.Name == "PointStyle")
                return ParsePointStyle(memoryTable);

            if (memoryTable.Name == "PolygonStyle")
                return ParsePolygonStyle(value);

            if (memoryTable.Name == "LineStyle")
                return ParseLineStyle(value);

            return null;
        }

        private IGdStyle ParseLineStyle(string value)
        {
            GdMemoryTable memoryTable = GdMemoryTable.LoadFromJson(value);
            IGdRow row = memoryTable.Rows.FirstOrDefault();
            if (row == null)
                return null;

            GdLineStyle style = new GdLineStyle();
            style.Stroke = ParseStroke(row.GetAsString("Stroke"));

            return style;
        }

        private IGdStyle ParsePolygonStyle(string value)
        {
            GdMemoryTable memoryTable = GdMemoryTable.LoadFromJson(value);
            IGdRow row = memoryTable.Rows.FirstOrDefault();
            if (row == null)
                return null;

            GdPolygonStyle style = new GdPolygonStyle();
            style.Stroke = ParseStroke(row.GetAsString("Stroke"));
            style.Fill = ParseFill(row.GetAsString("Fill"));

            return style;
        }

        private IGdStyle ParsePointStyle(string value)
        {
            GdMemoryTable memoryTable = GdMemoryTable.LoadFromJson(value);
            IGdRow row = memoryTable.Rows.FirstOrDefault();
            if (row == null)
                return null;

            GdPointStyle style = new GdPointStyle();
            style.Stroke = ParseStroke(row.GetAsString("Stroke"));
            style.Fill = ParseFill(row.GetAsString("Fill"));
            style.PointStleType = 

        }

        private IGdStroke ParseStroke(string value)
        {
            GdMemoryTable memoryTable = GdMemoryTable.LoadFromJson(value);
            IGdRow row = memoryTable.Rows.FirstOrDefault();
            if (row == null)
                return null;

            GdStroke stroke = new GdStroke();
            stroke.Width = (int)row.GetAsInteger("Width");
            stroke.Color = ParseColor(row.GetAsString("Color"));

            return stroke;
        }

        private IGdFill ParseFill(string value)
        {
            GdMemoryTable memoryTable = GdMemoryTable.LoadFromJson(value);
            IGdRow row = memoryTable.Rows.FirstOrDefault();
            if (row == null)
                return null;

            GdFill fill = new GdFill();
            fill.Color = ParseColor(row.GetAsString("Color"));
            fill.Material = row.GetAsBlob("Material");

            return fill;
        }

        private GdColor ParseColor(string value)
        {
            GdMemoryTable memoryTable = GdMemoryTable.LoadFromJson(value);
            IGdRow row = memoryTable.Rows.FirstOrDefault();

            GdColor color = new GdColor();
            color.R = (byte)row.GetAsInteger("R");
            color.G = (byte)row.GetAsInteger("G");
            color.B = (byte)row.GetAsInteger("B");
            color.A = (byte)row.GetAsInteger("A");

            return color;
        }

    }
}
