using System.Collections.Generic;
using NUnit.Framework;
using ozgurtek.framework.common.Data.Format.Wmst;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    [TestFixture]
    public class WmtsTest
    {
        [Test]
        public void GetCapabilitiesTest()
        {
            string url = "https://giris.csb.gov.tr/geoserver/gwc/service/wmts";
            GdWmtsDataSource dataSource = new GdWmtsDataSource(url);
            dataSource.Open();
            List<GdWmtsMap> map = dataSource.GetMap();
        }
    }
}
