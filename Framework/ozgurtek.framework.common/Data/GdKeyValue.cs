using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data
{
    public class GdKeyValue : IGdKeyValue
    {
        private int _key;
        private string _value;

        public GdKeyValue()
        {
        }

        public GdKeyValue(int key, string value)
        {
            _key = key;
            _value = value;
        }

        public int Key
        {
            get => _key;
            set => _key = value;
        }

        public string Value
        {
            get => _value;
            set => _value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
