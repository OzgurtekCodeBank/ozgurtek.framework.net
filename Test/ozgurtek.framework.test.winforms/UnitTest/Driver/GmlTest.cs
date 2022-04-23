using System.IO;
using NUnit.Framework;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    [TestFixture]
    public class GmlTest
    {
        /**
         * Veritabanındaki tablo sayısını öğrenmek
         */
        [Test]
        public void ReadTest()
        {
            //please look data folder
            string text = File.ReadAllText(@"C:\Users\eniso\Desktop\Work\TestData\test.gml");
            GdMemoryTable memoryTable = GdGmlTable.LoadFromGml(text, "TKGM:geom");
            string geojson = memoryTable.ToGeojson(GdGeoJsonSeralizeType.All);
            Assert.NotNull(geojson);
        }
    }
}
