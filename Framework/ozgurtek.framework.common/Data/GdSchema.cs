using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;

namespace ozgurtek.framework.common.Data
{
    public class GdSchema : List<IGdField>, IGdSchema
    {
        public IGdField GetFieldByName(string fieldName)
        {
            foreach (IGdField field in this)
            {
                if (string.Compare(field.FieldName, fieldName, StringComparison.OrdinalIgnoreCase) == 0)
                    return field;
            }
            return null;
        }

        public IEnumerable<IGdField> Fields
        {
            get { return this; }
        }

        public IGdField GetFieldByIndex(int index)
        {
            return this[index];
        }
    }
}
