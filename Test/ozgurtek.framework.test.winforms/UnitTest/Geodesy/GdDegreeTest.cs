using NUnit.Framework;
using ozgurtek.framework.common.Geodesy;

namespace ozgurtek.framework.test.winforms.UnitTest.Geodesy
{
    public class GdDegreeTest
    {
        [Test]
        public void ValueToDegMinSecTest()
        {
            GdDegree gdDegrees = new GdDegree(67.56565655)
            {
                Precision = 2
            };
            gdDegrees.Format = GdDegreeFormat.DegMinSec;
            string degreeString = gdDegrees.ToString();
            Assert.AreEqual(degreeString, "67° 33' 56.36'' N");
        }

        [Test]
        public void DegMinSecToStringTest()
        {
            GdDegree gdDegrees = new GdDegree(67, 33, 56.3636, true)
            {
                Precision = 2
            };
            gdDegrees.Format = GdDegreeFormat.DegMinSec;
            string degreeString = gdDegrees.ToString();
            Assert.AreEqual(degreeString, "67° 33' 56.36'' N");
        }

        [Test]
        public void DegMinSecToValueTest()
        {
            GdDegree gdDegrees = new GdDegree(67, 33, 56.3636, true)
            {
                Precision = 2
            };
            Assert.AreEqual(gdDegrees.Value, 67.57);
        }

        [Test]
        public void ValueToDegMinSecPrecisionTest()
        {
            GdDegree gdDegrees = new GdDegree(67.56565655);
            gdDegrees.Format = GdDegreeFormat.DegMinSec;
            gdDegrees.Precision = 4;
            string degreeString = gdDegrees.ToString();
            Assert.AreEqual(degreeString, "67° 33' 56.3636'' N");
        }

        [Test]
        public void ValueToDegMinTest()
        {
            GdDegree gdDegrees = new GdDegree(67.56565655);
            gdDegrees.Format = GdDegreeFormat.DegMin;
            string degreeString = gdDegrees.ToString();
            Assert.AreEqual(degreeString, "67° 33' N");
        }


        [Test]
        public void ValueToDegTest()
        {
            GdDegree gdDegrees = new GdDegree(67.56565655);
            gdDegrees.Format = GdDegreeFormat.Deg;
            string degreeString = gdDegrees.ToString();
            Assert.AreEqual(degreeString, "67° N");
        }
    }
}