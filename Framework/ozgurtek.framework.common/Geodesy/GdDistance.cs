using System;

namespace ozgurtek.framework.common.Geodesy
{
    public class GdDistance
    {
        private readonly GdDistanceUnit _unit = GdDistanceUnit.M;
        private readonly double _value;

        public GdDistance(double value)
        {
            _value = value;
        }

        public GdDistance(double value, GdDistanceUnit unit)
        {
            _value = value;
            _unit = unit;
        }

        public GdDistance Convert(GdDistanceUnit target, int precision = 4)
        {
            double val = _value * ToBaseUnit(_unit) / ToBaseUnit(target);
            return new GdDistance(Math.Round(val, precision), target);
        }

        public string GetString(int precission)
        {
            return $"{Math.Round(_value, precission)} {_unit}";
        }

        public override string ToString()
        {
            return $"{_value} {_unit}";
        }

        public GdDistanceUnit Unit
        {
            get { return _unit; }
        }

        public double Value
        {
            get { return _value; }
        }

        //m
        private double ToBaseUnit(GdDistanceUnit unit)
        {
            double result = 0;

            switch (unit)
            {
                case GdDistanceUnit.Km:
                    result = 1e3;
                    break;
                case GdDistanceUnit.M:
                    result = 1;
                    break;
                case GdDistanceUnit.Cm:
                    result = 1e-2;
                    break;
                case GdDistanceUnit.Mm:
                    result = 1e-3;
                    break;
                case GdDistanceUnit.Dm:
                    result = 1e-1;
                    break;
                case GdDistanceUnit.Ft:
                    result = 3048e-4;
                    break;
                case GdDistanceUnit.Mi:
                    result = 160934e-2;
                    break;
                case GdDistanceUnit.Yd:
                    result = 9144e-4;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }

            return result;
        }
    }
}
