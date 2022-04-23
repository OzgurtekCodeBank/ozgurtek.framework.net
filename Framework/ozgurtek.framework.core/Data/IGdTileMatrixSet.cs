using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace ozgurtek.framework.core.Data
{
    public interface IGdTileMatrixSet : IList<IGdTileMatrix>
    {
        string Name { get; set; }
        Envelope Envelope { get; set; }
        int Srid { get; set; }
        string WellKnownScaleSet { get; set; }
    }
}