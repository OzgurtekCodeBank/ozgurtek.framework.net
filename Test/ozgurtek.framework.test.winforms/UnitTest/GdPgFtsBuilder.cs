using System.Collections.Generic;

namespace ozgurtek.framework.test.winforms.UnitTest
{
    public class GdPgFtsBuilder : List<string>
    {
        public string SearchKey { get; set; }

        public string BuildWhereClause()
        {
            List<string> vector = new List<string>();
            foreach (string filterStr in this)
                vector.Add($"coalesce(cast({filterStr} as text), '')");

            string ftsFields = string.Join(" || ' ' || ", vector);
            string ftsValues = string.Join("&", SearchKey.Split(' '));
            return $"to_tsvector({ftsFields}) @@ to_tsquery(upper('{ftsValues}' collate pg_catalog.\"tr-TR-x-icu\"))";
        }
    }
}
