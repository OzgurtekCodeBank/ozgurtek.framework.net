using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data
{
    public class GdParameter : IGdParamater
    {
        private string _name;
        private GdDataType? _dataType;
        private object _value;

        public GdParameter()
        {
        }

        public GdParameter(string name, object value)
        {
            _name = name;
            _value = value;
        }

        public GdParameter(string name, object value, GdDataType? dataType)
        {
            _name = name;
            _dataType = dataType;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public GdDataType? DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}