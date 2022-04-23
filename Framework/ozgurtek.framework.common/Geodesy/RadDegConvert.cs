namespace ozgurtek.framework.common.Geodesy
{
    internal static class RadDegConvert
    {
        public static double DegreesToRadians(double deg)
        {
            return (Constants.D2R * deg);
        }

        public static double RadiansToDegrees(double rad)
        {
            return (Constants.R2D * rad);
        }
    }
}
