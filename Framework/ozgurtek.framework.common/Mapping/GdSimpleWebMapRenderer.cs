using ozgurtek.framework.common.Style;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Geodesy;

namespace ozgurtek.framework.common.Mapping
{
    public class GdSimpleWebMapRenderer : GdAbstractWebMapRenderer
    {
        private readonly IGdWmsLayer _layer;
        private int _tileSizeX = 256;
        private int _tileSizeY = 256;

        public GdSimpleWebMapRenderer(IGdWmsLayer layer)
        {
            Style = new GdImageStyle();
            _layer = layer;
        }

        public int TileSizeX
        {
            get => _tileSizeX;
            set => _tileSizeX = value;
        }

        public int TileSizeY
        {
            get => _tileSizeY;
            set => _tileSizeY = value;
        }

        protected override IGdHttpDownloadInfo DownloadInfo
        {
            get { return _layer.WmsMap.HttpDownloadInfo; }
        }

        protected override List<DownloadObject> GetDownloadObjects(IGdViewport viewport)
        {
            IGdWmsMap wmsMap = _layer.WmsMap;
            if (viewport.Srid <= 0 || wmsMap.Srid <= 0)
                throw new Exception("raster or map srid wrong");

            List<DownloadObject> result = new List<DownloadObject>();

            //transform envelope
            Envelope envelope = GdProjection.Project(viewport.World, viewport.Srid, wmsMap.Srid);
            if (!envelope.Intersects(wmsMap.Envelope)) //todo: herzaman olmayabilir....null geliyor bazen..
                return result;

            int w = _tileSizeX;
            int h = _tileSizeX;
            List<GdTileIndex> tileIndices;
            if (_tileSizeX > 0 && _tileSizeY > 0)
            {
                GdViewportDivider divider = new GdViewportDivider();
                tileIndices = divider.Divide(viewport, w, h);
            }
            else //single tile
            {
                tileIndices = new List<GdTileIndex>(1);
                GdTileIndex index = new GdTileIndex(0, 0);
                index.Envelope = viewport.World;
                tileIndices.Add(index);
                w = Convert.ToInt32(viewport.View.Width);
                h = Convert.ToInt32(viewport.View.Height);
            }

            GeometryFactory factory = new GeometryFactory();
            foreach (GdTileIndex tileIndex in tileIndices)
            {
                Envelope env = GdProjection.Project(tileIndex.Envelope, viewport.Srid, wmsMap.Srid);
                Polygon polygon = (Polygon) factory.ToGeometry(env);
                polygon.SRID = wmsMap.Srid;

                Uri uri = wmsMap.GetUri(env, w, h);
                DownloadObject obj = new DownloadObject(uri, polygon);
                result.Add(obj);
            }

            return result;
        }
    }
}