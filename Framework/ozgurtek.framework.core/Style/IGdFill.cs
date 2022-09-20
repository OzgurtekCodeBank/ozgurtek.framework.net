using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.core.Style
{
    public interface IGdFill
    {
        GdColor Color { get; set; }

        byte[] Material { get; set; }
    }
}
