using System.Collections.Generic;

namespace ozgurtek.framework.core.Data
{
    public interface IGdCrsDataSource
    {
        bool CanEdit { get; }

        void Add(int code, string defination);

        IEnumerable<IGdKeyValue> GetDefination();

        IGdKeyValue GetDefination(int key);

        string CrsType { get; }
    }
}
