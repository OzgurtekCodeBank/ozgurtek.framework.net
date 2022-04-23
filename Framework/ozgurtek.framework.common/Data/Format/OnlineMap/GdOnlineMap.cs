using ozgurtek.framework.common.Data.Format.OnlineMap.Bing;
using ozgurtek.framework.common.Data.Format.OnlineMap.Google;
using ozgurtek.framework.common.Data.Format.OnlineMap.OpenStreetMap;
using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data.Format.OnlineMap
{
    public abstract class GdOnlineMap : GdAbstractTileMap
    {
        private string _language = "en";

        public virtual string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public override string ConnectionString
        {
            get
            {
                return "OnlineMap";
            }
        }

        public override string Format
        {
            get { return "image/png";}
            set {}
        }

        public override string Title
        {
            get { return Name; }
        }

        public override int Srid { get; set; } = 3857;

        public override Envelope Envelope { get; set; } = new Envelope(-20026376.39, 20026376.39, -20048966.10, 20048966.10);

        public override IGdTileMatrixSet TileMatrixSet { get; set; } = new GdGoogleMapsTileMatrixSet();

        protected int GetServerNum(long x, long y, int max)
        {
            return (int)(x + 2 * y) % max;
        }

        public static GdOnlineMap Open(string name)
        {
            if (string.Equals(name, "GoogleMap", StringComparison.OrdinalIgnoreCase))
                return new GdGoogleMap();
            if (string.Equals(name, "GoogleSataliteMap", StringComparison.OrdinalIgnoreCase))
                return new GdGoogleSataliteMap();
            if (string.Equals(name, "GoogleHybridMap", StringComparison.OrdinalIgnoreCase))
                return new GdGoogleHybridMap();
            if (string.Equals(name, "GoogleTerrainMap", StringComparison.OrdinalIgnoreCase))
                return new GdGoogleTerrainMap();
            if (string.Equals(name, "OpenStreetMap", StringComparison.OrdinalIgnoreCase))
                return new GdOpenStreetMap();
            if (string.Equals(name, "BingHybrid", StringComparison.OrdinalIgnoreCase))
                return new GdBingHybridMap();
            if (string.Equals(name, "BingMap", StringComparison.OrdinalIgnoreCase))
                return new GdBingMap();
            return null;
        }

        public static IEnumerable<GdOnlineMap> GetAvailableMap()
        {
            List<GdOnlineMap> maps = new List<GdOnlineMap>(6);
            maps.Add(new GdGoogleMap());
            maps.Add(new GdGoogleSataliteMap());
            maps.Add(new GdGoogleHybridMap());
            maps.Add(new GdGoogleTerrainMap());
            maps.Add(new GdOpenStreetMap());
            maps.Add(new GdBingHybridMap());
            maps.Add(new GdBingMap());
            return maps;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}