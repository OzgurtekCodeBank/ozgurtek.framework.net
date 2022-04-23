using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;

namespace ozgurtek.framework.common.Mapping
{
    public class GdWmsLayer : GdAbstractLayer, IGdWmsLayer
    {
        private readonly IGdWmsMap _wmsMap;

        public GdWmsLayer(IGdWmsMap wmsMap)
        {
            _wmsMap = wmsMap;
            _renderer = new GdSimpleWebMapRenderer(this);
        }

        public GdWmsLayer(IGdWmsMap wmsMap, string name)
        {
            _wmsMap = wmsMap;
            _name = name;
            _renderer = new GdSimpleWebMapRenderer(this);
        }

        public IGdWmsMap WmsMap
        {
            get { return _wmsMap; }
        }
    }
}
