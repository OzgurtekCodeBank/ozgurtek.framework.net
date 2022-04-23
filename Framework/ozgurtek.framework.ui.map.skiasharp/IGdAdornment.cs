using Xamarin.Forms;
using Point = NetTopologySuite.Geometries.Point;

namespace ozgurtek.framework.ui.map.skiasharp
{
    public interface  IGdAdornment
    {
        Point Point { get; }
        View View { get; }
    }
}
