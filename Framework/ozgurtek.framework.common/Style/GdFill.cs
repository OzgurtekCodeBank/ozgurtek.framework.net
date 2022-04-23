using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.common.Style
{
    public class GdFill : IGdFill
    {
        GdColor _color = new GdColor();

        public GdFill()
        {
        }

        public GdFill(GdColor color)
        {
            _color = color;
        }

        public GdColor Color
        {
            get => _color;
            set => _color = value;
        }

        public static GdFill Soft
        {
            get { return new GdFill(GdColor.Soft); }
        }

        public static GdFill Sharp
        {
            get { return new GdFill(GdColor.Sharp); }
        }
    }
}