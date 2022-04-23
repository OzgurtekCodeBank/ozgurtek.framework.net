using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;


namespace ozgurtek.framework.core.Data
{
    /// <summary>
    /// Describes a simple rows buffer
    /// </summary>
    public interface IGdRowBuffer
    {
        /// <summary>
        /// Adds a value to the set.
        /// </summary>
        ///  <param name="key">the name of the value to put</param>        
        ///  <param name="value">the data for the value to put</param>        
        void Put(string key, string value);

        /// <summary>
        /// Adds a value to the set.
        /// </summary>
        ///  <param name="key">the name of the value to put</param>        
        ///  <param name="value">the data for the value to put</param>        
        void Put(string key, int value);

        /// <summary>
        /// Adds a value to the set.
        /// </summary>
        ///  <param name="key">the name of the value to put</param>        
        ///  <param name="value">the data for the value to put</param>        
        void Put(string key, double value);

        /// <summary>
        /// Adds a value to the set.
        /// </summary>
        ///  <param name="key">the name of the value to put</param>        
        ///  <param name="value">the data for the value to put</param>        
        void Put(string key, byte[] value);

        /// <summary>
        /// Adds a value to the set.
        /// </summary>
        ///  <param name="key">the name of the value to put</param>        
        ///  <param name="value">the data for the value to put</param>        
        void Put(string key, Geometry value);

        /// <summary>
        /// Adds a value to the set.
        /// </summary>
        ///  <param name="key">the name of the value to put</param>        
        ///  <param name="value">the data for the value to put</param>        
        void Put(string key, DateTime value);

        /// <summary>
        /// Adds a value to the set.
        /// </summary>
        ///  <param name="key">the name of the value to put</param>        
        ///  <param name="value">the data for the value to put</param>        
        void Put(string key, bool value);

        /// <summary>
        /// Adds a null value to the set.
        /// </summary>
        ///  <param name="key">the name of the value to make null</param>
        void PutNull(string key);

        /// <summary>
        /// put with parameter
        /// </summary>
        ///  <param name="key">the name of the value to put</param>        
        ///  <param name="value">the data for the value to put</param>        
        /// <param name="dataType">DataType to force</param>
        void Put(string key, object value, GdDataType dataType);

        /// <summary>
        /// returns the number of values.
        /// </summary>
        /// <returns>number of values</returns>
        int Size();

        /// <summary>
        /// Remove a single value.
        /// </summary>
        ///  <param name="key">the name of the value to remove</param>
        void Remove(string key);

        /// <summary>
        /// Removes all values. 
        /// </summary>       
        void Clear();

        /// <summary>
        /// Returns true if this object has the named value.
        /// </summary>
        ///  <param name="key">key the value to check for</param>        
        /// <returns>{@code true} if the value is present, {@code false} otherwise</returns>
        bool ContainsKey(string key);

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
        int GetAsInteger(string key);

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
