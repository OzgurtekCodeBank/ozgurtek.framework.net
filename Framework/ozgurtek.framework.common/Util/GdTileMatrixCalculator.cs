using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.CoordinateSystems;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Util
{
    public class GdTileMatrixCalculator
    {
        private readonly IGdTileMatrixSet _matrixSet;

        public GdTileMatrixCalculator(IGdTileMatrixSet matrixSet)
        {
            _matrixSet = matrixSet;
        }
        public int GetAppropriateZoomLevel(Envelope display, Envelope world, int srid)
        {
            int zoom = _matrixSet.Count - 1;
            int zoomMax = _matrixSet.Count;

            for (int i = 0; i < zoomMax; i++)
            {
                double neededTileAmount = display.Height * display.Width / (_matrixSet[i].TileHeight * _matrixSet[i].TileWidth);
                Envelope newWorld = SnapToGrid(world, i, out _);

                double tileCount = CalAreaTileCount(newWorld, i);
                if (tileCount < neededTileAmount)
                    continue;

                zoom = i;
                break;
            }
            return zoom;
        }

        private Envelope SnapToGrid(Envelope world, int zoom, out GdTileIndex index)
        {
            double metersPerUnit = 1;
            ICoordinateSystem coordinateSystem = GdProjection.GetCrs(_matrixSet.Srid);
            IUnit units = coordinateSystem.GetUnits(2);
            if (units is ILinearUnit linearUnit)
                metersPerUnit = linearUnit.MetersPerUnit;
            else if (units is IAngularUnit)
                metersPerUnit = 2 * Math.PI * 6378137 / 360; //111319.49079327358

            IGdTileMatrix matrix = _matrixSet[zoom];
            double pixelSpan = matrix.ScaleDenominator * 0.28 * 0.001 / metersPerUnit;
            double tileSpanX = matrix.TileWidth * pixelSpan;
            double tileSpanY = matrix.TileHeight * pixelSpan;
            double tileMatrixMinX = matrix.TopLeftCorner.Y;
            double tileMatrixMaxY = matrix.TopLeftCorner.X;

            double epsilon = 1e-6;
            double tileMinCol = Math.Floor((world.MinX - tileMatrixMinX) / tileSpanX + epsilon);
            double tileMinRow = Math.Floor((tileMatrixMaxY - world.MaxY) / tileSpanY + epsilon);
            
            index = new GdTileIndex((long)tileMinCol, (long)tileMinRow, zoom);

            //upper left corner
            double leftX = index.X * tileSpanX + tileMatrixMinX;
            double upperY = tileMatrixMaxY - index.Y * tileSpanY;

            Envelope newWorld = new Envelope(world);
            newWorld.Translate(leftX - world.MinX, upperY - world.MaxY);
            
            return newWorld;
        }

        public long CalAreaTileCount(Envelope envelope, int zoomLevel, int padding = 0)
        {
            double metersPerUnit = 1;
            ICoordinateSystem coordinateSystem = GdProjection.GetCrs(_matrixSet.Srid);
            IUnit units = coordinateSystem.GetUnits(2);
            if (units is ILinearUnit linearUnit)
                metersPerUnit = linearUnit.MetersPerUnit;
            else if (units is IAngularUnit)
                metersPerUnit = 2 * Math.PI * 6378137 / 360; //111319.49079327358

            IGdTileMatrix matrix = _matrixSet[zoomLevel];
            double pixelSpan = matrix.ScaleDenominator * 0.28 * 0.001 / metersPerUnit;
            double tileSpanX = matrix.TileWidth * pixelSpan;
            double tileSpanY = matrix.TileHeight * pixelSpan;
            double tileMatrixMinX = matrix.TopLeftCorner.Y;
            double tileMatrixMaxY = matrix.TopLeftCorner.X;

            double epsilon = 1e-6;
            double tileMinCol = Math.Floor((envelope.MinX - tileMatrixMinX) / tileSpanX + epsilon);
            double tileMaxCol = Math.Floor((envelope.MaxX - tileMatrixMinX) / tileSpanX - epsilon);
            double tileMinRow = Math.Floor((tileMatrixMaxY - envelope.MaxY) / tileSpanY + epsilon);
            double tileMaxRow = Math.Floor((tileMatrixMaxY - envelope.MinY) / tileSpanY - epsilon);

            //to avoid requesting out-of - range tiles
            if (tileMinCol < 0)
                tileMinCol = 0;
            if (tileMaxCol >= matrix.MatrixWidth) tileMaxCol = matrix.MatrixWidth - 1;
            if (tileMinRow < 0) tileMinRow = 0;
            if (tileMaxRow >= matrix.MatrixHeight) tileMaxRow = matrix.MatrixHeight - 1;

            return (long)((tileMaxCol - tileMinCol + 1) * (tileMaxRow - tileMinRow + 1));
        }

        public IEnumerable<GdTileIndex> GetAreaTileList(Envelope envelope, int zoomLevel, int padding = 0, bool centerBase = true)
        {
            double metersPerUnit = 1;
            ICoordinateSystem coordinateSystem = GdProjection.GetCrs(_matrixSet.Srid);
            IUnit units = coordinateSystem.GetUnits(2);
            if (units is ILinearUnit linearUnit)
                metersPerUnit = linearUnit.MetersPerUnit;
            else if (units is IAngularUnit)
                metersPerUnit = 2 * Math.PI * 6378137 / 360; //111319.49079327358

            IGdTileMatrix matrix = _matrixSet[zoomLevel];
            double pixelSpan = matrix.ScaleDenominator * 0.28 * 0.001 / metersPerUnit;
            double tileSpanX = matrix.TileWidth * pixelSpan;
            double tileSpanY = matrix.TileHeight * pixelSpan;
            double tileMatrixMinX = matrix.TopLeftCorner.Y;
            double tileMatrixMaxY = matrix.TopLeftCorner.X;

            double epsilon = 1e-6;
            long tileMinCol = (long)Math.Floor((envelope.MinX - tileMatrixMinX) / tileSpanX + epsilon);
            long tileMaxCol = (long)Math.Floor((envelope.MaxX - tileMatrixMinX) / tileSpanX - epsilon);
            long tileMinRow = (long)Math.Floor((tileMatrixMaxY - envelope.MaxY) / tileSpanY + epsilon);
            long tileMaxRow = (long)Math.Floor((tileMatrixMaxY - envelope.MinY) / tileSpanY - epsilon);

            //to avoid requesting out-of - range tiles
            if (tileMinCol < 0)
                tileMinCol = 0;
            if (tileMaxCol >= matrix.MatrixWidth) tileMaxCol = matrix.MatrixWidth - 1;
            if (tileMinRow < 0) tileMinRow = 0;
            if (tileMaxRow >= matrix.MatrixHeight) tileMaxRow = matrix.MatrixHeight - 1;

            //List<GdTileIndex> ret = new List<GdTileIndex>();
            for (long col = tileMinCol; col <= tileMaxCol; col++)
            {
                for (long row = tileMinRow; row <= tileMaxRow; row++)
                {
                    GdTileIndex p = new GdTileIndex(Math.Abs(col), Math.Abs(row));
                    p.Z = zoomLevel;
                    //if (!ret.Contains(p))
                    //{
                    //    ret.Add(p);
                    //}
                    yield return p;
                }
            }

            //if (centerBase)
            //    ret = SortByDistance(ret);

            //return ret;
        }

        public Polygon GetGeometry(GdTileIndex index)
        {
            double metersPerUnit = 1;
            ICoordinateSystem coordinateSystem = GdProjection.GetCrs(_matrixSet.Srid);
            IUnit units = coordinateSystem.GetUnits(2);
            if (units is ILinearUnit linearUnit)
                metersPerUnit = linearUnit.MetersPerUnit;
            else if (units is IAngularUnit)
                metersPerUnit = 2 * Math.PI * 6378137 / 360; //111319.49079327358

            IGdTileMatrix matrix = _matrixSet[index.Z];
            double pixelSpan = matrix.ScaleDenominator * 0.28 * 0.001 / metersPerUnit;
            double tileSpanX = matrix.TileWidth * pixelSpan;
            double tileSpanY = matrix.TileHeight * pixelSpan;
            double tileMatrixMinX = matrix.TopLeftCorner.Y;
            double tileMatrixMaxY = matrix.TopLeftCorner.X;

            //upper left corner
            double leftX = index.X * tileSpanX + tileMatrixMinX;
            double upperY = tileMatrixMaxY - index.Y * tileSpanY;

            //lower-right corner
            double rightX = (index.X + 1) * tileSpanX + tileMatrixMinX;
            double lowerY = tileMatrixMaxY - (index.Y + 1) * tileSpanY;

            Envelope envelope = new Envelope(leftX, rightX, lowerY, upperY);
            GeometryFactory geometryFactory = GdFactoryFinder.Instance.GeometryServices.CreateGeometryFactory();
            Polygon geometry = (Polygon)geometryFactory.ToGeometry(envelope);
            geometry.SRID = _matrixSet.Srid;

            return geometry;
        }

        public List<GdTileIndex> SortByDistance(List<GdTileIndex> lst)
        {
            if (lst.Count == 0)
                return lst;

            List<GdTileIndex> output = new List<GdTileIndex>();
            int meanX = (int)((lst.Max(m => m.X) + lst.Min(m => m.X)) / 2);
            int meanY = (int)((lst.Max(m => m.Y) + lst.Min(m => m.Y)) / 2);

            output.Add(lst[NearestPoint(meanX, meanY, lst)]);
            lst.Remove(output[0]);
            int q = 0;
            for (int i = 0; i < lst.Count + q; i++)
            {
                output.Add(lst[NearestPoint(meanX, meanY, lst)]);
                lst.Remove(output[output.Count - 1]);
                q++;
            }

            return output;
        }

        public int NearestPoint(long x, long y, List<GdTileIndex> lookIn)
        {
            KeyValuePair<double, int> smallestDistance = new KeyValuePair<double, int>();
            for (int i = 0; i < lookIn.Count; i++)
            {
                double distance = Math.Sqrt(Math.Pow(x - lookIn[i].X, 2) + Math.Pow(y - lookIn[i].Y, 2));
                if (i == 0)
                {
                    smallestDistance = new KeyValuePair<double, int>(distance, i);
                }
                else
                {
                    if (distance < smallestDistance.Key)
                    {
                        smallestDistance = new KeyValuePair<double, int>(distance, i);
                    }
                }
            }

            return smallestDistance.Value;
        }
    }
}