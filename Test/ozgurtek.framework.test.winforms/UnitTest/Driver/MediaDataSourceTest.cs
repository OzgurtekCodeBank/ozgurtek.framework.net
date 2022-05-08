using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using ozgurtek.framework.common.Data.Format;

namespace ozgurtek.framework.test.winforms.UnitTest.Driver
{
    [TestFixture]
    public class MediaDataSourceTest
    {
        private string baseFolder = "C:\\Users\\eniso\\Desktop\\work\\testdata\\media";

        [Test]
        public void PutMediaWithFileNameTest()
        {
            GdMediaDataSource dataSource = new GdMediaDataSource(new Uri(baseFolder));
            MemoryStream stream = new MemoryStream();
            Image img = new Bitmap(Properties.Resources.Image1);
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            dataSource.PutMedia(stream, @"test/1/2/img.jpg", "jpg");
        }

        [Test]
        public void PutMediaWithFolderNameTest()
        {
            GdMediaDataSource dataSource = new GdMediaDataSource(new Uri(baseFolder));
            MemoryStream stream = new MemoryStream();
            Image img = new Bitmap(Properties.Resources.Image1);
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            dataSource.PutMedia(stream, @"test/1/2", "jpg");
        }

        [Test]
        public void GetMediaNamesTest()
        {
            GdMediaDataSource dataSource = new GdMediaDataSource(new Uri(baseFolder));
            List<string> mediaNames = dataSource.GetMediaNames(@"test/1/2");
            Assert.Greater(mediaNames.Count, 0);
        }

        [Test]
        public void GetMediaTest()
        {
            GdMediaDataSource dataSource = new GdMediaDataSource(new Uri(baseFolder));
            List<string> mediaNames = dataSource.GetMediaNames(@"test/1/2");
            foreach (string mediaName in mediaNames)
            {
                string combine = Path.Combine(baseFolder, mediaName);
                Stream inputStream = dataSource.GetMedia(mediaName);
                using (FileStream outputFileStream = new FileStream(combine + "_output.jpg", FileMode.Create))
                {
                    inputStream.CopyTo(outputFileStream);
                }
            }
        }

        [Test]
        public void DeleteMediaTest()
        {
            GdMediaDataSource dataSource = new GdMediaDataSource(new Uri(baseFolder));
            List<string> mediaNames = dataSource.GetMediaNames(@"test/1/2");
            foreach (string mediaName in mediaNames)
            {
                dataSource.DeleteMedia(mediaName);
            }
        }
    }
}