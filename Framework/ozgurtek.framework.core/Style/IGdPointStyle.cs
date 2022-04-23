namespace ozgurtek.framework.core.Style
{
    public interface IGdPointStyle : IGdStyle
    {
        int Size { get; set; }

        GdPointStyleType PointStleType { get; set; }

        IGdStroke Stroke { get; set; }

        IGdFill Fill { get; set; }
    }
}
