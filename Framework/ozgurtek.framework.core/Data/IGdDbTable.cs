namespace ozgurtek.framework.core.Data
{
    /// <summary>
    /// Describes a table of simple rows.
    /// </summary>
    public interface IGdDbTable : IGdTable
    {
        /// <summary>
        /// Gets or sets sql filter
        /// </summary>
        IGdFilter SqlFilter { get; set; }

        /// <summary>
        /// Gets or set sql property filter
        /// </summary>
        string ColumnFilter { get; set; }

        /// <summary>
        /// Gets or set sql property filter
        /// </summary>
        string OrderBy { get; set; }

        /// <summary>
        /// Gets set the table limit
        /// </summary>
        long Limit { get; set; }

        /// <summary>
        /// Gets set the table row fetch offset
        /// </summary>
        long Offset { get; set; }

        /// <summary>
        /// gets table transaction
        /// if false CanSupportTransaction,CommitTransaction,RollBackTransaction trows exception
        /// </summary>
        bool CanSupportTransaction { get; }

        /// <summary>
        /// Transaction open
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits all edit
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Rollbacks all edit
        /// </summary>
        void RollBackTransaction();
    }
}