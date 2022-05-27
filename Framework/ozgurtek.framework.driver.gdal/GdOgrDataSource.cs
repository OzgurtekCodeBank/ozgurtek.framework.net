using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using OSGeo.OGR;
using OSGeo.OSR;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.driver.gdal
{
    public class GdOgrDataSource
    {
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

        public static GdOgrDataSource Open(string source, bool editable = false)
        {
            GdalConfiguration.ConfigureOgr();
            int count = Ogr.GetDriverCount();
            for (int i = 0; i < count; i++)
            {
                Driver ogrDriver = Ogr.GetDriver(i);
                DataSource dataSource = ogrDriver.Open(source, DbConvert.ToInt16(editable));
                if (dataSource != null)
                    return new GdOgrDataSource(dataSource, source);
            }

            return null;
        }

        public static GdOgrDataSource Open(string driverName, string source, bool editable = false)
        {
            GdalConfiguration.ConfigureOgr();
            Driver ogrDriver = Ogr.GetDriverByName(driverName);
            DataSource dataSource = ogrDriver.Open(source, DbConvert.ToInt16(editable));
            return new GdOgrDataSource(dataSource, source);
        }

        public static GdOgrDataSource Create(string driverName, string source, string[] options)
        {
            GdalConfiguration.ConfigureOgr();
            Driver ogrDriver = Ogr.GetDriverByName(driverName);
            DataSource dataSource = ogrDriver.CreateDataSource(source, options);
            return new GdOgrDataSource(dataSource, source);
        }

        public static int Delete(string driverName, string source)
        {
            GdalConfiguration.ConfigureOgr();
            Driver ogrDriver = Ogr.GetDriverByName(driverName);
            return ogrDriver.DeleteDataSource(source);
        }

        public static IEnumerable<string> DriverNames
        {
            get
            {
                GdalConfiguration.ConfigureOgr();
                int count = Ogr.GetDriverCount();
                for (int i = 0; i < count; i++)
                {
                    Driver ogrDriver = Ogr.GetDriver(i);
                    yield return ogrDriver.GetName();
                }
            }
        }

        public static bool CanCreateDataSource(string driverName)
        {
            GdalConfiguration.ConfigureOgr();
            Driver ogrDriver = Ogr.GetDriverByName(driverName);
            return ogrDriver.TestCapability(Ogr.ODrCCreateDataSource);
        }

        public static bool CanDeleteDataSource(string driverName)
        {
            GdalConfiguration.ConfigureOgr();
            Driver ogrDriver = Ogr.GetDriverByName(driverName);
            return ogrDriver.TestCapability(Ogr.ODrCDeleteDataSource);
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
            Layer layer = _ogrDs.ExecuteSQL(filter.Text, null, null);
            return new GdOgrTable(layer);
        }

        public DataSource OgrDataSource
        {
            get { return _ogrDs; }
        }

        public string Name
        {
            get { return "Osgeo-Ogr Data Source"; }
        }

        public GdOgrTable CreateTable(string name, GdGeometryType? geometryType, int? srid, string[] options, bool allowMultigeom = false)
        {
            SpatialReference spatialReference = GetSpatialReference(srid);
            wkbGeometryType wkbGeometryType = GetGeometryType(geometryType, allowMultigeom);
            Layer layer = _ogrDs.CreateLayer(name, spatialReference, wkbGeometryType, options);
            return new GdOgrTable(layer);
        }

        public void DeleteTable(string name)
        {
            //_ogrDs.
        }

        public void Dispose()
        {
            _ogrDs.Dispose();
        }

        private SpatialReference GetSpatialReference(int? srid)
        {
            if (!srid.HasValue)
                return null;

            ICoordinateSystem coordinateSystem = GdProjection.GetCrs(srid.Value);
            if (coordinateSystem == null)
                return null;

            SpatialReference reference = new SpatialReference(coordinateSystem.WKT);
            return reference;
        }

        private wkbGeometryType  GetGeometryType(GdGeometryType? type, bool allowMultigeom)
        {
            if (!type.HasValue)
                return wkbGeometryType.wkbNone;

            if (!allowMultigeom)
            {
                switch (type)
                {
                    case GdGeometryType.Line:
                        return wkbGeometryType.wkbLineString;
                    case GdGeometryType.Polygon:
                        return wkbGeometryType.wkbPolygon;
                    case GdGeometryType.Point:
                        return wkbGeometryType.wkbMultiPoint;
                    default:
                        return wkbGeometryType.wkbUnknown;
                }
            }

            switch (type)
            {
                case GdGeometryType.Line:
                    return wkbGeometryType.wkbMultiLineString;
                case GdGeometryType.Polygon:
                    return wkbGeometryType.wkbMultiPolygon;
                case GdGeometryType.Point:
                    return wkbGeometryType.wkbMultiPoint;
                default:
                    return wkbGeometryType.wkbUnknown;
            }
        }
    }
}