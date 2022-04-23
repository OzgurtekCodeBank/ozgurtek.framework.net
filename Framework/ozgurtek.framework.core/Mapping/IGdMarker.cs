using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.core.Mapping
{
    public interface IGdMarker
    {
        void Render(IGdRenderContext context, IGdTrack track = null);
    }
}
