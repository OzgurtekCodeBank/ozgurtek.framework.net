using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ozgurtek.framework.common.Util
{
    public delegate void LogChangedEventHandler(object sender, LogChangedEventArgs e);

    public sealed class GdFileLogger
    {
        public event LogChangedEventHandler LogChanged;

        private static readonly object SyncRoot = new object();
        private static volatile GdFileLogger _instance;

        private StreamWriter _logWriter;
        private bool _firstTime = true;
        private bool _userCancelled;
        private bool _systemCancelled;
        private string _logFolder;

        private GdFileLogger()
        {
        }

        private static bool CreateDirectory(string path)
        {
            if (Directory.Exists(path))
                return true;

            Directory.CreateDirectory(path);
            return true;
        }

        public string LogFolder
        {
            get => _logFolder;
        }

        public void InitializeLogger(string path)
        {
            _logFolder = path;
            if (!CreateDirectory(path) || !CreateFile(path))
                _systemCancelled = true;
        }

        private bool CreateFile(string path)
        {
            string fileName = $"{DateTime.Today.Day:00}{DateTime.Today.Month:00}{DateTime.Today.Year:0000}.log";
            string fullPath = Path.Combine(path, fileName);
            _logWriter = !File.Exists(fullPath) ? new StreamWriter(fullPath) : File.AppendText(fullPath);
            return true;
        }

        public static GdFileLogger Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new GdFileLogger();
                    }
                }

                return _instance;
            }
        }

        public bool Recording
        {
            get
            {
                return !_systemCancelled && !_userCancelled;
            }
            set
            {
                _userCancelled = value;
            }
        }

        public void Log(string line, LogType type)
        {
            if (string.IsNullOrWhiteSpace(_logFolder))
                throw new Exception("Use InitializeLogger");

            if (!Recording)
                return;

            StringBuilder builder = new StringBuilder();
            if (_firstTime)
            {
                builder.Append(Environment.NewLine);
                _firstTime = false;
            }
            DateTime dt = DateTime.Now;
            builder.Append($"{type} - {dt}");
            builder.Append(Environment.NewLine);
            string replaced = Regex.Replace(line, @"\t|\n|\r", "");
            builder.Append(replaced);
            _logWriter.WriteLine(builder.ToString());
            _logWriter.Flush();
            OnLogChanged(new LogChangedEventArgs(line, dt, type));
        }

        public void LogException(Exception e)
        {
            Log(e.Message + Environment.NewLine + e.StackTrace, LogType.Exception);
        }

        private void OnLogChanged(LogChangedEventArgs e)
        {
            LogChanged?.Invoke(this, e);
        }
    }

    public class LogChangedEventArgs : EventArgs
    {
        public LogChangedEventArgs(string inputString, DateTime time, LogType type)
        {
            Log = inputString;
            Time = time;
            Type = type;
        }
        public string Log { get; }
        public LogType Type { get; }
        public DateTime Time { get; }
    }

    public enum LogType
    {
        Unknown,
        Info,
        Warning,
        Error,
        Exception
    }
}
