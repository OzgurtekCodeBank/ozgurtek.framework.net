namespace ozgurtek.framework.core.Mapping
{
    public interface IGdLayer
    {
        string Name { get; set; }

        IGdRenderer Renderer { get; set; }
    }
}
