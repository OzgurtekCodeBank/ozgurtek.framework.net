using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.core.Style;

namespace ozgurtek.framework.common.Mapping
{
    public abstract class GdAbstractWebMapRenderer : IGdRenderer
    {
        private HttpClient _client;

        public void Render(IGdRenderContext context, IGdTrack track = null)
        {
            if (_client == null)
            {
                _client = new HttpClient();
                _client.Timeout = TimeSpan.FromMilliseconds(DownloadInfo.HttpConnectTimeOut);
                _client.DefaultRequestHeaders.Add("User-Agent", DownloadInfo.UserAgent);
                
                //todo: topla
                if (!string.IsNullOrWhiteSpace(DownloadInfo.RefererUrl))
                    _client.DefaultRequestHeaders.Add("Referer", DownloadInfo.RefererUrl);
            }

            _client.CancelPendingRequests();

            List<DownloadObject> objects = GetDownloadObjects(context.Viewport);
            foreach (DownloadObject downloadObject in objects)
            {
                try
                {
                    Geometry geometry = downloadObject.Geometry;

                    //check track
                    if (track != null && track.CancellationPending)
                    {
                        _client.CancelPendingRequests();
                        return;
                    }

                    //memory cache
                    if (DownloadInfo.UseMemoryCache)
                    {
                        byte[] bytes = GdMemoryCache.Instance.Get(downloadObject.CacheFile);
                        if (bytes != null)
                        {
                            Style.Render(context, geometry, bytes);
                            context.Flush();
                            downloadObject.Done = true;
                            continue;
                        }
                    }

                    //disk cache
                    if (DownloadInfo.UseDiskCache && !string.IsNullOrWhiteSpace(DownloadInfo.DiskCacheFolder) && !string.IsNullOrWhiteSpace(downloadObject.CacheFile))
                    {
                        string path = Path.Combine(DownloadInfo.DiskCacheFolder, downloadObject.CacheFile);
                        if (File.Exists(path))
                        {
                            byte[] bytes = File.ReadAllBytes(path);
                            Style.Render(context, geometry, bytes);
                            context.Flush();
                            downloadObject.Done = true;
                            continue;
                        }
                    }

                    //check track
                    if (track != null && track.CancellationPending)
                    {
                        _client.CancelPendingRequests();
                        return;
                    }

                    //get async
                    _client.GetAsync(downloadObject.Uri).ContinueWith(delegate (Task<HttpResponseMessage> response)
                    {
                        try
                        {
                            using (HttpResponseMessage responseMessage = response.Result)
                            {
                                //if (!CheckTileImageHttpResponse(responseMessage))
                                //{
                                //    downloadObject.Done = true;
                                //    return;
                                //}

                                if (!responseMessage.IsSuccessStatusCode)
                                {
                                    downloadObject.Done = true;
                                    return;
                                }

                                //get bytes
                                responseMessage.Content.ReadAsByteArrayAsync().ContinueWith(delegate (Task<byte[]> byteArray)
                                {
                                    try
                                    {
                                        //bytes
                                        byte[] bytes = byteArray.Result;
                                        if (bytes != null)
                                        {
                                            //draw immediately
                                            Style.Render(context, geometry, bytes);
                                            context.Flush();

                                            //memory cache
                                            if (DownloadInfo.UseMemoryCache)
                                                GdMemoryCache.Instance.Add(downloadObject.CacheFile, bytes);

                                            //disk cache
                                            if (DownloadInfo.UseDiskCache && !string.IsNullOrWhiteSpace(DownloadInfo.DiskCacheFolder) && !string.IsNullOrWhiteSpace(downloadObject.CacheFile))
                                            {
                                                string directoryName = Path.GetDirectoryName(downloadObject.CacheFile);
                                                Directory.CreateDirectory(Path.Combine(DownloadInfo.DiskCacheFolder, directoryName));
                                                File.WriteAllBytes(Path.Combine(DownloadInfo.DiskCacheFolder, downloadObject.CacheFile), bytes);
                                            }
                                        }
                                        downloadObject.Done = true;
                                    }
                                    catch (Exception e)
                                    {
                                        downloadObject.Done = true;
                                        Console.WriteLine(e);
                                    }
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            downloadObject.Done = true;
                            Console.WriteLine(e);
                        }
                    });
                }
                catch (Exception ex)
                {
                    downloadObject.Done = true;
                    Console.WriteLine(ex);
                }
            }

            bool loop = true;
            while (loop && track != null && !track.CancellationPending)
            {
                loop = false;
                foreach (DownloadObject data in objects)
                {
                    if (track.CancellationPending)
                        break;

                    if (!data.Done)
                    {
                        loop = true;
                        break;
                    }
                }
            }
        }

        private bool CheckTileImageHttpResponse(HttpResponseMessage response)
        {
            string contentType = response.Content.Headers.ContentType.ToString();
            return contentType.Contains("image");
        }

        protected string GetSafeKey(string key)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex regex = new Regex($"[{Regex.Escape(regexSearch)}]");
            return regex.Replace(key, "");
        }

        public IGdStyle Style { get; set; }
        protected abstract IGdHttpDownloadInfo DownloadInfo { get; }
        protected abstract List<DownloadObject> GetDownloadObjects(IGdViewport viewport);

        protected class DownloadObject
        {
            public Uri Uri;
            public Geometry Geometry;
            public bool Done;
            public string CacheFile;
            
            public DownloadObject(Uri uri, Geometry geometry)
            {
                Uri = uri;
                Geometry = geometry;
            }
        }
    }
}
