using System.Drawing;
using System.IO;
using ozgurtek.framework.common.Style;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.test.winforms.UnitTest.Style
{
    public class StyleColorDataRandomizer
    {
        public static GdColor GetSoftColor()
        {
            return GdColor.Soft;
        }

        public static GdColor GetSharpColor()
        {
            return GdColor.Sharp;
        }

        public static GdStroke GetStroke()
        {
            GdStroke stroke = new GdStroke();
            stroke.Width = 6;
            stroke.Color = GetSharpColor();
            return stroke;
        }

        public static GdFill GetMaterialFill()
        {
            GdFill fill = new GdFill();
            fill.Material = ImageToByteArray();
            return fill;
        }

        public static GdFill GetColorFill()
        {
            GdFill fill = new GdFill();
            fill.Color = GetSoftColor();
            return fill;
        }

        public static GdFill GetColorMaterialFill()
        {
            GdFill fill = new GdFill();
            fill.Color = GetSoftColor();
            fill.Material = ImageToByteArray();
            return fill;
        }

        public static byte[] ImageToByteArray()
        {
            using (var stream = new MemoryStream())
            {
                Image img = new Bitmap(Properties.Resources.Image1);
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
