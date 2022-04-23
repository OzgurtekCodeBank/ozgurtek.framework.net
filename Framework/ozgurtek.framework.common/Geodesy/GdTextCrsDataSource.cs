using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ozgurtek.framework.common.Geodesy
{
    public class GdTextCrsDataSource : IGdCrsDataSource
    {
        public bool CanEdit
        {
            get
            {
                return false;
            }
        }

        public void Add(int code, string defination)
        {
            throw new NotSupportedException("Look CanEdit property, not editable data source");
        }

        public IEnumerable<IGdKeyValue> GetDefination()
        {
            Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("ozgurtek.framework.common.Resources.srid.csv");

            using (var sr = new StreamReader(stream, Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    int split = line.IndexOf(';');
                    if (split <= -1)
                        continue;

                    GdKeyValue keyValue = new GdKeyValue
                    {
                        Key = int.Parse(line.Substring(0, split)),
                        Value = line.Substring(split + 1)
                    };
                    yield return keyValue;
                }
            }
        }

        public IGdKeyValue GetDefination(int key)
        {
            foreach (IGdKeyValue wkt in GetDefination())
                if (wkt.Key == key)
                    return wkt;
            return null;
        }

        public string CrsType
        {
            get
            {
                return "EPSG";
            }
        }
    }
}
