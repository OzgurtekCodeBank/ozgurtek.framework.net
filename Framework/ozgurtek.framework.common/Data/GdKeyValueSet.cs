using ozgurtek.framework.core.Data;
using System.Collections.Generic;

namespace ozgurtek.framework.common.Data
{
    public class GdKeyValueSet : IGdKeyValueSet
    {
        public GdKeyValueSet()
        {
        }

        public GdKeyValueSet(string name)
        {
            Name = name;
        }

        public GdKeyValueSet(IEnumerable<IGdKeyValue> keyValues)
        {
            KeyValues = keyValues;
        }

        public GdKeyValueSet(string name, IEnumerable<IGdKeyValue> keyValues)
        {
            Name = name;
            KeyValues = keyValues;
        }


        public string Name { get; }
        public IEnumerable<IGdKeyValue> KeyValues { get; }
    }
}
