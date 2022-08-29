using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace ozgurtek.framework.core.Data
{
    /// <summary>
    /// Describes a simple rows.
    /// </summary>
    public interface IGdRow
    {
        /// <summary>
        /// Gets the table of this  row
        /// </summary>
        IGdTable Table { get; }

        /// <summary>
        /// Gets a value and converts it to a String.
        /// </summary>
        ///  <param name="key">key the value to get</param>        
        /// <returns>the String for the value</returns>
        string GetAsString(string key);

        /// <summary>
        /// Gets a value and converts it to a Integer.
        /// </summary>
        ///  <param name="key">key the value to get</param>        
        /// <returns>the Integer for the value</returns>
        long GetAsInteger(string key);

        /// <summary>
        /// Gets a value and converts it to a Double.
        /// </summary>
        ///  <param name="key">key the value to get</param>        
        /// <returns>the Double for the value</returns>
        double GetAsReal(string key);

        /// <summary>
        /// Gets a value that is a byte array. Note that this method will not convert
        /// any other types to byte arrays.
        /// </summary>
        ///  <param name="key">key the value to get</param>        
        /// <returns>the {@code byte[]} value, or {@code null} is the value is missing or not a</returns>
        byte[] GetAsBlob(string key);

        /// <summary>
        /// Gets a value and converts it to a Date.
        /// </summary>
        ///  <param name="key">key the value to get</param>        
        /// <returns>the Date for the value</returns>
        DateTime GetAsDate(string key);

        /// <summary>
        /// Gets a value and converts it to a Boolean.
        /// </summary>
        ///  <param name="key">key the value to get</param>        
        /// <returns>the Boolean for the value</returns>
        bool GetAsBoolean(string key);

        /// <summary>
        /// Gets a value and converts it to a Geometry.
        /// </summary>
        ///  <param name="key">key the value to get</param>        
        /// <returns>the Geometry for the value</returns>
        Geometry GetAsGeometry(string key);

        /// <summary>
        /// Gets a value is null
        /// </summary>
        ///  <param name="key">key the value to get</param>        
        /// <returns>if true </returns>
        bool IsNull(string key);

        /// <summary>
        /// Get parameters
        /// </summary>
        IEnumerable<IGdParamater> Paramaters { get; }
    }
}
