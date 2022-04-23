using ozgurtek.framework.core.Mapping;
using System;
using System.Collections.Generic;

namespace ozgurtek.framework.common.Mapping
{
    /// <summary>
    /// Provides the data for LayerAdded and LayerRemoved events of a layer collection.
    /// </summary>
    public class GdLayerEventArgs : EventArgs
    {
        private readonly IEnumerable<IGdLayer> _layers;

        /// <summary>
        /// Initializes a new McxLayerEventArgs class using the specified map, layer and index.
        /// </summary>
        /// <param name="layers">A layer that raised the event.</param>
        public GdLayerEventArgs(IEnumerable<IGdLayer> layers)
        {
            _layers = layers;
        }

        /// <summary>
        /// Gets the layer that raised the event.
        /// </summary>
        public IEnumerable<IGdLayer> Layers
        {
            get { return _layers; }
        }
    }
}
