using System;
using System.Collections.Generic;
using ozgurtek.framework.common.Util;
using Xamarin.Forms;

namespace ozgurtek.framework.test.xamarin.Managers
{
    public class LogManager
    {
        public void LogException(Exception exception, bool silent = false, Dictionary<string, string> properties = null)
        {
            //console log
            Console.WriteLine(exception);

            //microsoft center log
            Dictionary<string, string> exInfoDict = properties ?? new Dictionary<string, string>();
            //Crashes.TrackError(exception, exInfoDict);

            //file log
            GdFileLogger.Current.LogFolder = GdApp.Instance.Settings.LogFolder;
            GdFileLogger.Current.LogException(exception);

            //user message
            if (!silent)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert("warning", exception.Message, "ok");
                });
            }
        }
    }
}
