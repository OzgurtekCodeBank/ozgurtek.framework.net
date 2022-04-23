using ozgurtek.framework.common.Style;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Style
{
    public class GdPolygonStyleView : StackLayout
    {
        private readonly GdStyleVisibilityView _styleVisibilityPreview;
        private readonly GdIntegerEntry _strokeWidthInputView;
        private readonly GdColorView _strokeColorView;
        private readonly GdColorView _fillColorView;

        public GdPolygonStyleView()
        {
            Orientation = StackOrientation.Vertical;

            _styleVisibilityPreview = new GdStyleVisibilityView();
            Children.Add(_styleVisibilityPreview);

            GdViewBox polygonSymVb = new GdViewBox();
            polygonSymVb.Header = "Polygon Symbol";

            _strokeWidthInputView = new GdIntegerEntry();
            polygonSymVb.AddItem("Stroke Width", _strokeWidthInputView);

            _strokeColorView = new GdColorView();
            polygonSymVb.AddItem("Stroke Color", _strokeColorView);

            _fillColorView = new GdColorView();
            polygonSymVb.AddItem("Fill Color", _fillColorView);

            Children.Add(polygonSymVb);
        }

        public GdPolygonStyle PolygonStyle
        {
            get
            {
                int val = 0;
                if (_strokeWidthInputView.Value.HasValue)
                    val = _strokeWidthInputView.Value.Value;

                return new GdPolygonStyle
                {
                    Fill = new GdFill(_fillColorView.SelectedColor),
                    Stroke = new GdStroke(_strokeColorView.SelectedColor, val),
                    Visible = _styleVisibilityPreview.IsPreviewVisible,
                    MinScale = _styleVisibilityPreview.MinScale,
                    MaxScale = _styleVisibilityPreview.MaxScale
                };
            }
            set
            {
                if (value.Fill != null)
                {
                    _fillColorView.SelectedColor = value.Fill.Color;
                }

                if (value.Stroke != null)
                {
                    _strokeColorView.SelectedColor = value.Stroke.Color;
                    _strokeWidthInputView.Value = value.Stroke.Width;
                }

                _styleVisibilityPreview.IsPreviewVisible = value.Visible;
                _styleVisibilityPreview.MinScale = value.MinScale;
                _styleVisibilityPreview.MaxScale = value.MaxScale;
            }
        }
    }
}
