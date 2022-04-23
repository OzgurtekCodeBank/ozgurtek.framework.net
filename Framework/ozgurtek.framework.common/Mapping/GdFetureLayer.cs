using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;

namespace ozgurtek.framework.common.Mapping
{
    public class GdFetureLayer : GdAbstractLayer, IGdFeatureLayer, IGdLabeledLayer
    {
        private readonly IGdTable _table;
        private IGdRenderer _labelRenderer;
        private string _labelFormat;

        public GdFetureLayer(IGdTable table)
        {
            _table = table;
            InitRenderer();
        }

        public GdFetureLayer(IGdTable table, string name)
        {
            _table = table;
            _name = name;
            InitRenderer();
        }

        private void InitRenderer()
        {
            if (!string.IsNullOrWhiteSpace(_table.GeometryField))
                _renderer = new GdSimpleFeatureRenderer(this, GdRenderMode.Geometry);

            if (!string.IsNullOrWhiteSpace(_table.GeometryField))
                _labelRenderer = new GdSimpleFeatureRenderer(this, GdRenderMode.Label);

            if (_labelRenderer != null && _labelRenderer.Style != null)
                _labelRenderer.Style.Visible = false;
        }

        public IGdTable Table
        {
            get { return _table; }
        }

        public IGdRenderer LabelRenderer
        {
            get => _labelRenderer;
            set => _labelRenderer = value;
        }

        public string LabelFormat
        {
            get
            {
                if (_labelFormat != null)
                    return _labelFormat;

                GdLabelFormatBuilder formatBuilder = new GdLabelFormatBuilder(_table);
                return _labelFormat = formatBuilder.CreateFormat();
            }
            set
            {
                _labelFormat = value;
            }
        }
    }
}