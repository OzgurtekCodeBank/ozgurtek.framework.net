using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Style;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Style
{
    public class GdImageStyleView : StackLayout
    {
        private readonly Slider _transparencySlider;
        private readonly GdStyleVisibilityView _styleVisibilityPreview;
        private readonly GdIntegerEntry _strokeWidthInputView;
        private readonly GdColorView _strokeColorView;

        public GdImageStyleView()
        {
            Orientation = StackOrientation.Vertical;

            _styleVisibilityPreview = new GdStyleVisibilityView();
            Children.Add(_styleVisibilityPreview);

            GdViewBox imageStyleViewBox = new GdViewBox();
            imageStyleViewBox.Header = "Image Symbol";

            _strokeWidthInputView = new GdIntegerEntry();
            imageStyleViewBox.AddItem("Stroke Width", _strokeWidthInputView);

            _strokeColorView = new GdColorView();
            imageStyleViewBox.AddItem("Stroke Color", _strokeColorView);

            _transparencySlider = new Slider(0, 1, 0.5);
            _transparencySlider.HorizontalOptions = LayoutOptions.FillAndExpand;
            _transparencySlider.MaximumTrackColor = Color.DimGray;
            _transparencySlider.MinimumTrackColor = Color.LightGray;
            _transparencySlider.ThumbColor = Color.Gray;
            imageStyleViewBox.AddItem("Transparency", _transparencySlider);

            Children.Add(imageStyleViewBox);
        }

        public GdImageStyle ImageStyle
        {
            get
            {
                GdImageStyle result = new GdImageStyle();
                result.Stroke = new GdStroke(_strokeColorView.SelectedColor, DbConvert.ToInt32(_strokeWidthInputView.Value));
                result.Transparent = _transparencySlider.Value;
                result.Visible = _styleVisibilityPreview.IsPreviewVisible;
                result.MinScale = _styleVisibilityPreview.MinScale;
                result.MaxScale = _styleVisibilityPreview.MaxScale;
                return result;
            }
            set
            {
                if (value.Stroke != null)
                {
                    _strokeColorView.SelectedColor = value.Stroke.Color;
                    _strokeWidthInputView.Value = value.Stroke.Width;
                }

                _transparencySlider.Value = value.Transparent;
                _styleVisibilityPreview.IsPreviewVisible = value.Visible;
                _styleVisibilityPreview.MinScale = value.MinScale;
                _styleVisibilityPreview.MaxScale = value.MaxScale;
            }
        }
    }
}
