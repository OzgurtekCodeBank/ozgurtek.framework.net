namespace ozgurtek.framework.core.Style
{
    public interface IGdImageStyle : IGdStyle
    {
        IGdStroke Stroke { get; set; }

        double Transparent { get; set; }
    }
}
