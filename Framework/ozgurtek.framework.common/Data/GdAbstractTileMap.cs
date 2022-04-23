using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using ozgurtek.framework.common.Util;

namespace ozgurtek.framework.common.Data
{
    public abstract class GdAbstractTileMap : IGdTileMap
    {
        public int GetAppropriateZoomLevel(Envelope display, Envelope world)
        {
            GdTileMatrixCalculator calculator = new GdTileMatrixCalculator(TileMatrixSet);
            return calculator.GetAppropriateZoomLevel(display, world, Srid);
        }

        public long CalAreaTileCount(Envelope envelope, int zoomLevel, int padding = 0)
        {
            GdTileMatrixCalculator calculator = new GdTileMatrixCalculator(TileMatrixSet);
            return calculator.CalAreaTileCount(envelope, zoomLevel, padding);
        }

        public List<GdTileIndex> GetAreaTileList(Envelope envelope, int zoomLevel, int padding = 0, bool centerBase = true)
        {
            GdTileMatrixCalculator calculator = new GdTileMatrixCalculator(TileMatrixSet);
            return calculator.GetAreaTileList(envelope, zoomLevel, padding, centerBase);
        }

        public Polygon GetGeometry(GdTileIndex index)
        {
            GdTileMatrixCalculator calculator = new GdTileMatrixCalculator(TileMatrixSet);
            return calculator.GetGeometry(index);
        }

        public virtual IGdHttpDownloadInfo HttpDownloadInfo { get; set; } = new GdHttpDownloadInfo();
        public virtual IGdTileMatrixSet TileMatrixSet { get; set; }
        public virtual int Srid { get; set; }
        public virtual Envelope Envelope { get; set; }
        public virtual string Name { get; set; }
        public virtual string ConnectionString { get; set; }
        public virtual string Title { get; set; }
        public virtual string Format { get; set; }
        public abstract Uri GetUri(long x, long y, int zoomLevel);
    }
}