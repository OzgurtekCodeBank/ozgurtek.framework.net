using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ozgurtek.framework.common.Mapping
{
    public class GdViewportDivider
    {
        private bool _centerBase = true;

        public List<GdTileIndex> Divide(IGdViewport viewport, int tileWidth, int tileHeight)
        {
            Envelope view = viewport.View;

            int tileArrayWidth = (int)Math.Floor(view.Width / tileWidth) + 1;
            int tileArrayHeight = (int)Math.Floor(view.Height / tileHeight) + 1;
            List<GdTileIndex> worldArray = new List<GdTileIndex>();

            for (int x = 0; x < tileArrayWidth; x++)
            {
                for (int y = 0; y < tileArrayHeight; y++)
                {
                    GdTileIndex index = new GdTileIndex(x, y);
                    Coordinate tl = viewport.ViewtoWorld(new Coordinate(x * tileWidth, y * tileHeight));
                    Coordinate br = viewport.ViewtoWorld(new Coordinate((x + 1) * tileWidth, (y + 1) * tileHeight));
                    Envelope env = new Envelope(tl, br);
                    index.Envelope = env;
                    worldArray.Add(index);
                }
            }

            return CenterBase ? SortByDistance(worldArray) : worldArray;
        }

        private List<GdTileIndex> SortByDistance(List<GdTileIndex> lst)
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

        private int NearestPoint(long x, long y, List<GdTileIndex> lookIn)
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

        public bool CenterBase
        {
            get => _centerBase;
            set => _centerBase = value;
        }
    }
}
