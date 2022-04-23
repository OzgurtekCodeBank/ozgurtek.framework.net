using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.common.Style;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using System;
using System.Collections.Generic;

namespace ozgurtek.framework.common.Mapping
{
    public class GdSimpleTileRenderer : GdAbstractWebMapRenderer
    {
        private readonly IGdTileLayer _layer;

        public GdSimpleTileRenderer(IGdTileLayer layer)
        {
            _layer = layer;
            Style = new GdImageStyle();
        }

        protected override IGdHttpDownloadInfo DownloadInfo
        {
            get { return _layer.TileMap.HttpDownloadInfo; }
        }

        protected override List<DownloadObject> GetDownloadObjects(IGdViewport viewport)
        {
            Envelope world = viewport.World;
            Envelope display = viewport.View;
            IGdTileMap tileMap = _layer.TileMap;

            List<DownloadObject> result = new List<DownloadObject>();
            Envelope envelope = GdProjection.Project(world, viewport.Srid, _layer.TileMap.Srid);
            int zoomlevel = tileMap.GetAppropriateZoomLevel(display, envelope);
            List<GdTileIndex> areaTileList = tileMap.GetAreaTileList(envelope, zoomlevel);
            foreach (GdTileIndex tileIndex in areaTileList)
            {
                tileIndex.Z = zoomlevel;
                Polygon polygon = tileMap.GetGeometry(tileIndex);
                if (!polygon.EnvelopeInternal.Intersects(envelope))
                    continue;

                Uri uri = _layer.TileMap.GetUri(tileIndex.X, tileIndex.Y, tileIndex.Z);
                DownloadObject obj = new DownloadObject(uri, polygon);
                obj.CacheFile = $"{GetSafeKey(tileMap.ConnectionString)}/" +
                                $"{GetSafeKey(tileMap.Name)}/" +
                                $"{GetSafeKey(tileMap.TileMatrixSet.Name)}/" +
                                $"{GetSafeKey(tileMap.Format)}/" +
                                $"{tileIndex.Z}/" +
                                $"{tileIndex.X}{tileIndex.Y}";
                result.Add(obj);
            }

            return result;
        }
    }
}