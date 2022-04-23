using System.Collections.Generic;

namespace ozgurtek.framework.core.Data
{
    public interface IGdKeyValueSet
    {
        string Name { get; }

        IEnumerable<IGdKeyValue> KeyValues { get; }
    }
}
