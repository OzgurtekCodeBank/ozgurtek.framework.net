using NetTopologySuite.Geometries;
using System.Globalization;

namespace ozgurtek.framework.core.Data
{
    public class GdTileIndex
    {
        private Envelope _envelope;

        public GdTileIndex(long x, long y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        public GdTileIndex(long x, long y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public long X { get; set; }
        public long Y { get; set; }
        public int Z { get; set; }

        public override string ToString()
        {
            return $"X: {X.ToString(CultureInfo.InvariantCulture)}, Y: {Y.ToString(CultureInfo.InvariantCulture)}";
        }

        public Envelope Envelope
        {
            get { return _envelope; }
            set { _envelope = value; }
        }
    }
}