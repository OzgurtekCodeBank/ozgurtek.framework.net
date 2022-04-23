using System;
using System.Net.Http;
using Newtonsoft.Json;
using ozgurtek.framework.common.Data.Format.Wms;
using Exception = System.Exception;

namespace ozgurtek.framework.common.Data.Format.ArcGis
{
    public class GdArcGisServerInfo
    {
        private static Info _info;

        private GdArcGisServerInfo(Info info)
        {
            _info = info;
        }

        public static GdArcGisServerInfo Open(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                Uri uri = new Uri(url);
                string leftPart = uri.GetLeftPart(UriPartial.Authority);
                string req = $"{leftPart}/{uri.Segments[1]}rest/info?f=pjson";
                string s = client.GetStringAsync(req).Result;
                Info info = JsonConvert.DeserializeObject<Info>(s);
                if (info == null || string.IsNullOrWhiteSpace(info.currentVersion))
                    throw new Exception(s);

                return new GdArcGisServerInfo(info);
            }
        }

        public Info ServerInfo
        {
            get { return _info; }
        }

        public class AuthInfo
        {
            public bool isTokenBasedSecurity { get; set; }
            public string tokenServicesUrl { get; set; }
            public int shortLivedTokenValidity { get; set; }
        }

        public class Info
        {
            public string currentVersion { get; set; }
            public string fullVersion { get; set; }
            public string soapUrl { get; set; }
            public string secureSoapUrl { get; set; }
            public string owningSystemUrl { get; set; }
            public AuthInfo authInfo { get; set; }
        }
    }
}
