using System;

namespace ozgurtek.framework.common.Geodesy
{
    public class GdDegree
    {
        private readonly double _degree;
        private readonly double _minute;
        private readonly double _second;
        private readonly bool _positiveSide;
        private readonly double _value;
        private bool _isLon = true;
        private int _precision = 4;
        private GdDegreeFormat _degreeFormat = GdDegreeFormat.DecimalDegrees;

        public GdDegree(double degree, double minute = 0, double second = 0, bool positiveSide = true)
        {
            _degree = degree;
            _minute = minute;
            _second = second;
            _positiveSide = positiveSide;
            _value = (_positiveSide ? 1 : -1) * (_degree + _minute / 60 + _second / 3600);
        }

        public GdDegree(double value)
        {
            _value = value;
            _positiveSide = value >= 0;
            double absValue = Math.Abs(_value);
            _degree = Math.Floor(absValue);
            _minute = Math.Floor((absValue - _degree) * 60);
            _second = (absValue - _degree - (_minute / 60)) * 3600;
        }

        public double Value
        {
            get { return Math.Round(_value, Precision); }
        }

        public bool IsLon
        {
            get { return _isLon; }
            set { _isLon = value; }
        }

        public GdDegreeFormat Format
        {
            get { return _degreeFormat; }
            set { _degreeFormat = value; }
        }

        public int Precision
        {
            get { return _precision; }
            set { _precision = value; }
        }

        public override string ToString()
        {
            if (Format == GdDegreeFormat.DecimalDegrees)
            {
                return Value.ToString();
            }

            //Deg, DegMin, DegMinSec
            string side = IsLon ? (_positiveSide ? "N" : "S") : (_positiveSide ? "E" : "W");

            string str = $"{_degree}°";

            if (Format != GdDegreeFormat.Deg)
            {
                //DegMin, DegMinSec
                str += $" {_minute}'";
            }

            if (Format == GdDegreeFormat.DegMinSec)
            {
                //DegMinSec
                str += $" {Math.Round(_second, Precision).ToString()}''";
            }

            str += $" {side}";

            return str;
        }
    }
}
