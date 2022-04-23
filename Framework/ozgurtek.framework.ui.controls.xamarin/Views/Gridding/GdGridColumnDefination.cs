namespace ozgurtek.framework.ui.controls.xamarin.Views.Gridding
{
    public enum GdGridColumnType
    {
        Table,
        Custom,
        Calculated,
    }

    public class GdGridColumnDefination
    {
        private GdGridColumnType _type = GdGridColumnType.Table;
        private string _caption = string.Empty;
        private string _field;

        public GdGridColumnType Type
        {
            get => _type;
            set => _type = value;
        }

        public string Caption
        {
            get => _caption;
            set => _caption = value;
        }

        public string Field
        {
            get => _field;
            set => _field = value;
        }
    }
}
