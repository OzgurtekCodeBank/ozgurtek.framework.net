using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using ozgurtek.framework.common.Data.Format.ArcGis;
using ozgurtek.framework.common.Data.Format.Wmts;
using Exception = System.Exception;

namespace ozgurtek.framework.common.Data.Format.Wmst
{
    public class GdWmtsDataSource
    {
        private readonly string _address;
        private GdArcGisToken _arcgistoken;
        private List<GdWmtsMap> _cache;
        private string _cacheFolder;

        public GdWmtsDataSource(string address)
        {
            _address = address.Trim();
        }

        public void Open()
        {
            FillCache();
        }

        public string Address
        {
            get { return _address; }
        }

        public string CacheFolderForCapabilities
        {
            get => _cacheFolder;
            set => _cacheFolder = value;
        }

        public List<GdWmtsMap> GetMap()
        {
            if (_cache == null)
                throw new Exception("open the datasource first");

            return _cache;
        }

        public List<GdWmtsMap> GetMap(string name)
        {
            List<GdWmtsMap> result = new List<GdWmtsMap>();
            IEnumerable<GdWmtsMap> maps = GetMap();
            foreach (GdWmtsMap map in maps)
            {
                if (map.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    result.Add(map);
            }
            return result;
        }

        public GdArcGisToken Arcgistoken
        {
            get => _arcgistoken;
            set => _arcgistoken = value;
        }

        public Capabilities GetCapabilities()
        {
            string urlString = $"{Address}{"?service=wmts&request=GetCapabilities"}";
            if (_arcgistoken != null)
                urlString += $"&token={_arcgistoken.Token}";

            XmlReader reader = XmlReader.Create(urlString);
            XmlSerializer serializer = new XmlSerializer(typeof(Capabilities));
            Capabilities capabilities = (Capabilities) serializer.Deserialize(reader);

            return capabilities;
        }

        private void FillCache()
        {
            if (_cache != null)
                return;

            _cache = new List<GdWmtsMap>();
            Capabilities capabilities = GetCapabilities();
            LayerType[] layers = capabilities.Contents.Layers;
            if (layers == null)
                return;

            Dictionary<string, GdTileMatrixSet> tileMatrixSet = GetTileMatrixSet(capabilities);
            foreach (LayerType layer in layers)//each layer
            {
                TileMatrixSetLink[] matrixSetLink = layer.TileMatrixSetLink;
                if (matrixSetLink == null)
                    continue;

                foreach (TileMatrixSetLink link in matrixSetLink)//each matrix set
                {
                    if (!tileMatrixSet.TryGetValue(link.TileMatrixSet, out GdTileMatrixSet matrixSet))
                        continue;

                    foreach (string format in layer.Format)//each format
                    {
                        foreach (Wmts.Style style in layer.Style)//each style
                        {
                            GdWmtsMap map = new GdWmtsMap(_address);
                            map.Name = GdWmtsUtil.GetString(layer.Identifier);
                            map.Title = GdWmtsUtil.GetString(layer.Title);
                            map.Style = GdWmtsUtil.GetString(style.Identifier);
                            map.Envelope = GdWmtsUtil.GetEnvelope(layer.Items, matrixSet.Srid);
                            map.TileMatrixSet = matrixSet;
                            map.Arcgistoken = _arcgistoken;
                            map.Srid = matrixSet.Srid;
                            map.Format = format;
                            map.Capabilities = layer;
                            _cache.Add(map);
                        }
                    }
                }
            }
        }

        private Dictionary<string, GdTileMatrixSet> GetTileMatrixSet(Capabilities capabilities)
        {
            Dictionary<string, GdTileMatrixSet> result = new Dictionary<string, GdTileMatrixSet>();

            TileMatrixSet[] sets = capabilities.Contents.TileMatrixSet;
            foreach (TileMatrixSet ogcSet in sets)
            {
                GdTileMatrixSet matrixSet = new GdTileMatrixSet();
                matrixSet.Name = GdWmtsUtil.GetString(ogcSet.Identifier);
                matrixSet.WellKnownScaleSet = ogcSet.WellKnownScaleSet;
                matrixSet.Envelope = GdWmtsUtil.GetEnvelope(ogcSet.Item);
                int? srid = GdWmtsUtil.GetSrid(ogcSet.SupportedCRS);
                if (srid.HasValue)
                    matrixSet.Srid = srid.Value;

                TileMatrix[] tileMatrices = ogcSet.TileMatrix;
                if (tileMatrices == null)
                    continue;

                foreach (TileMatrix ogcMatrix in tileMatrices)
                {
                    GdTileMatrix matrix = new GdTileMatrix();
                    matrix.Name = GdWmtsUtil.GetString(ogcMatrix.Identifier);
                    matrix.MatrixWidth = ogcMatrix.MatrixWidth;
                    matrix.MatrixHeight = ogcMatrix.MatrixHeight;
                    matrix.ScaleDenominator = ogcMatrix.ScaleDenominator;
                    matrix.TileHeight = ogcMatrix.TileHeight;
                    matrix.TileWidth = ogcMatrix.TileWidth;
                    matrix.TopLeftCorner = GdWmtsUtil.GetCoordinate(ogcMatrix.TopLeftCorner);
                    matrixSet.Add(matrix);
                }

                result.Add(matrixSet.Name, matrixSet);
            }

            return result;
        }
    }
}