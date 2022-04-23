using ozgurtek.framework.core.Data;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Gridding
{
    public class GdLabelGridCell : GdAbstractGridCell
    {
        public Label Label;

        public GdLabelGridCell(IGdTable table, IGdRow row, int rowNum, int colNum) :
            base(table, row, rowNum, colNum)
        {
            CornerRadius = 0;
            Padding = 3;
            Margin = 0;
            BorderColor = Color.Black;
            HasShadow = false;

            Label label = new Label
            {
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
            };

            Content = label;
            Label = label;
        }

        public string Text
        {
            get { return Label.Text; }
            set { Label.Text = value; }
        }
    }
}