using NUnit.Framework;
using ozgurtek.framework.common.Style;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.test.winforms.UnitTest.Style
{
    [TestFixture]
    public class PolygonStyleTest
    {
        [Test]
        public void PolygonSerializeTest()
        {
            GdStyleJsonSerializer serializer = new GdStyleJsonSerializer();

            string serialize1 = serializer.Serialize(GetStyle1());
            Assert.IsNotNull(serialize1);

            string serialize2 = serializer.Serialize(GetStyle2());
            Assert.IsNotNull(serialize2);

            string serialize3 = serializer.Serialize(GetStyle3());
            Assert.IsNotNull(serialize3);

            GdStyleJsonDeSerializer deSerializer = new GdStyleJsonDeSerializer();
            GdPolygonStyle style1 = (GdPolygonStyle) deSerializer.DeSerialize(serialize1);
            Assert.IsNotNull(style1);
            Assert.IsNull(style1.Fill);
            Assert.IsNotNull(style1.Stroke);

            GdPolygonStyle style2 = (GdPolygonStyle) deSerializer.DeSerialize(serialize2);
            Assert.IsNotNull(style2);
            Assert.IsNull(style2.Stroke);
            Assert.IsNotNull(style2.Fill);
            Assert.IsNull(style2.Fill.Material);

            GdPolygonStyle style3 = (GdPolygonStyle) deSerializer.DeSerialize(serialize3);
            Assert.IsNotNull(style3);
            Assert.IsNull(style3.Stroke);
            Assert.IsNotNull(style3.Fill);
            Assert.IsNotNull(style3.Fill.Material);
        }

        private IGdPolygonStyle GetStyle1()
        {
            GdPolygonStyle polygonStyle = new GdPolygonStyle();
            polygonStyle.Fill = null;
            polygonStyle.Stroke = StyleColorDataRandomizer.GetStroke();
            return polygonStyle;
        }

        private IGdPolygonStyle GetStyle2()
        {
            GdPolygonStyle polygonStyle = new GdPolygonStyle();
            polygonStyle.Fill = StyleColorDataRandomizer.GetColorFill();
            polygonStyle.Stroke = null;
            return polygonStyle;
        }

        private IGdPolygonStyle GetStyle3()
        {
            GdPolygonStyle polygonStyle = new GdPolygonStyle();
            polygonStyle.Fill = StyleColorDataRandomizer.GetMaterialFill();
            polygonStyle.Stroke = null;
            return polygonStyle;
        }
    }
}