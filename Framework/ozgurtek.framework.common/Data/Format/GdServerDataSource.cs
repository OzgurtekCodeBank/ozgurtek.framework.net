using Newtonsoft.Json;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace ozgurtek.framework.common.Data.Format
{
    public class GdServerDataSource
    {
        public enum CommitType { Insert, Update, Delete }
        private readonly Uri _uri;
        private readonly HttpClient _client;
        private readonly Dictionary<string, string> _queryParameters = new Dictionary<string, string>();
        private readonly string _credential;
        private string _token;
        public IGdProxy Proxy { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="uri">Server location</param>
        /// <param name="credential">User credentials</param>
        public GdServerDataSource(Uri uri, string credential)
        {
            _credential = credential;
            _uri = uri;
            _client = new HttpClient();
        }

        /// <summary>
        /// timeout
        /// </summary>
        public TimeSpan TimeOut
        {
            get { return _client.Timeout; }
            set { _client.Timeout = value; }
        }

        /// <summary>
        /// cancel pending request
        /// </summary>
        public void CancelPendingRequests()
        {
            _client.CancelPendingRequests();
        }

        /// <summary>
        /// Adds Parameter to current query
        /// </summary>
        /// <param name="key">parameter key</param>
        /// <param name="value">parameter value</param>
        /// <exception cref="ArgumentException">Throws exception when specified key already present</exception>
        public void AddParameter(string key, string value)
        {
            if (_queryParameters.ContainsKey(key))
                throw new ArgumentException("Specified key already present.", nameof(key));

            _queryParameters.Add(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearParameters()
        {
            _queryParameters.Clear();
        }

        /// <summary>
        /// Validates credential
        /// </summary>
        /// <returns>True if credential validated, false if not validated</returns>
        public bool ValidateCredential()
        {
            CheckForAuthority();

            string url = _uri.AbsoluteUri + "/CheckUser";
            Uri uri = new Uri(url);
            var payload = new Dictionary<string, string>
            {
                {"token", _token}
            };

            string strPayload = JsonConvert.SerializeObject(payload);
            HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PostAsync(uri, c).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                return responseString == "true";
            }

            throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// Gets Table
        /// </summary>
        /// <param name="tableName">table name present int sps_tables table</param>
        /// <param name="limit">table limit</param>
        /// <param name="offset">table offset</param>
        /// <returns>IGdTable table</returns>
        public GdMemoryTable GetTable(string tableName, long? limit = null, long? offset = null)
        {
            CheckForAuthority();

            //collect query parameters
            string queryParam = null;
            List<string> queryparams = new List<string>();

            if (limit.HasValue)
                queryparams.Add($"sps_limit:{limit}");//sps_limit reserve field

            if (offset.HasValue)
                queryparams.Add($"sps_offset:{offset}");//sps_offset reserve field

            if (_queryParameters.Count > 0)
            {
                foreach (KeyValuePair<string, string> kvp in _queryParameters)
                    queryparams.Add(kvp.Key.Trim() + ":" + kvp.Value.Trim());
            }

            if (queryparams.Count > 0)
                queryParam = string.Join(";", queryparams);

            string url = _uri.AbsoluteUri + "/execute";
            Uri uri = new Uri(url);
            var payload = new Dictionary<string, string>
            {
                {"token", _token},
                {"tablename", tableName},
                {"parameters", queryParam}
            };

            string strPayload = JsonConvert.SerializeObject(payload);
            HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(uri, c).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                return GdMemoryTable.LoadFromJson(responseString);
            }

            throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// Updates table
        /// </summary>
        /// <param name="tableName">table name present int sps_tables table</param>
        /// <param name="updatetable">Table containing values to Edit</param>
        /// <param name="commitType">Edit Type</param>
        /// <returns></returns>
        public GdMemoryTable Commit(string tableName, IGdTable updatetable, CommitType commitType)
        {
            CheckForAuthority();

            string geojson = updatetable.ToGeojson(GdGeoJsonSeralizeType.All);
            string commitTypeStr;
            string token = _token;
            switch (commitType)
            {
                case CommitType.Insert:
                    commitTypeStr = "insert";
                    break;
                case CommitType.Delete:
                    commitTypeStr = "delete";
                    break;
                default:
                    commitTypeStr = "update";
                    break;
            }

            string url = _uri.AbsoluteUri;
            string formatStr = "/UpdateTable";

            url += string.Format(formatStr, token, tableName, geojson, commitTypeStr);
            Uri uri = new Uri(url);
            var payload = new Dictionary<string, string>
            {
                {"token", token},
                {"tablename", tableName},
                {"table", geojson},
                {"executeType", commitTypeStr}
            };

            string strPayload = JsonConvert.SerializeObject(payload);
            HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PostAsync(uri, c).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                return GdMemoryTable.LoadFromJson(responseString);
            }

            throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// Update tables, with transaction
        /// </summary>
        /// <param name="set"></param>
        /// <returns>Result tables</returns>
        public List<GdMemoryTable> Commit(TableSet set)
        {
            CheckForAuthority();

            GdMemoryTable table = new GdMemoryTable();
            table.CreateField(new GdField("table", GdDataType.String));
            table.CreateField(new GdField("executeType", GdDataType.String));

            foreach (Tuple<IGdTable, CommitType> tuple in set.Tupples)
            {
                GdRowBuffer buffer = new GdRowBuffer();
                buffer.Put("table", tuple.Item1.ToGeojson(GdGeoJsonSeralizeType.All));
                buffer.Put("executeType", tuple.Item2.ToString());
                table.Insert(buffer);
            }

            string token = _token;
            string url = _uri.AbsoluteUri;
            string formatStr = "/UpdateTables";
            string geojson = table.ToGeojson(GdGeoJsonSeralizeType.All);

            url += string.Format(formatStr, token, geojson);
            Uri uri = new Uri(url);
            var payload = new Dictionary<string, string>
            {
                {"token", token},
                {"tables", geojson},
            };

            List<GdMemoryTable> result = new List<GdMemoryTable>();
            string strPayload = JsonConvert.SerializeObject(payload);
            HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PostAsync(uri, c).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                GdMemoryTable memoryTable = GdMemoryTable.LoadFromJson(responseString);

                foreach (IGdRow row in memoryTable.Rows)
                {
                    string resultStr = row.GetAsString("result");
                    GdMemoryTable memTable = GdMemoryTable.LoadFromJson(resultStr);
                    result.Add(memTable);
                }
                return result;
            }

            throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// Updates table
        /// </summary>
        /// <param name="param">Table containing values to Edit</param>
        /// <returns></returns>
        public GdMemoryTable Run(IGdTable param)
        {
            CheckForAuthority();

            string geojson = param.ToGeojson(GdGeoJsonSeralizeType.All);
            string token = _token;

            string url = _uri.AbsoluteUri;
            string formatStr = "/Run";

            url += string.Format(formatStr, token, geojson);
            Uri uri = new Uri(url);
            var payload = new Dictionary<string, string>
            {
                {"token", token},
                {"param", geojson},
            };

            string strPayload = JsonConvert.SerializeObject(payload);
            HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PostAsync(uri, c).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                return new GdJsonTableDeserializer().Deserialize(responseString);
            }

            throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// Downloads file
        /// </summary>
        /// <param name="imageName">Path of file presented in GetMediaNames result</param>
        /// <returns></returns>
        public Stream DownloadMedia(string imageName)
        {
            CheckForAuthority();

            var payload = new Dictionary<string, string>
            {
                {"token", _token},
                {"medianame", imageName}
            };

            string strPayload = JsonConvert.SerializeObject(payload);
            HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PostAsync(_uri.AbsoluteUri + "/DownloadMedia", c).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                return responseContent.ReadAsStreamAsync().Result;
            }

            throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// Gets media names in i
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public string[] GetMediaNames(string imagePath)
        {
            CheckForAuthority();

            var payload = new Dictionary<string, string>
            {
                {"token", _token},
                {"path",imagePath }
            };

            string strPayload = JsonConvert.SerializeObject(payload);
            HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PostAsync(_uri.AbsoluteUri + "/GetMediaNames", c).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                var responseString = responseContent.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<string[]>(responseString);
            }

            throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaPath"></param>
        public void DeleteMedia(string mediaPath)
        {
            CheckForAuthority();

            var payload = new Dictionary<string, string>
            {
                {"token", _token},
                {"mediapath", mediaPath}
            };

            string strPayload = JsonConvert.SerializeObject(payload);
            HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PostAsync(_uri.AbsoluteUri + "/DeleteMedia", c).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// </summary>
        /// <param name="mediaPath"></param>
        /// <param name="mediaType"></param>
        /// <param name="stream"></param>
        public void UploadMedia(string mediaPath, string mediaType, Stream stream)
        {
            CheckForAuthority();

            //stream to byte array
            stream.Seek(0, SeekOrigin.Begin);
            byte[] fileToSend = new byte[stream.Length];
            int count = 0;
            while (count < stream.Length)
                fileToSend[count++] = Convert.ToByte(stream.ReadByte());

            string url = _uri.AbsoluteUri;
            url += "/UploadMedia";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/octet-stream";
            request.ContentLength = fileToSend.Length;
            request.Headers.Add("token", _token);
            request.Headers.Add("mediapath", mediaPath);
            request.Headers.Add("mediatype", mediaType);

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileToSend, 0, fileToSend.Length);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            string s = StreamToString(responseStream);
            if (!s.Contains("successfull"))
                throw new Exception(s);
        }

        //Servisin version numarasını ve bilgilerini döner....
        public string GetVersion()
        {
            return _client.GetStringAsync(_uri.AbsoluteUri + "/GetVersion").Result;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _client?.Dispose();
        }

        ///// <summary>
        ///// override token
        ///// </summary>
        ///// <param name="token"></param>
        //public void SetOverrideToken(string token)
        //{
        //    _token = token;
        //}

        /// <summary>
        /// verilen token'ı alır
        /// </summary>
        private void CheckForAuthority()
        {
            if (_token == null)
            {
                string encryptedCredential = GdCrypto.EncryptSimple(_credential);
                encryptedCredential = HttpUtility.UrlEncode(encryptedCredential);
                Uri uri = new Uri(_uri.AbsoluteUri + "/CreateToken");
                var payload = new Dictionary<string, string>
                {
                    {"credential", encryptedCredential}
                };

                string strPayload = JsonConvert.SerializeObject(payload);
                HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(uri, c).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    var str = responseContent.ReadAsStringAsync().Result;
                    str = str.Substring(0, str.Length - 1).Substring(1);
                    _token = str;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        private string StreamToString(Stream stream)
        {
            string result = string.Empty;
            if (stream == null)
                return result;

            using (StreamReader reader = new StreamReader(stream))
                result += reader.ReadToEnd();
            return result;
        }

        public class TableSet
        {
            internal  List<Tuple<IGdTable, CommitType>> Tupples = new List<Tuple<IGdTable, CommitType>>();

            public void Add(IGdTable table, CommitType type)
            {
                Tupples.Add(new Tuple<IGdTable, CommitType>(table, type));
            }
        }
    }
}