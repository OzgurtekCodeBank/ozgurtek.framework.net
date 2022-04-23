using System;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Style
{
    public class GdStyleVisibilityView : GdViewBox
    {
        private readonly GdCheckBox _isVisibleCheckBox;
        private readonly GdDoubleEntry _minScaleInput;
        private readonly GdDoubleEntry _maxScaleInput;

        public GdStyleVisibilityView()
        {
            Header = "Visibility";

            _isVisibleCheckBox = new GdCheckBox
            {
                IsChecked = true
            };
            AddItem("Visible", _isVisibleCheckBox);

            _minScaleInput = new GdDoubleEntry();
            AddItem("Min Scale 1/", _minScaleInput);

            _maxScaleInput = new GdDoubleEntry();
            AddItem("Max Scale 1/", _maxScaleInput);
        }

        public bool IsPreviewVisible
        {
            get { return _isVisibleCheckBox.IsChecked; }
            set { _isVisibleCheckBox.IsChecked = value; }
        }

        public double? MinScale
        {
            get
            {
                if (_minScaleInput.Value == null || string.IsNullOrWhiteSpace(_minScaleInput.Value.ToString()))
                    return null;

                return Convert.ToDouble(_minScaleInput.Value.ToString());
            }
            set
            {
                if (value == null)
                    _minScaleInput.Value = null;

                _minScaleInput.Value = value;
            }
        }

        public double? MaxScale
        {
            get
            {
                if (_maxScaleInput.Value == null || string.IsNullOrWhiteSpace(_maxScaleInput.Value.ToString()))
                    return null;

                return Convert.ToDouble(_maxScaleInput.Value);
            }
            set
            {
                if (value == null)
                    _maxScaleInput.Value = null;

                _maxScaleInput.Value = value;
            }
        }
    }
}
