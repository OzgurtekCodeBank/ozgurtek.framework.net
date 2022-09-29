using NUnit.Framework;
using ozgurtek.framework.common.Style;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.test.winforms.UnitTest.Style
{
    [TestFixture]
    public class PointStyleTest
    {
        [Test]
        public void PointSerializeTest()
        {
            GdStyleJsonSerializer serializer = new GdStyleJsonSerializer();

            string serialize1 = serializer.Serialize(GetStyle1());
            Assert.IsNotNull(serialize1);

            string serialize2 = serializer.Serialize(GetStyle2());
            Assert.IsNotNull(serialize2);

            string serialize3 = serializer.Serialize(GetStyle3());
            Assert.IsNotNull(serialize3);

            GdStyleJsonDeSerializer deSerializer = new GdStyleJsonDeSerializer();
            GdPointStyle style1 = (GdPointStyle)deSerializer.DeSerialize(serialize1);
            Assert.IsNotNull(style1);
            Assert.IsNull(style1.Fill);
            Assert.GreaterOrEqual(style1.Size,4);
            Assert.AreEqual(style1.PointStleType,GdPointStyleType.Circle);
            Assert.IsNotNull(style1.Stroke);

            GdPointStyle style2 = (GdPointStyle)deSerializer.DeSerialize(serialize2);
            Assert.IsNotNull(style2);
            Assert.IsNull(style2.Stroke);
            Assert.IsNotNull(style2.Fill);
            Assert.GreaterOrEqual(style2.Size, 2);
            Assert.AreEqual(style2.PointStleType, GdPointStyleType.Square);
            Assert.IsNull(style2.Fill.Material);

            GdPointStyle style3 = (GdPointStyle)deSerializer.DeSerialize(serialize3);
            Assert.IsNotNull(style3);
            Assert.IsNull(style3.Stroke);
            Assert.IsNotNull(style3.Fill);
            Assert.GreaterOrEqual(style3.Size, 5);
            Assert.AreEqual(style3.PointStleType, GdPointStyleType.Square);
            Assert.IsNotNull(style3.Fill.Material);
        }

        private GdPointStyle GetStyle1()
        {
            GdPointStyle style = new GdPointStyle();
            style.Fill = null;
            style.Size = 4;
            style.Stroke = StyleColorDataRandomizer.GetStroke();
            style.PointStleType = GdPointStyleType.Circle;
            return style;
        }

        private GdPointStyle GetStyle2()
        {
            GdPointStyle style = new GdPointStyle();
            style.Fill = StyleColorDataRandomizer.GetColorFill();
            style.Size = 2;
            style.Stroke = null;
            style.PointStleType = GdPointStyleType.Square;
            return style;
        }

        private GdPointStyle GetStyle3()
        {
            GdPointStyle style = new GdPointStyle();
            style.Fill = StyleColorDataRandomizer.GetMaterialFill();
            style.Size = 5;
            style.Stroke = null;
            style.PointStleType = GdPointStyleType.Square;
            return style;
        }
    }
}
