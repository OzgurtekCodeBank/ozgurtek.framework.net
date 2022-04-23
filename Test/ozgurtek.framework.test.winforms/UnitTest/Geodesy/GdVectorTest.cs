using NUnit.Framework;
using ozgurtek.framework.common.Geodesy;

namespace ozgurtek.framework.test.winforms.UnitTest.Geodesy
{
    public class GdVectorTest
    {
        [Test]
        public void DestinationPointTest()
        {
            GdLonLat gdLonLat = new GdLonLat()
            {
                Lon = new GdDegree(28),
                Lat = new GdDegree(41)
            };

            GdRhumbLine gdRhumbLine = new GdRhumbLine(gdLonLat, 30);

            GdLonLat dp = gdRhumbLine.DestinationPointTo(12);

            Assert.AreEqual(41, dp.Lat.Value, 0.1);
            Assert.AreEqual(28, dp.Lon.Value, 0.1);
        }

        [Test]
        public void RhumbDestinationPointTest()
        {
            GdLonLat gdLonLat = new GdLonLat()
            {
                Lon = new GdDegree(28),
                Lat = new GdDegree(41)
            };

            GdRhumbLine gdRhumbLine = new GdRhumbLine(gdLonLat, 30);

            GdLonLat dp = gdRhumbLine.RhumbDestinationPoint(12);

            Assert.AreEqual(41, dp.Lat.Value, 0.1);
            Assert.AreEqual(28, dp.Lon.Value, 0.1);
        }

        [Test]
        public void InterSectionTest()
        {
            GdLonLat gdLatLon1 = new GdLonLat()
            {
                Lon = new GdDegree(0.2545),
                Lat = new GdDegree(51.8853)
            };
            double bearing1 = 108.547;
            GdRhumbLine latLonVector1 = new GdRhumbLine(gdLatLon1, bearing1);
            GdLonLat gdLatLon2 = new GdLonLat()
            {
                Lon = new GdDegree(2.5735),
                Lat = new GdDegree(49.0034)
            };
            double bearing2 = 32.435;
            GdLonLat result = latLonVector1.InterSection(gdLatLon2, bearing2);
            Assert.AreEqual(50.9078, result.Lat.Value, 0.1);
            Assert.AreEqual(4.5084, result.Lon.Value, 0.001);
        }
    }
}
