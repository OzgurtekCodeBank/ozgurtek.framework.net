using NUnit.Framework;
using ozgurtek.framework.common.Geodesy;

namespace ozgurtek.framework.test.winforms.UnitTest.Geodesy
{
    public class GdDistanceTest
    {
        [Test]
        public void KmToMTest()
        {
            GdDistance distance = new GdDistance(3000);
            Assert.AreEqual(distance.Value, 3);
        }

        [Test]
        public void MToFeet()
        {
            GdDistance distance = new GdDistance(1000, GdDistanceUnit.Ft);
            Assert.AreEqual(distance.Value, 304.8);
        }
    }
}
