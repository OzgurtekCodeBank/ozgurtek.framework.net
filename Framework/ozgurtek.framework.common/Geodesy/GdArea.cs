using System;

namespace ozgurtek.framework.common.Geodesy
{
    public class GdArea
    {
        private readonly GdAreaUnit _areaUnit = GdAreaUnit.M2;
        private readonly double _value;

        public GdArea(double value)
        {
            _value = value;
        }

        public GdArea(double value, GdAreaUnit areaUnit)
        {
            _value = value;
            _areaUnit = areaUnit;
        }

        public GdArea Convert(GdAreaUnit target, int precision = 4)
        {
            double val = _value * ToBaseUnit(_areaUnit) / ToBaseUnit(target);
            return new GdArea(Math.Round(val, precision), target);
        }

        public string GetString(int precission)
        {
            return $"{Math.Round(_value, precission)} {_areaUnit}";
        }

        public GdAreaUnit AreaUnit
        {
            get { return _areaUnit; }
        }

        public double Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return $"{_value} {_areaUnit}";
        }

        //m2
        private double ToBaseUnit(GdAreaUnit unit)
        {
            double result = 0;
            switch (unit)
            {
                case GdAreaUnit.Cm2:
                    result = 1e-4;
                    break;
                case GdAreaUnit.Mm2:
                    result = 1e-6;
                    break;
                case GdAreaUnit.Km2:
                    result = 1e6;
                    break;
                case GdAreaUnit.M2:
                    result = 1;
                    break;
                case GdAreaUnit.Plaza:
                    result = 64e2;
                    break;
                case GdAreaUnit.Ha:
                    result = 1e4;
                    break;
                case GdAreaUnit.In2:
                    result = 64516e-8;
                    break;
                case GdAreaUnit.Ft2:
                    result = 92903e-6;
                    break;
                case GdAreaUnit.Yd2:
                    result = 836127e-6;
                    break;
            }
            return result;
        }
    }
}
