using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.core.Mapping
{
    public interface IGdTileLayer : IGdLayer
    {
        IGdTileMap TileMap { get; }
    }
}