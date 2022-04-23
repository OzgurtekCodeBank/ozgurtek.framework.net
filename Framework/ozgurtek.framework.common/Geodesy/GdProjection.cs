using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;

namespace ozgurtek.framework.common.Geodesy
{
    public static class GdProjection
    {
        private static readonly Dictionary<int, ICoordinateSystem> CrsList = new Dictionary<int, ICoordinateSystem>();
        private static readonly Dictionary<string, ICoordinateTransformation> TransformList = new Dictionary<string, ICoordinateTransformation>();
        private static readonly Lazy<CoordinateSystemFactory> CoordinateSystemFactory = new Lazy<CoordinateSystemFactory>(() => new CoordinateSystemFactory());

        public static IGdCrsDataSource CrsDataSource = new GdTextCrsDataSource();

        public static ICoordinateSystem GetCrs(int id)
        {
            if (CrsList.ContainsKey(id))
                return CrsList[id];

            IEnumerable<IGdKeyValue> keyValues = CrsDataSource.GetDefination();
            foreach (IGdKeyValue wkt in keyValues)
            {
                if (wkt.Key != id)
                    continue;

                ICoordinateSystem coordinateSystem = CoordinateSystemFactory.Value.CreateFromWkt(wkt.Value);
                if (!CrsList.ContainsKey(id))
                    CrsList.Add(id, coordinateSystem);

                return coordinateSystem;
            }

            return null;
        }

        public static Geometry Project(Geometry geom, int destination)
        {
            ICoordinateTransformation trans = CreateTransformation(geom.SRID, destination);
            Geometry projGeom = Transform(geom, trans.MathTransform);
            projGeom.SRID = destination;
            return projGeom;
        }

        public static Envelope Project(Envelope envelope, int source, int destination)
        {
            ICoordinateTransformation trans = CreateTransformation(source, destination);
            double[] ll = trans.MathTransform.Transform(new[] { envelope.MinX, envelope.MinY, 0 });
            double[] ur = trans.MathTransform.Transform(new[] { envelope.MaxX, envelope.MaxY, 0 });
            return new Envelope(new Coordinate(ll[0], ll[1]), new Coordinate(ur[0], ur[1]));
        }

        public static Coordinate Project(Coordinate coordinate, int source, int destination)
        {
            ICoordinateTransformation trans = CreateTransformation(source, destination);
            double[] transform = trans.MathTransform.Transform(new[] { coordinate.X, coordinate.Y, coordinate.Z });
            return new Coordinate(transform[0], transform[1]);
        }

        public static ICoordinateTransformation CreateTransformation(int source, int destination)
        {
            string key = $"{source}-{destination}";
            if (TransformList.ContainsKey(key))
                return TransformList[key];

            ICoordinateSystem sourceCoordSystem = source == 3857 || 
                                                  source == 900913 ||
                                                  source == 3587 ||
                                                  source == 54004 ||
                                                  source == 41001 ||
                                                  source == 102113 ||
                                                  source == 102100 ||
                                                  source == 3785 ? ProjectedCoordinateSystem.WebMercator : GetCrs(source);

            ICoordinateSystem targetCoordSystem = destination == 3857 ||
                                                  destination == 900913 ||
                                                  destination == 3587 ||
                                                  destination == 54004 ||
                                                  destination == 41001 ||
                                                  destination == 102113 ||
                                                  destination == 102100 ||
                                                  destination == 3785 ? ProjectedCoordinateSystem.WebMercator : GetCrs(destination);

            ICoordinateTransformation trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(sourceCoordSystem, targetCoordSystem);
            if (!TransformList.ContainsKey(key))
                TransformList.Add(key, trans);

            return trans;
        }


        private static Geometry Transform(Geometry geom, IMathTransform transform)
        {
            geom = geom.Copy();
            geom.Apply(new Mtf(transform));
            return geom;
        }
    }

    internal sealed class Mtf : ICoordinateSequenceFilter
    {
        private readonly IMathTransform _mathTransform;

        public Mtf(IMathTransform mathTransform)
        {
            _mathTransform = mathTransform;
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
            double x = seq.GetX(i);
            double y = seq.GetY(i);
            double z = seq.GetZ(i);
            double[] input = { x, y, z };
            double[] transform = _mathTransform.Transform(input);
            seq.SetX(i, transform[0]);
            seq.SetY(i, transform[1]);
            seq.SetZ(i, transform[2]);
        }
    }
}