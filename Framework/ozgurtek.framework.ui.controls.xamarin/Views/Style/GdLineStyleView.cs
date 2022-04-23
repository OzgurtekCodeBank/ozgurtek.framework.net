using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Style;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Style
{
    public class GdLineStyleView : StackLayout
    {
        private readonly GdStyleVisibilityView _styleVisibilityPreview;
        private readonly GdIntegerEntry _strokeWidthInputView;
        private readonly GdColorView _strokeColorView;

        public GdLineStyleView()
        {
            Orientation = StackOrientation.Vertical;

            _styleVisibilityPreview = new GdStyleVisibilityView();
            Children.Add(_styleVisibilityPreview);

            GdViewBox lineViewBox = new GdViewBox();
            lineViewBox.Header = "Line Symbol";

            _strokeWidthInputView = new GdIntegerEntry();
            lineViewBox.AddItem("Stroke Width", _strokeWidthInputView);

            _strokeColorView = new GdColorView();
            lineViewBox.AddItem("Stroke Color", _strokeColorView);

            Children.Add(lineViewBox);
        }

        public GdLineStyle LineStyle
        {
            get
            {
                return new GdLineStyle
                {
                    Stroke = new GdStroke(_strokeColorView.SelectedColor, DbConvert.ToInt32(_strokeWidthInputView.Value)),
                    Visible = _styleVisibilityPreview.IsPreviewVisible,
                    MinScale = _styleVisibilityPreview.MinScale,
                    MaxScale = _styleVisibilityPreview.MaxScale
                };
            }
            set
            {
                _strokeColorView.SelectedColor = value.Stroke.Color;
                _strokeWidthInputView.Value = value.Stroke.Width;
                _styleVisibilityPreview.IsPreviewVisible = value.Visible;
                _styleVisibilityPreview.MinScale = value.MinScale;
                _styleVisibilityPreview.MaxScale = value.MaxScale;
            }
        }
    }
}