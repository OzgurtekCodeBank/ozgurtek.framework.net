using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data
{
    public class GdProxy : IGdProxy
    {
        public string Adress { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseDefaultProxy { get; set; }
    }
}
