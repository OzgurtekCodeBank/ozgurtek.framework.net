using System;
using System.Collections.Generic;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.core.Mapping
{
    public interface IGdMap
    {
        IGdViewport Viewport { get; set; }

        IGdLayerCollection LayerCollection { get; set; }

        void Render();

        GdColor BackColor { set; get; }

        void AbortRender();

        IGdMapController Controller { get; set; }

        IList<IGdMarker> Markers { get; set; }
    }
}
