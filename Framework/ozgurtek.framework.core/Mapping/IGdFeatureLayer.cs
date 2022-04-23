using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.core.Mapping
{
    public interface IGdFeatureLayer : IGdLayer
    {
        IGdTable Table { get; }
    }
}
