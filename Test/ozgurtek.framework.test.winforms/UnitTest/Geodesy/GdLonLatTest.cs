using NUnit.Framework;
using ozgurtek.framework.common.Geodesy;

namespace ozgurtek.framework.test.winforms.UnitTest.Geodesy
{
    public class GdLonLatTest
    {
        [Test]
        public void DistanceToTest()
        {
            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(0.119),
                Lat = new GdDegree(52.205)
            };

            GdLonLat p2 = new GdLonLat()
            {
                Lon = new GdDegree(2.351),
                Lat = new GdDegree(48.857)
            };

            GdDistance distance = p1.DistanceTo(p2);
            Assert.AreEqual(distance.Value, 404279, 0.2);
        }

        [Test]
        public void MidpointToTest()
        {
            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(0.119),
                Lat = new GdDegree(52.205)
            };

            GdLonLat p2 = new GdLonLat()
            {
                Lon = new GdDegree(2.351),
                Lat = new GdDegree(48.857)
            };

            GdLonLat midPoint = p1.MidPointTo(p2);
            Assert.AreEqual(midPoint.Lon.Value, 1.2746, 0.3); //todo burada hata var gibi
            Assert.AreEqual(midPoint.Lat.Value, 50.5363, 0.1);
        }

        [Test]
        public void IntermediatePointToTest()
        {
            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(0.119),
                Lat = new GdDegree(52.205)
            };

            GdLonLat p2 = new GdLonLat()
            {
                Lon = new GdDegree(2.351),
                Lat = new GdDegree(48.857)
            };

            GdLonLat midPoint = p1.IntermediatePointTo(p2, 0.25);
            Assert.AreEqual(midPoint.Lon.Value, 0.7073, 0.001);
            Assert.AreEqual(midPoint.Lat.Value, 51.3721, 0.1);
        }


        [Test]
        public void DestinationPointToTest()
        {
            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(-0.0015),
                Lat = new GdDegree(51.4778)
            };

            GdLonLat p2 = p1.DestinationPointTo(7794, 300.7);
            Assert.AreEqual(p2.Lon.Value, -0.0983, 0.001);
            Assert.AreEqual(p2.Lat.Value, 51.5135, 0.1);
        }


        [Test]
        public void MaxLatitudeTest()
        {
            GdLonLat lonlat = new GdLonLat()
            {
                Lon = new GdDegree(1),
                Lat = new GdDegree(1)
            };

            GdDegree deg = lonlat.MaxLatitude(157);

            Assert.AreEqual(67.003, deg.Value, 0.001);
        }

        [Test]
        public void BearingToTest()
        {
            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(0.119),
                Lat = new GdDegree(52.205)
            };

            GdLonLat p2 = new GdLonLat()
            {
                Lon = new GdDegree(2.351),
                Lat = new GdDegree(48.857)
            };

            GdBearing bearing = p1.BearingTo(p2);

            Assert.AreEqual(156.2, bearing.Value, 0.1);
        }

        [Test]
        public void FinalBearingToTest()
        {
            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(0.119),
                Lat = new GdDegree(52.205)
            };

            GdLonLat p2 = new GdLonLat()
            {
                Lon = new GdDegree(2.351),
                Lat = new GdDegree(48.857)
            };

            GdBearing bearing = p1.FinalBearingTo(p2);

            Assert.AreEqual(157.9, bearing.Value, 0.1);
        }

        [Test]
        public void CrossTrackDistanceToTest()
        {
            GdLonLat pCurrent = new GdLonLat()
            {
                Lon = new GdDegree(-0.7972),
                Lat = new GdDegree(53.2611)
            };

            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(-1.7297),
                Lat = new GdDegree(53.3206)
            };

            GdLonLat p2 = new GdLonLat()
            {
                Lon = new GdDegree(0.1334),
                Lat = new GdDegree(53.1887)
            };

            GdDistance distance = pCurrent.CrossTrackDistanceTo(p1, p2);

            Assert.AreEqual(-307.5, distance.Value, 0.1);
        }


        [Test]
        public void AlongTrackDistanceToTest()
        {
            GdLonLat pCurrent = new GdLonLat()
            {
                Lon = new GdDegree(-0.7972),
                Lat = new GdDegree(53.2611)
            };

            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(-1.7297),
                Lat = new GdDegree(53.3206)
            };

            GdLonLat p2 = new GdLonLat()
            {
                Lon = new GdDegree(0.1334),
                Lat = new GdDegree(53.1887)
            };

            GdDistance distance = pCurrent.AlongTrackDistanceTo(p1, p2);

            Assert.AreEqual(62331.49, distance.Value, 0.1);
        }

        [Test]
        public void RhumbDistanceToTest()
        {
            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(-1.7297),
                Lat = new GdDegree(53.3206)
            };

            GdLonLat p2 = new GdLonLat()
            {
                Lon = new GdDegree(0.1334),
                Lat = new GdDegree(53.1887)
            };

            GdDistance distance = p1.RhumbDistanceTo(p2);

            Assert.AreEqual(124804.45, distance.Value, 0.5);
        }

        [Test]
        public void RhumbBearingToTest()
        {
            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(1.338),
                Lat = new GdDegree(51.127)
            };

            GdLonLat p2 = new GdLonLat()
            {
                Lon = new GdDegree(1.853),
                Lat = new GdDegree(50.964)
            };

            GdBearing bearing = p1.RhumbBearingTo(p2);

            Assert.AreEqual(116.7, bearing.Value, 0.1);
        }

        [Test]
        public void RhumbDestinationPointTest()
        {
            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(1.338),
                Lat = new GdDegree(51.127)
            };

            GdLonLat p2 = p1.RhumbDestinationPoint(40300, 116.7);

            Assert.AreEqual(50.9642, p2.Lat.Value, 0.1);
            Assert.AreEqual(1.853, p2.Lon.Value, 0.1);
        }

        [Test]
        public void RhumbMidpointToTest()
        {
            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(1.338),
                Lat = new GdDegree(51.127)
            };

            GdLonLat p2 = new GdLonLat()
            {
                Lon = new GdDegree(1.853),
                Lat = new GdDegree(50.964)
            };

            GdLonLat rmid = p1.RhumbMidpointTo(p2);

            Assert.AreEqual(51.0455, rmid.Lat.Value, 0.1);
            Assert.AreEqual(1.5957, rmid.Lon.Value, 0.1);
        }

        [Test]
        public void EqualsTest()
        {
            GdLonLat p1 = new GdLonLat()
            {
                Lon = new GdDegree(1.338),
                Lat = new GdDegree(51.127)
            };

            GdLonLat p2 = new GdLonLat()
            {
                Lon = new GdDegree(1.853),
                Lat = new GdDegree(50.964)
            };

            GdLonLat p3 = new GdLonLat()
            {
                Lon = new GdDegree(1.338),
                Lat = new GdDegree(51.127)
            };


            Assert.AreEqual(false, p1.Equals(p2));
            Assert.AreEqual(true, p1.Equals(p3));
        }
    }
}
