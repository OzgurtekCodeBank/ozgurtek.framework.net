namespace ozgurtek.framework.core.Style
{
    public interface IGdPolygonStyle : IGdStyle
    {
        IGdStroke Stroke { get; set; }

        IGdFill Fill { get; set; }
    }
}
