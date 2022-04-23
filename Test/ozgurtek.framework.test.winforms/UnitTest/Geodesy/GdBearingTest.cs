using NUnit.Framework;
using ozgurtek.framework.common.Geodesy;

namespace ozgurtek.framework.test.winforms.UnitTest.Geodesy
{
    public class GdBearingTest
    {
        [Test]
        public void ConvertCompassPointStringTest()
        {
            GdBearing gdBearing = new GdBearing(24);
            string nne = gdBearing.ToCompassPointString();
            string n = gdBearing.ToCompassPointString(1);
            Assert.AreEqual("NNE", nne);
            Assert.AreEqual("N", n);
        }
    }
}
