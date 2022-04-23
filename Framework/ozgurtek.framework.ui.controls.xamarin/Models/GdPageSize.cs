namespace ozgurtek.framework.ui.controls.xamarin.Models
{
    public class GdPageSize
    {
        private readonly double _fixedSize;
        private double? _percentage;
        private readonly double _maxsize = double.MaxValue;
        private double _constant;

        public GdPageSize(double fixedSize)
        {
            _fixedSize = fixedSize;
        }

        public GdPageSize(double percentage, double maxsize, double constant = 0)
        {
            _percentage = percentage;
            _maxsize = maxsize;
            _constant = constant;
        }

        internal double GetCalculatedSize(double size)
        {
            if (_percentage.HasValue)
            {
                size = (size * _percentage.Value) + _constant;
                if (size > _maxsize)
                    size = _maxsize;
            }
            else
            {
                size = _fixedSize;
            }

            return size;
        }
    }
}
