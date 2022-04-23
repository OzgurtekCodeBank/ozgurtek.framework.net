using System.Collections.Generic;

namespace ozgurtek.framework.core.Data
{
    public interface IGdFilter
    {
        /// <summary>
        /// Gets query text.  
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameter"></param>
        void Add(IGdParamater parameter);

        /// <summary>
        /// Adds the parameter
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        void Add(string fieldName, object value);

        /// <summary>
        /// Adds the parameter
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="dataType"></param>
        void Add(string fieldName, object value, GdDataType dataType);

        /// <summary>
        /// ranges
        /// </summary>
        /// <param name="range"></param>
        void AddRange(IEnumerable<IGdParamater> range);

        /// <summary>
        /// Remove the parameter
        /// </summary>
        /// <param name="parameter"></param>
        bool Remove(IGdParamater parameter);

        /// <summary>
        /// Clears all parameter
        /// </summary>
        void Clear();

        /// <summary>
        /// gets count of the parameter
        /// </summary>
        int Count { get; }

        /// <summary>
        /// indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IGdParamater this[int index] { get; set; }

        /// <summary>
        /// Get all parameter
        /// </summary>
        IEnumerable<IGdParamater> Parameters { get; }
    }
}
