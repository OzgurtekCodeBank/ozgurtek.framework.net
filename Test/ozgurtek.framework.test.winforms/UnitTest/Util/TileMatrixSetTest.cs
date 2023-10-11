using NetTopologySuite.Geometries;
using NUnit.Framework;
using ozgurtek.framework.common.Data.Format.OnlineMap.Google;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.test.winforms.UnitTest.Util
{
    [TestFixture]
    public class TileMatrixSetTest
    {
        [Test]
        public void LevelTest1()
        {
            GdGoogleMapsTileMatrixSet set = new GdGoogleMapsTileMatrixSet();
            GdTileIndex index = new GdTileIndex(302760, 202569, 19);
            GdTileMatrixCalculator cal = new GdTileMatrixCalculator(set);
            Polygon pol = cal.GetGeometry(index);
            Envelope polEnvelopeInternal = pol.EnvelopeInternal;
            Assert.Greater(pol.Area, 0);
        }

    }
}
