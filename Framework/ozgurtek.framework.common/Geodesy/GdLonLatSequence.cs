using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ozgurtek.framework.common.Geodesy
{
    public class GdLonLatSequence
    {
        private List<GdLonLat> _lonlats;

        public GdLonLatSequence(List<GdLonLat> lonlats)
        {
            _lonlats = lonlats;
        }

        public GdDistance Distance()
        {
            double sum = 0;

            for (int i = 0; i < _lonlats.Count - 1; i++)
            {
                sum += _lonlats[i].DistanceTo(_lonlats[i + 1]).Value;
            }

            return new GdDistance(sum);
        }

        public GdDistance RhumbDistance()
        {
            double sum = 0;

            for (int i = 0; i < _lonlats.Count - 1; i++)
            {
                sum += _lonlats[i].RhumbDistanceTo(_lonlats[i + 1]).Value;
            }

            return new GdDistance(sum);
        }

        public GdArea AreaOf()
        {
            if (_lonlats.Count <= 1)
                return new GdArea(0);

            //make closed if not

            bool notClosed = !_lonlats.First().Equals(_lonlats.Last());
            if (notClosed)
            {
                _lonlats.Add(_lonlats.First());
            }

            double S = 0; // spherical excess in steradians

            for (int v = 0; v < _lonlats.Count - 1; v++)
            {
                double a1 = RadDegConvert.DegreesToRadians(_lonlats[v].Lat.Value);
                double a2 = RadDegConvert.DegreesToRadians(_lonlats[v + 1].Lat.Value);
                double delta_b = RadDegConvert.DegreesToRadians(_lonlats[v + 1].Lon.Value - _lonlats[v].Lon.Value);
                double E = 2 * Math.Atan2(Math.Tan(delta_b / 2) * (Math.Tan(a1 / 2) + Math.Tan(a2 / 2)), 1 + Math.Tan(a1 / 2) * Math.Tan(a2 / 2));
                S += E;
            }

            if (IsPoleEnclosedBy())
                S = Math.Abs(S) - 2 * Math.PI;

            double A = Math.Abs(S * Constants.RADIUS * Constants.RADIUS); // area in units of R

            if (notClosed)
            {
                _lonlats.Remove(_lonlats.Last());
            }

            return new GdArea(A);
        }

        //todo
        public Envelope ExpandToEnvelope()
        {
            return null;
        }

        private bool IsPoleEnclosedBy()
        {
            double sumDelta = 0;
            double prevBrng = _lonlats[0].BearingTo(_lonlats[1]).Value;

            for (int v = 0; v < _lonlats.Count - 1; v++)
            {
                double initBrng = _lonlats[v].BearingTo(_lonlats[v + 1]).Value;
                double finalBrng = _lonlats[v].FinalBearingTo(_lonlats[v + 1]).Value;
                sumDelta += (initBrng - prevBrng + 540) % 360 - 180;
                sumDelta += (finalBrng - initBrng + 540) % 360 - 180;
                prevBrng = finalBrng;
            }

            //todo: yanlış sanki
            double intBrng = _lonlats[0].BearingTo(_lonlats[1]).Value;
            sumDelta += (intBrng - prevBrng + 540) % 360 - 180;

            // TODO: fix (intermittant) edge crossing pole - eg (85,90), (85,0), (85,-90)
            bool enclosed = Math.Abs(sumDelta) < 90; // 0°-ish

            return enclosed;
        }
    }
}
