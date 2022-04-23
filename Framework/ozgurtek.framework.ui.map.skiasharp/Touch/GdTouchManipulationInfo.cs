using SkiaSharp;

namespace ozgurtek.framework.ui.map.skiasharp.Touch
{
    public class GdTouchManipulationInfo
    {
        public SKPoint PreviousPoint { set; get; }

        public SKPoint NewPoint { set; get; }
    }
}