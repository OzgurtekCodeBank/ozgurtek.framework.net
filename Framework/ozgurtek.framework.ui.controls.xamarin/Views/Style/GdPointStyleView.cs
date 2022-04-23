using ozgurtek.framework.common.Style;
using ozgurtek.framework.core.Style;
using System;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Style
{
    public class GdPointStyleView : StackLayout
    {
        private readonly GdPicker _pointStyleTypePicker;
        private readonly GdIntegerEntry _sizeInput;
        private readonly GdStyleVisibilityView _styleVisibilityPreview;
        private readonly GdIntegerEntry _strokeWidthInputView;
        private readonly GdColorView _strokeColorView;
        private readonly GdColorView _fillColorView;

        public GdPointStyleView()
        {
            Orientation = StackOrientation.Vertical;

            _styleVisibilityPreview = new GdStyleVisibilityView();
            Children.Add(_styleVisibilityPreview);

            GdViewBox pointSymbol = new GdViewBox();
            pointSymbol.Header = "Point Symbol";

            _pointStyleTypePicker = new GdPicker();
            _pointStyleTypePicker.HorizontalOptions = LayoutOptions.FillAndExpand;

            _pointStyleTypePicker.CreateItem(GdPointStyleType.Circle.ToString(), null, (int)GdPointStyleType.Circle);
            _pointStyleTypePicker.CreateItem(GdPointStyleType.Square.ToString(), null, (int)GdPointStyleType.Square);

            //_pointStyleTypePicker.ItemsSource = Enum.GetValues(typeof(GdPointStyleType));
            pointSymbol.AddItem("Point Style", _pointStyleTypePicker);

            _sizeInput = new GdIntegerEntry();
            pointSymbol.AddItem("Size", _sizeInput);

            _strokeWidthInputView = new GdIntegerEntry();
            pointSymbol.AddItem("Stroke Width", _strokeWidthInputView);

            _strokeColorView = new GdColorView();
            pointSymbol.AddItem("Stroke Color", _strokeColorView);

            _fillColorView = new GdColorView();
            pointSymbol.AddItem("Fill Color", _fillColorView);

            Children.Add(pointSymbol);
        }

        public GdPointStyle PointStyle
        {
            get
            {
                return new GdPointStyle()
                {
                    Fill = new GdFill(_fillColorView.SelectedColor),
                    Stroke = new GdStroke(_strokeColorView.SelectedColor, (int)_strokeWidthInputView.Value),
                    PointStleType = (GdPointStyleType)_pointStyleTypePicker.SelectedItem.ItemId,
                    Size = (int)_sizeInput.Value,
                    Visible = _styleVisibilityPreview.IsPreviewVisible,
                    MinScale = _styleVisibilityPreview.MinScale,
                    MaxScale = _styleVisibilityPreview.MaxScale
                };
            }
            set
            {
                _pointStyleTypePicker.SelectedId = (int) value.PointStleType;
                _sizeInput.Value = value.Size;
                _styleVisibilityPreview.IsPreviewVisible = value.Visible;
                _styleVisibilityPreview.MinScale = value.MinScale;
                _styleVisibilityPreview.MaxScale = value.MaxScale;

                if (value.Stroke != null)
                {
                    _strokeColorView.SelectedColor = value.Stroke.Color;
                    _strokeWidthInputView.Value = value.Stroke.Width;
                }

                if (value.Fill != null)
                    _fillColorView.SelectedColor = value.Fill.Color;
            }
        }
    }
}