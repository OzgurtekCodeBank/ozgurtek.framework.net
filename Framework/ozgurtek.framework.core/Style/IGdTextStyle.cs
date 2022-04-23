namespace ozgurtek.framework.core.Style
{
    public interface IGdTextStyle : IGdStyle
    {
        IGdStroke Stroke { get; set; }

        IGdFill Fill { get; set; }

        int Size { get; set; }
    }
}
