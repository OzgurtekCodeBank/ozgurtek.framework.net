using System;
using System.IO;

namespace ozgurtek.framework.test.xamarin.Managers
{
    public class SettingsManager
    {
        private const string CacheFolderKey = "cache";
        private const string LogFolderKey = "log";

        public string WorkspaceFolder
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); }
        }

        public string CacheFolder
        {
            get
            {
                string path = Path.Combine(WorkspaceFolder, CacheFolderKey);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        public string LogFolder
        {
            get
            {
                string path = Path.Combine(WorkspaceFolder, LogFolderKey);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }
    }
}
