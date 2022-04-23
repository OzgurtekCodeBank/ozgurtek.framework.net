using System;
using System.Collections.Generic;

namespace ozgurtek.framework.core.Mapping
{
    /// <summary>
    /// Describes a collection of layers.
    /// </summary>
    public interface IGdLayerCollection : IEnumerable<IGdLayer>
    {
        /// <summary>
        /// Gets the number of layers in this collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets or the layer at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the layer to get.</param>
        /// <returns>The layer at the specified index.</returns>
        IGdLayer this[int index] { get; }

        /// <summary>
        /// Adds a layer to this collection.
        /// </summary>
        /// <param name="layer">The layer to add.</param>
        void Add(IGdLayer layer);

        /// <summary>
        /// Adds set of layer to this collection
        /// </summary>
        /// <param name="layers"></param>
        void AddRange(IEnumerable<IGdLayer> layers);

        /// <summary>
        /// Removes all the layer from this collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether this collection contains the specified layer.
        /// </summary>
        /// <param name="layer">The layer to locate.</param>
        /// <returns>true if the layer is found, otherwise false.</returns>
        bool Contains(IGdLayer layer);

        /// <summary>
        /// Determines the index of a specific layer in this list.
        /// </summary>
        /// <param name="layer">The layer to locate in the list.</param>
        /// <returns>The index of layer if found in the list; otherwise, -1.</returns>
        int IndexOf(IGdLayer layer);

        /// <summary>
        /// Inserts a layer to this list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which layer should be inserted.</param>
        /// <param name="layer">The layer to insert.</param>
        void Insert(int index, IGdLayer layer);

        /// <summary>
        /// Attempts to remove the specified layer from this collection.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        /// <returns>true if the layer was successfully removed, otherwise false.</returns>
        bool Remove(IGdLayer layer);

        /// <summary>
        /// Removes the layer at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the layer to remove.</param>
        void RemoveAt(int index);

        /// <summary>
        /// Copies the layers of this List to a new array.
        /// </summary>
        /// <returns>An array containing copies of the layers of this list.</returns>
        IGdLayer[] ToArray();

        /// <summary>
        /// Gets or set active layer
        /// </summary>
        IGdLayer ActiveLayer { get; set; }

        /// <summary>
        /// Finds the layers having the specified name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>An array of layers if successful, otherwise null.</returns>
        IGdLayer[] FindLayer(string name);

        /// <summary>
        /// Occurs when a layer is added to this collection.
        /// </summary>
        event EventHandler<GdLayerEventArgs> LayerAdded;

        /// <summary>
        /// Occurs when a layer is removed from this collection.
        /// </summary>
        event EventHandler<GdLayerEventArgs> LayerRemoved;

        /// <summary>
        /// Occurs when all layers in this collection is removed.
        /// </summary>
        event EventHandler Reset;

        /// <summary>
        /// Occurs when an active layer changed
        /// </summary>
        event EventHandler ActiveLayerChanged;
    }
}
