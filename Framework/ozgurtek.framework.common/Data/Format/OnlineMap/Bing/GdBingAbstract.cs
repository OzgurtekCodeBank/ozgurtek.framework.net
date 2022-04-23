using System.Text;

namespace ozgurtek.framework.common.Data.Format.OnlineMap.Bing
{
    public abstract class GdAbstractBing : GdOnlineMap
    {
        protected string Version = "4810";
        protected bool ForceSessionIdOnTileAccess = false;
        protected string SessionId = string.Empty;

        protected string TileXyToQuadKey(long tileX, long tileY, int levelOfDetail)
        {
            StringBuilder quadKey = new StringBuilder();
            for (int i = levelOfDetail; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);
                if ((tileX & mask) != 0)
                {
                    digit++;
                }

                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }

                quadKey.Append(digit);
            }

            return quadKey.ToString();
        }
    }
}