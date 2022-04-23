using System;
using NetTopologySuite.Geometries;

namespace ozgurtek.framework.core.Mapping
{
    public interface IGdViewport
    {
        Envelope World { get; set; }

        Envelope View { get; set; }

        Coordinate WorldtoView(Coordinate coordinate);

        Coordinate ViewtoWorld(Coordinate point);

        int Srid { get; set; }

        double Scale { get; set; }

        double PixelScale { get; }

        EventHandler Changed { get; set; }
    }
}
