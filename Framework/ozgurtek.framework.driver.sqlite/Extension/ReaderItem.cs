using System;
using System.Collections.Generic;
using System.Linq;

namespace ozgurtek.framework.driver.sqlite.Extension
{
    internal class ReaderItem
    {
        private readonly IDictionary<string, object> data;
        private readonly List<Type> _types;

        public ReaderItem()
        {
            data = new Dictionary<string, object>();
            _types = new List<Type>();
        }

        public object this[string propertyName]
        {
            get
            {
                if (data.ContainsKey(propertyName))
                    return data[propertyName];
                else
                    return null;
            }
            set
            {
                if (data.ContainsKey(propertyName))
                    data[propertyName] = value;
                else
                    data.Add(propertyName, value);
            }
        }

        /// <summary>
        /// Get column names
        /// </summary>
        public List<string> Fields
        {
            get
            {
                return data.Keys.ToList();
            }
        }

        public List<Type> Columns
        {
            get { return _types; }
        }
    }
}
