using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.core.Mapping
{
    public interface IGdRenderer
    {
        void Render(IGdRenderContext context, IGdTrack track = null);

        IGdStyle Style { set; get; }
    }
}