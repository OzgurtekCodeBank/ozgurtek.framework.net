namespace ozgurtek.framework.test.xamarin.Managers
{
    public sealed class GdApp
    {
        private static GdApp _instance;
        private static readonly object Padlock = new object();

        private readonly DataManager _dataManager = new DataManager();
        private readonly SettingsManager _settingsManager = new SettingsManager();
        private readonly LogManager _logManager = new LogManager();
        private readonly UtilityManager _utilityManager = new UtilityManager();
        private readonly PermissionManager _permission = new PermissionManager();
        private IGdPlatformManager _platformManager;

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
                return _settingsManager;
            }
        }
        
        public DataManager Data
        {
            get
            {
                return _dataManager;
            }
        }

        public UtilityManager Util
        {
            get
            {
                return _utilityManager;
            }
        }

        public LogManager LogManager
        {
            get
            {
                return _logManager;
            }
        }

        public PermissionManager PermissionManager
        {
            get { return _permission; }
        }

        public IGdPlatformManager PlatformManager
        {
            get { return _platformManager; }
        }
    }

    public interface IGdPlatformManager
    {
        void CloseApp();

        void OpenBrowser(string link);
    }
}
