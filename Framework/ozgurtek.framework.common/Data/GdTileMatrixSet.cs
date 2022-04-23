using System.Collections.Generic;
using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data
{
    public class GdTileMatrixSet : List<IGdTileMatrix>, IGdTileMatrixSet
    {
        public string Name { get; set; }
        public Envelope Envelope { get; set; }
        public int Srid { get; set; }
        public string WellKnownScaleSet { get; set; }
    }
}
