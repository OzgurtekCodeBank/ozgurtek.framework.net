namespace ozgurtek.framework.core.Data
{
    public interface IGdProxy
    {
        string Adress { get; set; }
        string Port { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        bool UseDefaultProxy { get; set; }
    }
}
