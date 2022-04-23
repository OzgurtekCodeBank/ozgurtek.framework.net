using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data
{
    public class GdTileMatrix : IGdTileMatrix
    {
        public string Name { get; set; }
        public int MatrixWidth { get; set; }
        public int MatrixHeight { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public double ScaleDenominator { get; set; }
        public Coordinate TopLeftCorner { get; set; }
    }
}
