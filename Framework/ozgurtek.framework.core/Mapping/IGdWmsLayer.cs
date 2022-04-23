using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.core.Mapping
{
    public interface IGdWmsLayer : IGdLayer
    {
        IGdWmsMap WmsMap { get; }
    }
}
