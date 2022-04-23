using System.Collections.Generic;

namespace ozgurtek.framework.core.Data
{
    public interface IGdSchema
    {
        /// <summary>
        /// Gets field by name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        IGdField GetFieldByName(string fieldName);

        /// <summary>
        /// Gets the field
        /// </summary>
        IEnumerable<IGdField> Fields { get; }

        /// <summary>
        /// Get field count
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the fields by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IGdField GetFieldByIndex(int index);
    }
}
