using System;
using NetTopologySuite.Geometries;

namespace ozgurtek.framework.common.Data.Format.OnlineMap.Google
{
    public class GdGoogleMapsTileMatrixSet : GdTileMatrixSet
    {
        public GdGoogleMapsTileMatrixSet()
        {
            Srid = 3857;
            Envelope = new Envelope(-20026376.39, 20026376.39, -20048966.10, 20048966.10);
            WellKnownScaleSet = "GoogleMapsCompatible";
            Name = "GoogleMapsCompatible";

            double[] scales = {
                559082264.0287178,
                279541132.0143589,
                139770566.0071794,
                69885283.00358972,
                34942641.50179486,
                17471320.75089743,
                8735660.375448715,
                4367830.187724357,
                2183915.093862179,
                1091957.546931089,
                545978.7734655447,
                272989.3867327723,
                136494.6933663862,
                68247.34668319309,
                34123.67334159654,
                17061.83667079827,
                8530.918335399136,
                4265.459167699568,
                2132.729583849784,
                //1066.364791924892
            };

            for (int i = 0; i < 19; i++)
            {
                GdTileMatrix matrix = new GdTileMatrix();
                matrix.Name = i.ToString();
                matrix.MatrixHeight = matrix.MatrixWidth = (int)Math.Pow(2, i);
                matrix.TileWidth = matrix.TileHeight = 256;
                matrix.TopLeftCorner = new Coordinate(20037508.34278925, -20037508.34278925);
                matrix.ScaleDenominator = scales[i];
                Add(matrix);
            }
        }
    }
}
