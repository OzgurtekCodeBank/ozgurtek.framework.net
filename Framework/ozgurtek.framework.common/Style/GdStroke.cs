using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.common.Style
{
    public class GdStroke : IGdStroke
    {
        GdColor _color = new GdColor();
        private int _width = 1;

        public GdStroke()
        {
        }

        public GdStroke(GdColor color)
        {
            _color = color;
        }

        public GdStroke(GdColor color, int width)
        {
            _color = color;
            _width = width;
        }

        public GdColor Color
        {
            get => _color;
            set => _color = value;
        }

        public int Width
        {
            get => _width;
            set => _width = value;
        }

        public static GdStroke Soft
        {
            get { return new GdStroke(GdColor.Soft); }
        }

        public static GdStroke Sharp
        {
            get { return new GdStroke(GdColor.Sharp); }
        }
    }
}