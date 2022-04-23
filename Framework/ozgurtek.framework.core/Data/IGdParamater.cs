namespace ozgurtek.framework.core.Data
{
    public interface IGdParamater
    {
        /// <summary>
        /// Gets get or set the parameter name.  
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or set the data type of parameter
        /// </summary>
        GdDataType? DataType { get; set; }

        /// <summary>
        /// Get or set value of the parameter
        /// </summary>
        object Value { get; set; }
    }
}
