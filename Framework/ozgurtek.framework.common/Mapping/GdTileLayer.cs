using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;

namespace ozgurtek.framework.common.Mapping
{
    public class GdTileLayer : GdAbstractLayer, IGdTileLayer
    {
        private readonly IGdTileMap _map;

        public GdTileLayer(IGdTileMap map)
        {
            _map = map;
            _renderer = new GdSimpleTileRenderer(this);
        }

        public GdTileLayer(IGdTileMap map, string name)
        {
            _map = map;
            _name = name;
            _renderer = new GdSimpleTileRenderer(this);
        }

        public IGdTileMap TileMap
        {
            get { return _map; }
        }
    }
}
