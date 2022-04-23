using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace ozgurtek.framework.core.Data
{
    public interface IGdTileMap
    {
        /// <summary>
        /// Gets adress of this layer
        /// </summary>
        string ConnectionString { get; }
        
        /// <summary>
        /// Gets name of this layer
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// human readable format of this layer
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="display"></param>
        /// <param name="world"></param>
        /// <returns></returns>
        int GetAppropriateZoomLevel(Envelope display, Envelope world);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="zoomLevel"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        long CalAreaTileCount(Envelope envelope, int zoomLevel, int padding = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="zoomLevel"></param>
        /// <param name="padding"></param>
        /// <param name="centerBase"></param>
        /// <returns></returns>
        List<GdTileIndex> GetAreaTileList(Envelope envelope, int zoomLevel, int padding = 0, bool centerBase = true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        Uri GetUri(long x, long y, int zoomLevel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Polygon GetGeometry(GdTileIndex index);

        /// <summary>
        /// 
        /// </summary>
        int Srid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Envelope Envelope { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IGdHttpDownloadInfo HttpDownloadInfo { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IGdTileMatrixSet TileMatrixSet { get; set; }

        /// <summary>
        /// tile format
        /// </summary>
        string Format { get; set; }
    }
}