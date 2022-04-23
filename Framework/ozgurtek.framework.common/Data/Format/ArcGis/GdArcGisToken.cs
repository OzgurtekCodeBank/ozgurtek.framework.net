using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Exception = System.Exception;

namespace ozgurtek.framework.common.Data.Format.ArcGis
{
    public class GdArcGisToken
    {
        private readonly string _tokenUrl;
        private string _user;
        private string _pass;
        private int _expiration = 60 * 24 * 5; //5 days
        private DateTime _expirationTime;
        private string _token;

        public GdArcGisToken(string tokenUrl)
        {
            _tokenUrl = tokenUrl;
        }

        public string User
        {
            get => _user;
            set => _user = value;
        }

        public string Pass
        {
            get => _pass;
            set => _pass = value;
        }

        public string TokenUrl
        {
            get { return _tokenUrl; }
        }

        public int Expiration
        {
            get => _expiration;
            set => _expiration = value;
        }

        public string Token
        {
            get
            {
                if (DateTime.Now <= _expirationTime)
                    return _token;

                using (HttpClient client = new HttpClient())
                {
                    var body = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("username", User),
                        new KeyValuePair<string, string>("password", Pass),
                        new KeyValuePair<string, string>("f", "json"),
                        new KeyValuePair<string, string>("client", "requestIp"),
                        new KeyValuePair<string, string>("expiration", Expiration.ToString())
                    };

                    HttpResponseMessage res = client.PostAsync(_tokenUrl, new FormUrlEncodedContent(body)).Result;
                    if (res.IsSuccessStatusCode)
                    {
                        string result = res.Content.ReadAsStringAsync().Result;
                        GdArcGisTokenResponse response = JsonConvert.DeserializeObject<GdArcGisTokenResponse>(result);
                        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        _expirationTime = start.AddMilliseconds(response.Expires).ToLocalTime();
                        _token = response.Token;
                        if (_token == null)
                           throw new Exception("Esri Token Error " + result);
                        return _token;
                    }

                    throw new Exception("Esri token request returned error: " + res.StatusCode + ". Error Content: " +
                                        res.Content.ReadAsStringAsync().Result);
                }
            }
        }

        public DateTime ExpirationTime
        {
            get { return _expirationTime; }
        }

        internal class GdArcGisTokenResponse
        {
            public long Expires { get; set; }
            public string Token { get; set; }
        }
    }
}