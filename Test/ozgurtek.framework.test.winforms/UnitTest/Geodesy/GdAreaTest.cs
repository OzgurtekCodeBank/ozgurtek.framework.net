using NUnit.Framework;
using ozgurtek.framework.common.Geodesy;

namespace ozgurtek.framework.test.winforms.UnitTest.Geodesy
{
    public class GdAreaTest
    {
        [Test]
        public void M2ToKm2Test()
        {
            GdArea area = new GdArea(3000000);
            //double value = area.Convert(GdAreaUnit.Km2, 3);
            //Assert.AreEqual(value, 3);
        }
    }
}
