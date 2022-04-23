using System;
using System.Collections.Generic;
using System.IO;

namespace ozgurtek.framework.common.Data.Format
{
    public class GdMediaDataSource
    {
        private readonly Uri _basePath;

        public GdMediaDataSource(Uri basePath)
        {
            _basePath = basePath;
        }

        /// <summary>
        /// Puts the given media....
        /// </summary>
        /// <example>hidrant\yeni\428\</example>
        /// <param name="stream">Byte stream of media file.</param>
        /// <param name="mediaPath"></param>       
        /// <param name="mediaType">.png</param>
        public void PutMedia(Stream stream, string mediaPath, string mediaType)
        {
            //imagepath + file name -> dosya adı ile var mı diye kontrol edebilmek için
            string fullPath = Path.Combine(_basePath.OriginalString, mediaPath);
            string extension = Path.GetExtension(mediaPath);

            string fullFilePath; //yazılacak dosyanın tam adresi
            FileMode fileMode; //Dosya yazma modu(open ya da create)

            if (!string.IsNullOrWhiteSpace(extension) && File.Exists(fullPath)) //Dosya var
            {
                fullFilePath = Path.Combine(_basePath.OriginalString, mediaPath);
                fileMode = FileMode.Open;
            }
            else //dosya yok
            {
                //Directoryleri yarat
                string[] directories = mediaPath.Split('/');
                fullFilePath = _basePath.OriginalString;
                for (int i = 0; i < directories.Length; i++)
                {
                    fullFilePath = Path.Combine(fullFilePath, directories[i]);
                    Directory.CreateDirectory(fullFilePath);
                }

                //dosya tipi
                string ext = "." + mediaType.ToLowerInvariant();

                //yaratılacak dosya adı
                string generatedFileName =
                    mediaPath.Replace("/", "_").Replace('\\', '_') + "-" + DateTime.Now.Ticks + ext;
                fullFilePath = Path.Combine(fullFilePath, generatedFileName);
                fileMode = FileMode.Create;
            }

            FileStream targetStream = new FileStream(fullFilePath, fileMode, FileAccess.ReadWrite, FileShare.None);

            stream.CopyTo(targetStream);

            targetStream.Close();
            targetStream.Dispose();
        }

        /// <summary>
        /// gets the given media
        /// </summary>
        /// <param name="mediaPath">Path to get media from</param>
        /// <returns>stream</returns>
        public Stream GetMedia(string mediaPath)
        {
            mediaPath = mediaPath.Replace('/', '\\');
            string filePath = Path.Combine(_basePath.OriginalString, mediaPath);

            // check if file exists
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", mediaPath);

            // open stream
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return stream;
        }

        /// <summary>
        /// deletes given media or folder
        /// </summary>
        /// <param name="mediaPath">Path to delete media from</param>
        public void DeleteMedia(string mediaPath)
        {
            mediaPath = mediaPath.Replace('/', '\\');
            string filePath = Path.Combine(_basePath.OriginalString, mediaPath);

            if (File.Exists(filePath))
                File.Delete(filePath);
            else if (Directory.Exists(filePath))
                Directory.Delete(filePath);
            else
                throw new FileNotFoundException("File not found", mediaPath);
        }

        /// <summary>
        /// Enumarates given media
        /// </summary>
        /// <example>
        /// hidrant\yeni\428\
        /// hidrant\eski\125\
        /// 
        /// returns
        /// hidrant_yeni_428_121545645454.png
        /// hidrant_yeni_428_12154564588989.word
        /// hidrant_yeni_428_121545454545454.mp4
        /// </example>
        /// <param name="path">Path to get containing media files</param>
        /// <returns>
        /// List of media full names
        /// </returns>
        public List<string> GetMediaNames(string path)
        {
            path = path.Replace("/", "\\");

            string fullPath = Path.Combine(_basePath.OriginalString, path); //todo: sıkıntı

            if (!Directory.Exists(fullPath))
                return new List<string>();

            string[] files =
                Directory.GetFiles(fullPath, "*", SearchOption.TopDirectoryOnly); //todo: sıkıntı klasör boşsa

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                files[i] = file.Replace(_basePath.OriginalString, "").TrimStart('\\').Replace("\\", "/");
            }

            List<string> result = new List<string>(files);
            return result;
        }
    }
}