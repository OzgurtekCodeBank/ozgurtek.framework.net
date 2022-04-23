using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Util
{
    public class GdLabelFormatBuilder
    {
        private readonly IGdTable _table;

        public GdLabelFormatBuilder(IGdTable table)
        {
            _table = table;
        }

        public string CreateFormat()
        {
            IGdSchema schema = _table.Schema;
            foreach (IGdField field in schema.Fields) //first string field
            {
                if (field.FieldType == GdDataType.String)
                    return $"[{field.FieldName}]";
            }

            if (!string.IsNullOrWhiteSpace(_table.KeyField))//key field
                return $"[{_table.KeyField}]";

            IGdField byIndex = schema.GetFieldByIndex(0);//first field
            return $"[{byIndex.FieldName}]";
        }

        public string ResolveFormat(IGdRow row, string format)
        {
            string label = format;
            foreach (IGdField field in row.Table.Schema.Fields)
            {
                string find = $"[{field.FieldName}]";

                string replace = "null";
                if (!row.IsNull(field.FieldName))
                    replace = row.GetAsString(field.FieldName);

                label = label.Replace(find, replace);
            }
            return label;
        }
    }
}
