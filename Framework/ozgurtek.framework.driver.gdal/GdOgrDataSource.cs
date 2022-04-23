using System.Collections.Generic;
using OSGeo.OGR;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.driver.gdal
{
    public class GdOgrDataSource
    {
        private static bool _isInit;
        private readonly DataSource _ogrDs;
        private readonly string _connectionString;

        internal GdOgrDataSource(DataSource ds, string source)
        {
            _ogrDs = ds;
            _connectionString = source;
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public static GdOgrDataSource Open(string source)
        {
            InitGdal();

            int count = Ogr.GetDriverCount();
            for (int i = 0; i < count; i++)
            {
                Driver ogrDriver = Ogr.GetDriver(i);
                DataSource dataSource = ogrDriver.Open(source, 1);
                if (dataSource != null)
                    return new GdOgrDataSource(dataSource, source);
            }

            return null;
        }

        public int TableCount
        {
            get { return _ogrDs.GetLayerCount(); }
        }

        public GdOgrTable GetTable(string name)
        {
            Layer layer = _ogrDs.GetLayerByName(name);
            return new GdOgrTable(layer);
        }

        public IEnumerable<GdOgrTable> GetTable()
        {
            int layerCount = _ogrDs.GetLayerCount();
            for (int i = 0; i < layerCount; i++)
            {
                Layer layer = _ogrDs.GetLayerByIndex(i);
                yield return new GdOgrTable(layer);
            }
        }

        public GdOgrTable ExecuteSql(string name, IGdFilter filter)
        {
            //_ogrDs.ExecuteSQL()
            return null;
        }

        public DataSource OgrDataSource
        {
            get { return _ogrDs; }
        }

        public string Name
        {
            get { return "Gdal-Ogr Data Source"; }
        }

        private static void InitGdal()
        {
            if (_isInit)
                return;

            GdalConfiguration.ConfigureGdal();
            Ogr.RegisterAll();
            _isInit = true;
        }
    }
}