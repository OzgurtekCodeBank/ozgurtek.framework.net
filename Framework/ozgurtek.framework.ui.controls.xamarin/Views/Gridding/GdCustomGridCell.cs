using ozgurtek.framework.core.Data;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Gridding
{
    public class GdCustomGridCell : GdAbstractGridCell
    {
        private View _view;

        public GdCustomGridCell(IGdTable table, IGdRow row, int rowNum, int colNum)
           : base(table, row, rowNum, colNum)
        {
            CornerRadius = 0;
            Padding = 3;
            Margin = 0;
            BorderColor = Color.Black;
            HasShadow = false;
        }

        public View View
        {
            get
            {
                return _view;
            }
            set
            {
                Content = value;
                _view = value;
            }
        }
    }
}
