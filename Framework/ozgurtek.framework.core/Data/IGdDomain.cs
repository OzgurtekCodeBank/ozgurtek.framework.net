using System.Collections.Generic;

namespace ozgurtek.framework.core.Data
{
    public interface IGdDomain
    {
        IEnumerable<IGdKeyValueSet> GetDomain();

        IGdKeyValueSet GetDomain(string domainName);

        void AddDomain(string domainName);

        void AddKeyValue(string domainName, int code, string value);

        void DeleteDomain(string domainName);

        void UpdateDomain(string oldValue, string newValue);

        void DeleteKeyValue(string domain, int code);

        void AddFieldAsDomain(string tableName, string fieldName, string domainName);

        string GetDomainName(string tableName, string fieldName);
    }
}
