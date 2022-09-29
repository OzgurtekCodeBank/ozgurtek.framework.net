using NUnit.Framework;
using ozgurtek.framework.common.Style;

namespace ozgurtek.framework.test.winforms.UnitTest.Style
{
    [TestFixture]
    public class LineStyleTest
    {
        [Test]
        public void LineSerializeTest()
        {
            GdStyleJsonSerializer serializer = new GdStyleJsonSerializer();

            string serialize1 = serializer.Serialize(GetStyle1());
            Assert.IsNotNull(serialize1);

            string serialize2 = serializer.Serialize(GetStyle2());
            Assert.IsNotNull(serialize2); 

            GdStyleJsonDeSerializer deSerializer = new GdStyleJsonDeSerializer();
            GdLineStyle style1 = (GdLineStyle)deSerializer.DeSerialize(serialize1);
            Assert.IsNotNull(style1);
            Assert.IsNotNull(style1.Stroke);

            GdLineStyle style2 = (GdLineStyle)deSerializer.DeSerialize(serialize2);
            Assert.IsNotNull(style2);
            Assert.IsNull(style2.Stroke);
        }

        private GdLineStyle GetStyle1()
        {
            GdLineStyle lineStyle = new GdLineStyle();
            lineStyle.Stroke = StyleColorDataRandomizer.GetStroke();
            return lineStyle;
        }

        private GdLineStyle GetStyle2()
        {
            GdLineStyle lineStyle = new GdLineStyle();
            lineStyle.Stroke = null;
            return lineStyle;
        } 
    }
}
