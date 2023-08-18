using OSGeo.GDAL;
using System.Collections.Generic;

namespace ozgurtek.framework.driver.gdal
{
    public class GdGdalDataSource
    {
        internal GdGdalDataSource()
        {
        }

        public static GdOgrDataSource Open(string source, bool editable = false)
        {
            GdalConfiguration.ConfigureGdal();
            int count = Gdal.GetDriverCount();
            for (int i = 0; i < count; i++)
            {
                OSGeo.GDAL.Driver driver = Gdal.GetDriver(i);
            }

            return null;
        }

        public static IEnumerable<string> DriverNames
        {
            get
            {
                GdalConfiguration.ConfigureGdal();
                int count = Gdal.GetDriverCount();
                for (int i = 0; i < count; i++)
                {
                    Driver driver = Gdal.GetDriver(i);
                    yield return driver.ShortName + " " + driver.LongName + " " + driver.GetDescription();
                }
            }
        }
    }
}
