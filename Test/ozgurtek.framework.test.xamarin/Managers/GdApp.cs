namespace ozgurtek.framework.test.xamarin.Managers
{
    public sealed class GdApp
    {
        private static GdApp _instance;
        private static readonly object Padlock = new object();

        public static GdApp Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new GdApp());
                }
            }
        }

        public SettingsManager Settings
        {
            get
            {
                return new SettingsManager();
            }
        }
        
        public DataManager Data
        {
            get
            {
                return new DataManager();
            }
        }

        public Util Util
        {
            get
            {
                return new Util();
            }
        }

        //public LogManager LogManager { get; set; }
    }
}
