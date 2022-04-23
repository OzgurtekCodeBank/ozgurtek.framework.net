using ozgurtek.framework.core.Data;
using System.Collections.Generic;

namespace ozgurtek.framework.common.Data
{
    public class GdSqlFilter : List<IGdParamater>, IGdFilter
    {
        private string _filterText;

        public GdSqlFilter()
        {
        }

        public GdSqlFilter(string filterText)
        {
            _filterText = filterText;
        }

        public string Text
        {
            get { return _filterText; }
            set { _filterText = value; }
        }

        public void Add(string field, object value)
        {
            Add(new GdParameter(field, value));
        }

        public void Add(string field, object value, GdDataType type)
        {
            Add(new GdParameter(field, value, type));
        }

        public IEnumerable<IGdParamater> Parameters { get { return this; } }

        public override string ToString()
        {
            return Text;
        }
    }
}
