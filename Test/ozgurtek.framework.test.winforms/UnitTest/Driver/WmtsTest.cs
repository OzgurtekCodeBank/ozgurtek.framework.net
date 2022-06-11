using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
