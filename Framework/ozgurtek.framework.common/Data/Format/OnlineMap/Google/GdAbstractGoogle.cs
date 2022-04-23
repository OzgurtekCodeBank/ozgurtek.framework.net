namespace ozgurtek.framework.common.Data.Format.OnlineMap.Google
{
    public abstract class GdAbstractGoogle : GdOnlineMap
    {
        private string _secureWord = "Galileo";
        private string _sec1 = "&s=";
        protected string Server = "google.com";

        protected GdAbstractGoogle()
        {
            HttpDownloadInfo.RefererUrl = $"http://maps.{Server}/";
        }

        protected SecureWords GetSecureWords(long x, long y)
        {
            SecureWords securewords = new SecureWords();
            string sec1 = ""; // after &x=...
            string sec2 = ""; // after &zoom=...
            int seclen = (int)((x * 3) + y) % 8;
            sec2 = _secureWord.Substring(0, seclen);
            if (y >= 10000 && y < 100000)
            {
                sec1 = _sec1;
            }
            securewords.SetSec1(sec1);
            securewords.SetSec2(sec2);
            return securewords;
        }

        protected class SecureWords
        {
            string _sec1;
            string _sec2;

            public string GetSec1()
            {
                return _sec1;
            }

            public void SetSec1(string sec1)
            {
                _sec1 = sec1;
            }

            public string GetSec2()
            {
                return _sec2;
            }

            public void SetSec2(string sec2)
            {
                _sec2 = sec2;
            }
        }
    }
}
