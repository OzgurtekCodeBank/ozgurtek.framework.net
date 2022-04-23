namespace ozgurtek.framework.core.Mapping
{
    public interface IGdLabeledLayer
    {
        IGdRenderer LabelRenderer { get; set; }

        string LabelFormat { get; set; }
    }
}
