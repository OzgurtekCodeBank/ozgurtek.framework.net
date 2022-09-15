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
        private GdMemoryTable _memTable;
        private const string Id = "gd_id";
        private const string Geometry = "gd_geometry";
        private const string Height = "gd_ext_height";
        private const string Style = "gd_style";
        private const string Description = "gd_description";

        public void Export(IGdTable table, string outputFolder, long xyTileCount, int epsgCode, IGdTrack track)
        {
            if (!Directory.Exists(outputFolder))
                throw new Exception("Folder not exists");

            if (Directory.EnumerateFileSystemEntries(outputFolder).Any())
                throw new Exception("Folder must be empty");

            IGdField field = table.Schema.GetFieldByName(Id);
            if (field == null)
                throw new Exception($"{Id} field missing...");

            field = table.Schema.GetFieldByName(Geometry);
            if (field == null)
                throw new Exception($"{Id} field missing...");

            field = table.Schema.GetFieldByName(Height);
            if (field == null)
                throw new Exception($"{Height} field missing...");

            //divide 4326....
            table.GeometryField = Geometry;
            Envelope project = GdProjection.Project(table.Envelope, DbConvert.ToInt32(epsgCode), 4326);
            List<GdTileIndex> wgsTileIndex = Divide(project, project.Width / xyTileCount, project.Height / xyTileCount);

            //create json models...
            CreateModels(table, outputFolder, wgsTileIndex, epsgCode, track);

            //create sqlite index file...
            CreateIndexFile(outputFolder, wgsTileIndex);

            //finish
            if (track != null)
                track.ReportProgress(100);
        }

        private void CreateModels(IGdTable table, string outputFolder, List<GdTileIndex> tileIndex, int epsgCode, IGdTrack track)
        {
            PrepareNewMemTable();

            long fileName = 0;
            foreach (GdTileIndex index in tileIndex)
            {
                Envelope envelope = GdProjection.Project(index.Envelope, 4326, epsgCode);
                GeometryFactory factory = new GeometryFactory();
                Geometry filterGeom = factory.ToGeometry(envelope);
                filterGeom.SRID = epsgCode;
                table.GeometryFilter = new GdGeometryFilter(filterGeom, GdSpatialRelation.Intersects);

                foreach (IGdRow row in table.Rows)
                {
                    if (row.IsNull(Geometry))
                        continue;

                    if (row.IsNull(Id))
                        continue;

                    GdRowBuffer buffer = new GdRowBuffer();
                    
                    //id
                    buffer.Put(Id, row.GetAsInteger(Id));

                    //geometry
                    Geometry geometry = row.GetAsGeometry(Geometry);
                    geometry.SRID = epsgCode;
                    geometry = GdProjection.Project(geometry, 4326);
                    buffer.Put(Geometry, geometry);

                    //height
                    buffer.Put(Height, row.GetAsReal(Height));

                    //optional style
                    if (row.Table.Schema.GetFieldByName(Style) != null)
                        buffer.Put(Style, row.GetAsReal(Style));

                    //optional description
                    if (row.Table.Schema.GetFieldByName(Description) != null)
                        buffer.Put(Description, row.GetAsString(Description));

                    _memTable.Insert(buffer);
                }

                //write file....
                string fullFileName = Path.Combine(outputFolder, DbConvert.ToString(++fileName) + ".json");
                string geojson = _memTable.ToGeojson(GdGeoJsonSeralizeType.OnlyData, 3);
                File.WriteAllText(fullFileName, geojson);

                PrepareNewMemTable();

                if (track != null)
                    track.ReportProgress(DbConvert.ToDouble(fileName * 100 / tileIndex.Count));
            }
        }

        private void CreateIndexFile(string outputFolder, List<GdTileIndex> tileIndex)
        {
            //crete sqllite index file 
            string path = Path.Combine(outputFolder, "index.sqlite");
            GdSqlLiteDataSource sqlLiteDataSource = GdSqlLiteDataSource.OpenOrCreate(path);
            GdSqlLiteTable sqlLiteTable = sqlLiteDataSource.CreateTable("gd_index", GdGeometryType.Polygon, 4326, null);
            sqlLiteTable.CreateField(new GdField("min_x", GdDataType.Real));
            sqlLiteTable.CreateField(new GdField("min_y", GdDataType.Real));
            sqlLiteTable.CreateField(new GdField("max_x", GdDataType.Real));
            sqlLiteTable.CreateField(new GdField("max_y", GdDataType.Real));

            sqlLiteTable.BeginTransaction();
            
            GeometryFactory factory = new GeometryFactory();
            foreach (GdTileIndex index in tileIndex)
            {
                Geometry geometry = factory.ToGeometry(index.Envelope);
                geometry.SRID = 4326;
                GdRowBuffer buffer = new GdRowBuffer();
                buffer.Put("min_x", index.Envelope.MinX);
                buffer.Put("min_y", index.Envelope.MinY);
                buffer.Put("max_x", index.Envelope.MaxX);
                buffer.Put("max_y", index.Envelope.MaxY);
                buffer.Put("geometry", geometry);
                sqlLiteTable.Insert(buffer);
            }
            
            sqlLiteTable.CommitTransaction();
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

        private void PrepareNewMemTable()
        {
            _memTable = new GdMemoryTable();
            _memTable.CreateField(new GdField(Id, GdDataType.Integer));
            _memTable.CreateField(new GdField(Geometry, GdDataType.Geometry));
            _memTable.CreateField(new GdField(Height, GdDataType.Real));
            _memTable.CreateField(new GdField(Style, GdDataType.Real));
            _memTable.CreateField(new GdField(Description, GdDataType.Real));
        }
    }
}