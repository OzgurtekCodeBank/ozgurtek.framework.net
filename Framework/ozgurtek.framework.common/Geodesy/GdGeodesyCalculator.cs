using GeoAPI.CoordinateSystems;
using NetTopologySuite.Geometries;

namespace ozgurtek.framework.common.Geodesy
{
    public class GdGeodesyCalculator
    {
        public GdArea CalculateArea(Geometry geometry)
        {
            Geometry copy = ConvertTo(geometry);
            return new GdArea(copy.Area, GdAreaUnit.M2);
        }

        public GdDistance CalculateDistance(Geometry geometry)
        {
            Geometry copy = ConvertTo(geometry);
            return new GdDistance(copy.Length, GdDistanceUnit.M);
        }

        private Geometry ConvertTo(Geometry geometry)
        {
            Geometry copy = geometry.Copy();

            ICoordinateSystem coordinateSystem = GdProjection.GetCrs(geometry.SRID);//todo: buraya cache koy...
            IUnit units = coordinateSystem.GetUnits(2);
            if (units is ILinearUnit linearUnit)
                copy.Apply(new LinearUnitFilter(linearUnit.MetersPerUnit));
            else if (units is IAngularUnit angularUnit)
                copy.Apply(new LinearUnitFilter(RadDegConvert.RadiansToDegrees(angularUnit.RadiansPerUnit)));//todo: enis buraya bak...

            return copy;
        }

        internal sealed class LinearUnitFilter : ICoordinateSequenceFilter
        {
            private readonly double _factor;

            public LinearUnitFilter(double factor)
            {
                _factor = factor;
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
                seq.SetX(i, x * _factor);
                seq.SetY(i, y * _factor);
                seq.SetZ(i, z * _factor);
            }
        }
    }
}
