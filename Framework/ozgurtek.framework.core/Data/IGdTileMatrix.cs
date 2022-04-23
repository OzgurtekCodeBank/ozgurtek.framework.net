using NetTopologySuite.Geometries;

namespace ozgurtek.framework.core.Data
{
    public interface IGdTileMatrix
    {
        string Name { get; set; }
        int MatrixWidth { get; set; }
        int MatrixHeight { get; set; }
        int TileWidth { get; set; }
        int TileHeight { get; set; }
        double ScaleDenominator { get; set; }
        Coordinate TopLeftCorner { get; set; }
    }
}
