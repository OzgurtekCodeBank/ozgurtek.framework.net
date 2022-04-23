using NetTopologySuite;
using ozgurtek.framework.common.Data;

namespace ozgurtek.framework.common
{
    public class GdFactoryFinder
    {
        private static GdFactoryFinder obj;

        private GdFactoryFinder()
        {
        }

        public static GdFactoryFinder Instance
        {
            get
            {
                if (obj == null)
                    obj = new GdFactoryFinder();
                return obj;
            }
        }

        public NtsGeometryServices GeometryServices
        {
            get
            {
                return new NtsGeometryServices();
            }
        }
    }
}
