using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.sqlite;

namespace ozgurtek.framework.converter.winforms
{
    public class GdExtrudedModelExportEngine
    {
        private string _outputFolder;
        private long _xyTileCount = -1;
        private int _epsgCode = -1;
        private bool _suppressBlankTile = true;
        private double _groundHeight;
        private string _fidFieldName;
        private string _geomFieldName;
        private string _extFieldName;
        private string _styleFieldName;
        private string _descFieldName;
        private double _tileSizeInMeter = -1;

        public string OutputFolder
        {
            get => _outputFolder;
            set => _outputFolder = value;
        }

        public long XyTileCount
        {
            get => _xyTileCount;
            set => _xyTileCount = value;
        }

        public double TileSizeInMeter
        {
            get => _tileSizeInMeter;
            set => _tileSizeInMeter = value;
        }

        public int EpsgCode
        {
            get => _epsgCode;
            set => _epsgCode = value;
        }

        public bool SuppressBlankTile
        {
            get => _suppressBlankTile;
            set => _suppressBlankTile = value;
        }

        public double GroundHeight
        {
            get => _groundHeight;
            set => _groundHeight = value;
        }

        public string FidFieldName
        {
            get => _fidFieldName;
            set => _fidFieldName = value;
        }

        public string GeomFieldName
        {
            get => _geomFieldName;
            set => _geomFieldName = value;
        }

        public string ExtFieldName
        {
            get => _extFieldName;
            set => _extFieldName = value;
        }

        public string StyleFieldName
        {
            get => _styleFieldName;
            set => _styleFieldName = value;
        }

        public string DescFieldName
        {
            get => _descFieldName;
            set => _descFieldName = value;
        }

        public void Export(IGdTable table, IGdTrack track)
        {
            //folder
            if (string.IsNullOrWhiteSpace(OutputFolder))
                throw new Exception("Folder name blank...");

            if (!Directory.Exists(OutputFolder))
                throw new Exception("Folder not exists");

            if (Directory.EnumerateFileSystemEntries(OutputFolder).Any())
                throw new Exception("Folder must be empty");

            //tile count
            if (XyTileCount <= 0 && TileSizeInMeter <= 0)
                throw new Exception("XyTileCount or TileSize Wrong");

            //epsg code
            if (EpsgCode <= 0)
                throw new Exception("EpsgCode Wrong");

            //fid field
            if (string.IsNullOrWhiteSpace(FidFieldName))
                throw new Exception("FidFieldName not exists");

            //geometry field
            if (string.IsNullOrWhiteSpace(GeomFieldName))
                throw new Exception("GeomFieldName not exists");

            //divide 4326....
            long xyTileCount = XyTileCount;
            if (TileSizeInMeter > 0)
            {
                Envelope envelope = GdProjection.Project(table.Envelope, EpsgCode, 3857);
                xyTileCount = (long) Math.Floor(envelope.Area / (TileSizeInMeter * TileSizeInMeter));
            }

            table.GeometryField = GeomFieldName;
            Envelope project = GdProjection.Project(table.Envelope, EpsgCode, 4326);
            IEnumerable<GdTileIndex> wgsTileIndex = DivideByCount(project, xyTileCount);

            //create json models...
            CreateModels(table, wgsTileIndex, xyTileCount, track);

            //finish
            if (track != null)
                track.ReportProgress(100);
        }

        private void CreateModels(IGdTable table, IEnumerable<GdTileIndex> tileIndex, long tileCount, IGdTrack track)
        {
            //crete sqllite index file 
            string path = Path.Combine(OutputFolder, "index.sqlite");
            GdSqlLiteDataSource sqlLiteDataSource = GdSqlLiteDataSource.OpenOrCreate(path);
            GdSqlLiteTable sqlLiteTable = sqlLiteDataSource.CreateTable("gd_index", GdGeometryType.Polygon, 4326, null);
            sqlLiteTable.CreateField(new GdField("min_x", GdDataType.Real));
            sqlLiteTable.CreateField(new GdField("min_y", GdDataType.Real));
            sqlLiteTable.CreateField(new GdField("max_x", GdDataType.Real));
            sqlLiteTable.CreateField(new GdField("max_y", GdDataType.Real));
            sqlLiteTable.BeginTransaction();

            long fileName = 0;
            foreach (GdTileIndex index in tileIndex)
            {
                if (index.Envelope.Area <= 0)
                    continue;

                //search given table
                Envelope envelope = GdProjection.Project(index.Envelope, 4326, EpsgCode);
                table.GeometryFilter = new GdGeometryFilter(envelope);

                if (SuppressBlankTile && table.RowCount == 0)
                    continue;

                //write index to sqlite file
                GdRowBuffer sqlLiteBuffer = new GdRowBuffer();
                GeometryFactory factory = new GeometryFactory();
                Geometry geometryWgs84 = factory.ToGeometry(index.Envelope);
                geometryWgs84.SRID = 4326;
                sqlLiteBuffer.Put("min_x", index.Envelope.MinX);
                sqlLiteBuffer.Put("min_y", index.Envelope.MinY);
                sqlLiteBuffer.Put("max_x", index.Envelope.MaxX);
                sqlLiteBuffer.Put("max_y", index.Envelope.MaxY);
                sqlLiteBuffer.Put("geometry", geometryWgs84);
                sqlLiteTable.Insert(sqlLiteBuffer);

                //write json file
                GdMemoryTable memTable = Util.PrepareNewMemTable();
                foreach (IGdRow row in table.Rows)
                {
                    if (row.IsNull(GeomFieldName))
                        continue;

                    if (row.IsNull(FidFieldName))
                        continue;

                    GdRowBuffer buffer = new GdRowBuffer();

                    //id
                    buffer.Put(Util.GdId, row.GetAsString(FidFieldName));

                    //geometry
                    Geometry geometry = row.GetAsGeometry(GeomFieldName);
                    geometry = geometry.Copy();
                    geometry.Apply(new Mtf(_groundHeight));
                    geometry.SRID = EpsgCode;
                    geometry = GdProjection.Project(geometry, 4326);
                    buffer.Put(Util.GdGeometry, geometry);

                    //optional height
                    if (!string.IsNullOrEmpty(ExtFieldName) && !row.IsNull(ExtFieldName))
                        buffer.Put(Util.GdHeight, row.GetAsReal(ExtFieldName));

                    //optional style
                    if (!string.IsNullOrEmpty(StyleFieldName) && !row.IsNull(StyleFieldName))
                        buffer.Put(Util.GdStyle, row.GetAsString(StyleFieldName));

                    //optional description
                    if (!string.IsNullOrEmpty(DescFieldName) && !row.IsNull(DescFieldName))
                        buffer.Put(Util.GdDescription, row.GetAsString(DescFieldName));

                    memTable.Insert(buffer);
                }

                //write json file....
                string fullFileName = Path.Combine(OutputFolder, DbConvert.ToString(++fileName) + ".json");
                string geojson = memTable.ToGeojson(GdGeoJsonSeralizeType.OnlyData, 3);
                File.WriteAllText(fullFileName, geojson);

                if (track != null)
                    track.ReportProgress(DbConvert.ToDouble(fileName * 100 / tileCount));
            }

            sqlLiteTable.CommitTransaction();
        }

        public IEnumerable<GdTileIndex> DivideByCount(Envelope viewport, long xyTileCount)
        {
            double xStep = viewport.Width / xyTileCount;
            double yStep = viewport.Height / xyTileCount;

            for (long y = 0; y < xyTileCount; y++)
            {
                for (long x = 0; x < xyTileCount; x++)
                {
                    GdTileIndex index = new GdTileIndex(x + 1, y + 1);
                    Coordinate coordinateMin = new Coordinate(viewport.MinX + x * xStep, viewport.MinY + y * yStep);
                    Coordinate coordinateMax = new Coordinate(coordinateMin.X + xStep, coordinateMin.Y + yStep);
                    Envelope env = new Envelope(coordinateMin, coordinateMax);
                    index.Envelope = env;
                    yield return index;
                }
            }
        }

        public List<GdTileIndex> Divide(Envelope viewport, double tileWidth, double tileHeight)
        {
            List<GdTileIndex> worldArray = new List<GdTileIndex>();

            long xindex = 0;
            long yindex = 0;
            for (double y = viewport.MinY; y <= viewport.MaxY - double.Epsilon; y += tileHeight)
            {
                for (double x = viewport.MinX; x <= viewport.MaxX - double.Epsilon; x += tileWidth)
                {
                    GdTileIndex index = new GdTileIndex(xindex, yindex);
                    Coordinate coordinateMin = new Coordinate(x, y);
                    Coordinate coordinateMax = new Coordinate(x + tileWidth, y + tileHeight);
                    Envelope env = new Envelope(coordinateMin, coordinateMax);
                    index.Envelope = env;
                    worldArray.Add(index);
                    yindex++;
                }

                xindex++;
            }

            return worldArray;
        }
    }

    internal sealed class Mtf : ICoordinateSequenceFilter
    {
        private readonly double _ground;

        public Mtf(double ground)
        {
            _ground = ground;
        }

        public bool Done
        {
            get { return false; }
        }

        public bool GeometryChanged
        {
            get { return true; }
        }

        public void Filter(CoordinateSequence seq, int i)
        {
            double z = seq.GetZ(i);
            seq.SetZ(i, z - _ground);
        }
    }
}