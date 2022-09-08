using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace ozgurtek.framework.core.Data
{
    public interface IGdTable
    {
        /// <summary>
        /// Gets the number of row in this layer.
        /// </summary>
        long RowCount { get; }

        /// <summary>
        /// Performs a search in this table
        /// </summary>
        /// <returns>Feature reader</returns>
        IEnumerable<IGdRow> Rows { get; }

        /// <summary>
        /// get envelope of table
        /// </summary>
        Envelope Envelope { get; }

        /// <summary>
        /// get geom type
        /// or null if not supported.
        /// </summary>
        GdGeometryType? GeometryType { get; set; }

        /// <summary>
        /// get srid of table, if not exits return <= 0
        /// </summary>
        int Srid { get; set; }

        /// <summary>
        /// Gets the name of the underlying database column being used as
        /// the geometry column, or null if not supported.
        /// </summary>
        string GeometryField { get; set; }

        /// <summary>
        /// Gets the name of this layer.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets description of table
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets or sets extras...
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// Gets the name of the underlying database column being used as
        /// the primary key, or null if not supported. 
        /// </summary>
        string KeyField { get; set; }

        /// <summary>
        /// returns schema in layer
        /// </summary>
        IGdSchema Schema { get; }

        /// <summary>
        /// search using identity field
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns>row</returns>
        IGdRow FindRow(long rowId);

        /// <summary>
        /// Returns distinct values for a given column
        /// </summary>
        ///  <param name="fieldName">field name</param>        
        /// <returns>set of object</returns>
        IEnumerable<object> GetDistinctValues(string fieldName);

        /// <summary>
        /// Set a new spatial filter
        /// </summary>
        IGdGeometryFilter GeometryFilter { get; set; }

        /// <summary>
        /// inserts given row
        /// </summary>
        /// <param name="row">row buffer</param>
        /// <returns>if key field is set, return value is keyfields value(serial column id)
        /// For example, when using a serial column to provide unique identifiers, 
        /// RETURNING can return the ID assigned to a new row
        /// </returns>
        long Insert(IGdRowBuffer row);

        /// <summary>
        /// updates given row
        /// </summary>
        /// <param name="row">row</param>
        long Update(IGdRowBuffer row);

        /// <summary>
        /// deletes given row
        /// </summary>
        /// <param name="id">row</param>
        long Delete(long id);

        /// <summary>
        /// Truncate table
        /// </summary>
        void Truncate();

        /// <summary>
        /// gets table updateable
        /// if false insert,delete,update trows exception
        /// </summary>
        bool CanEditRow { get; }

        /// <summary>
        /// create field
        /// </summary>
        /// <param name="field"></param>
        void CreateField(IGdField field);

        /// <summary>
        /// delete field
        /// </summary>
        /// <param name="field"></param>
        void DeleteField(IGdField field);

        /// <summary>
        /// gets table updateable
        /// if false CreateField,DeleteField throws exception
        /// </summary>
        bool CanEditField { get; }

        /// <summary>
        /// convert to geojson
        /// </summary>
        /// <returns></returns>
        string ToGeojson(GdGeoJsonSeralizeType type, int dimension = 2);

        /// <summary>
        /// Clones the structure of the table
        /// </summary>
        /// <returns></returns>
        IGdTable Clone();

        /// <summary>
        /// Occurs when row edit(insert, delete, update) of this table
        /// </summary>
        event EventHandler<GdRowChangedEventArgs> RowChanged;
    }
}
