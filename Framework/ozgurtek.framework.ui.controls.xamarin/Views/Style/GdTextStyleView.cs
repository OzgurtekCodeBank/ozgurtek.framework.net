using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Style;
using ozgurtek.framework.core.Style;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Style
{
    public class GdTextStyleView : StackLayout
    {
        private readonly GdStyleVisibilityView _styleVisibilityView;
        private readonly GdIntegerEntry _sizeInputView;
        private readonly GdIntegerEntry _strokeWitdh;
        private readonly GdColorView _strokeColorView;
        private readonly GdColorView _fillColorView;

        public GdTextStyleView()
        {
            Orientation = StackOrientation.Vertical;

            _styleVisibilityView = new GdStyleVisibilityView();
            Children.Add(_styleVisibilityView);

            GdViewBox box = new GdViewBox();
            box.Header = "Text Symbol";

            _sizeInputView = new GdIntegerEntry();
            box.AddItem("Size", _sizeInputView);

            _strokeWitdh = new GdIntegerEntry();
            box.AddItem("Stroke Width", _strokeWitdh);

            _strokeColorView = new GdColorView();
            box.AddItem("Stroke Color", _strokeColorView);

            _fillColorView = new GdColorView();
            box.AddItem("Fill Color", _fillColorView);

            Children.Add(box);
        }

        public IGdTextStyle TextStyle
        {
            get
            {
                GdTextStyle textStyle = new GdTextStyle();
                textStyle.Size = DbConvert.ToInt32(_sizeInputView.Value);

                textStyle.Stroke.Width = DbConvert.ToInt32(_strokeWitdh.Value);
                textStyle.Stroke.Color = _strokeColorView.SelectedColor;

                textStyle.Fill.Color = _fillColorView.SelectedColor;

                textStyle.Visible = _styleVisibilityView.IsPreviewVisible;
                textStyle.MinScale = _styleVisibilityView.MinScale;
                textStyle.MaxScale = _styleVisibilityView.MaxScale;

                return textStyle;
            }
            set
            {
                _sizeInputView.Value = value.Size;

                _styleVisibilityView.IsPreviewVisible = value.Visible;
                _styleVisibilityView.MinScale = value.MinScale;
                _styleVisibilityView.MaxScale = value.MaxScale;

                _strokeWitdh.Value = value.Stroke.Width;
                _strokeColorView.SelectedColor = value.Stroke.Color;

                _fillColorView.SelectedColor = value.Fill.Color;
            }
        }
    }
}
