using OSGeo.GDAL;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System;
using OSGeo.OSR;
using System.Drawing;
using System.Drawing.Imaging;

namespace ozgurtek.framework.driver.gdal
{
    public class GdGdalDataSource
    {
        private readonly Dataset _ds;
        private readonly string _source;

        internal GdGdalDataSource(Dataset ds, string source)
        {
            _ds = ds;
            _source = source;
        }

        public static GdGdalDataSource Open(string source)
        {
            GdalConfiguration.ConfigureGdal();
            Dataset dataset = Gdal.OpenShared(source, Access.GA_ReadOnly);
            return new GdGdalDataSource(dataset, source);
        }

        public static IEnumerable<string> DriverNames
        {
            get
            {
                GdalConfiguration.ConfigureGdal();
                int count = Gdal.GetDriverCount();
                for (int i = 0; i < count; i++)
                {
                    Driver driver = Gdal.GetDriver(i);
                    yield return driver.ShortName + " " + driver.LongName + " " + driver.GetDescription();
                }
            }
        }

        public Dataset GdalDataSource
        {
            get { return _ds; }
        }

        public string Name
        {
            get { return "Otek Osgeo-Gdal Data Source"; }
        }

        public string Source
        {
            get { return _source; }
        }

        public void Dispose()
        {
            _ds.Dispose();
        }

        public double[] GeoTransform
        {
            get
            {
                double[] geoTransform = new double[6];
                _ds.GetGeoTransform(geoTransform);
                if (geoTransform[0] == 0.0 &&
                    geoTransform[1] == 1.0 &&
                    geoTransform[2] == 0.0 &&
                    geoTransform[3] == 0.0 &&
                    geoTransform[4] == 0.0 &&
                    geoTransform[5] == 1.0)
                    geoTransform[5] = -1.0;

                return geoTransform;
            }
        }

        /// <summary>
        /// convert pixel to geo
        /// </summary>
        /// <param name="x">pixel</param>
        /// <param name="y">pixel</param>
        /// <returns>geo coordinate</returns>
        public Coordinate Project(int x, int y)
        {
            double[] gt = GeoTransform;
            Coordinate result = new Coordinate();
            result.X = gt[0] + x * gt[1] + y * gt[2];
            result.Y = gt[3] + x * gt[4] + y * gt[5];
            return result;
        }

        /// <summary>
        /// convert geo to pixel
        /// </summary>
        /// <param name="x">geo</param>
        /// <param name="y">geo</param>
        /// <returns>pixel</returns>
        public Coordinate UnProject(double x, double y)
        {
            double[] mgt = GeoTransform;
            double[] igt = new double[6];
            Gdal.InvGeoTransform(mgt, igt);

            Coordinate result = new Coordinate();
            result.X = (int)Math.Round(igt[0] + x * igt[1] + y * igt[2]);
            result.Y = (int)Math.Round(igt[3] + x * igt[4] + y * igt[5]);
            return result;
        }

        public int RasterWidth
        {
            get { return _ds.RasterXSize; }
        }

        public int RasterHeight
        {
            get { return _ds.RasterYSize; }
        }

        public Envelope Envelope
        {
            get
            {
                int width = RasterWidth;
                int height = RasterHeight;

                Coordinate[] points = new Coordinate[2];
                points[0] = Project(0, 0);
                points[1] = Project(width, height);

                return new Envelope(points[0], points[1]);
            }
        }

        public string ProjectionString
        {
            get
            {
                SpatialReference spatialReference = _ds.GetSpatialRef();
                if (spatialReference == null)
                    return null;

                spatialReference.ExportToWkt(out var wkt, null);
                return wkt;
            }
        }

        public int BandCount
        {
            get { return _ds.RasterCount; }
        }

        public Bitmap ReadRaster(Rectangle bounds, Size size)
        {
            int rasterCount = _ds.RasterCount;
            if (rasterCount == 0)
                return null;

            int[] bandMap = new[] { 0, 0, 0, 0 };
            int bandCount = 1;
            bool hasAlpha = false;
            bool isIndexed = false;
            bool isFloat = false;
            double minimum = 0.0;
            double maximum = 0.0;
            int channelSize = 8;
            ColorTable colorTable = null;
            int redNoDataValue = -1;
            int greenNoDataValue = -1;
            int blueNoDataValue = -1;
            double noDataValue;
            int hasNoDataValue;
            bool isUndefined = false;

            for (int i = 1; i <= rasterCount; i++)
            {
                Band band = _ds.GetRasterBand(i);
                int chSize = (int)band.DataType;
                if (chSize > 8)
                    channelSize = 16;

                ColorInterp gdc = band.GetColorInterpretation();
                switch (gdc)
                {
                    case ColorInterp.GCI_RedBand:
                    case ColorInterp.GCI_GreenBand:
                    case ColorInterp.GCI_BlueBand:
                    case ColorInterp.GCI_AlphaBand:
                        if (rasterCount == 1)
                            gdc = ColorInterp.GCI_GrayIndex;
                        break;

                    case ColorInterp.GCI_PaletteIndex:
                        if (rasterCount >= 3)
                            gdc = ColorInterp.GCI_RedBand;
                        break;
                }

                switch (gdc)
                {
                    case ColorInterp.GCI_AlphaBand:
                        bandCount = 4;
                        hasAlpha = true;
                        bandMap[3] = i;
                        break;

                    case ColorInterp.GCI_BlueBand:
                        if (bandCount < 3)
                            bandCount = 3;
                        bandMap[0] = i;
                        band.GetNoDataValue(out noDataValue, out hasNoDataValue);
                        if (hasNoDataValue != 0)
                            blueNoDataValue = Convert.ToInt32(noDataValue);
                        break;

                    case ColorInterp.GCI_RedBand:
                        if (bandCount < 3)
                            bandCount = 3;
                        bandMap[2] = i;
                        band.GetNoDataValue(out noDataValue, out hasNoDataValue);
                        if (hasNoDataValue != 0)
                            redNoDataValue = Convert.ToInt32(noDataValue);
                        break;

                    case ColorInterp.GCI_GreenBand:
                        if (bandCount < 3)
                            bandCount = 3;
                        bandMap[1] = i;
                        band.GetNoDataValue(out noDataValue, out hasNoDataValue);
                        if (hasNoDataValue != 0)
                            greenNoDataValue = Convert.ToInt32(noDataValue);
                        break;

                    case ColorInterp.GCI_PaletteIndex:
                        colorTable = band.GetColorTable();
                        isIndexed = true;
                        bandMap[0] = i;
                        band.GetNoDataValue(out noDataValue, out hasNoDataValue);
                        if (hasNoDataValue != 0)
                            redNoDataValue = greenNoDataValue = blueNoDataValue = Convert.ToInt32(noDataValue);
                        break;

                    case ColorInterp.GCI_GrayIndex:
                        {
                            isIndexed = true;
                            bandMap[0] = i;

                            band.GetMinimum(out minimum, out int hasMinValue);
                            band.GetMaximum(out maximum, out int hasMaxValue);
                            isFloat = hasMinValue != 0 && hasMaxValue != 0;

                            band.GetNoDataValue(out noDataValue, out hasNoDataValue);
                            if (hasNoDataValue != 0)
                            {
                                if (isFloat)
                                {
                                    //ColorTable tab = band.GetColorTable();
                                    redNoDataValue = Convert.ToInt32((noDataValue - minimum) * (maximum - minimum) * 255.0);
                                    if (redNoDataValue < 0) redNoDataValue = 0;
                                    {
                                        if (redNoDataValue > 255) redNoDataValue = 255;
                                        {
                                            greenNoDataValue = redNoDataValue;
                                        }
                                    }

                                    blueNoDataValue = redNoDataValue;
                                }
                                else
                                {
                                    redNoDataValue = (int)noDataValue;
                                    greenNoDataValue = (int)noDataValue;
                                    blueNoDataValue = (int)noDataValue;
                                }
                            }
                        }
                        break;

                    case ColorInterp.GCI_Undefined:
                        isUndefined = true;
                        break;

                    default:
                        return null;
                }
            }

            if (isUndefined)
            {
                switch (rasterCount)
                {
                    case 4:
                        bandCount = 4;
                        hasAlpha = true;
                        bandMap[0] = 1;
                        bandMap[1] = 2;
                        bandMap[2] = 3;
                        bandMap[3] = 4;
                        break;

                    case 3:
                        bandCount = 3;
                        bandMap[0] = 1;
                        bandMap[1] = 2;
                        bandMap[2] = 3;
                        break;

                    case 1:
                        {
                            Band band = _ds.GetRasterBand(1);
                            isIndexed = true;

                            band.GetMinimum(out minimum, out int hasMinValue);
                            band.GetMaximum(out maximum, out int hasMaxValue);
                            isFloat = hasMinValue != 0 && hasMaxValue != 0;

                            band.GetNoDataValue(out noDataValue, out hasNoDataValue);
                            if (hasNoDataValue != 0)
                            {
                                if (isFloat)
                                {
                                    redNoDataValue = (int)((noDataValue - minimum) * (maximum - minimum) * 255.0);
                                    if (redNoDataValue < 0) redNoDataValue = 0;
                                    if (redNoDataValue > 255) redNoDataValue = 255;
                                    greenNoDataValue = redNoDataValue;
                                    blueNoDataValue = redNoDataValue;
                                }
                                else
                                {
                                    redNoDataValue = (int)noDataValue;
                                    greenNoDataValue = (int)noDataValue;
                                    blueNoDataValue = (int)noDataValue;
                                }
                            }
                        }
                        break;
                }
            }

            PixelFormat pixelFormat;
            DataType dataType;
            int pixelSpace;
            if (isIndexed)
            {
                pixelFormat = PixelFormat.Format8bppIndexed;
                dataType = isFloat ? DataType.GDT_Float32 : DataType.GDT_Byte;
                pixelSpace = isFloat ? 4 : 1;
            }
            else
            {
                if (bandCount == 1)
                {
                    if (channelSize > 8)
                    {
                        pixelFormat = PixelFormat.Format16bppGrayScale;
                        dataType = DataType.GDT_Int16;
                        pixelSpace = 2;
                    }
                    else
                    {
                        pixelFormat = PixelFormat.Format24bppRgb;
                        bandCount = 3;
                        dataType = DataType.GDT_Byte;
                        pixelSpace = 3;
                    }
                }
                else
                {
                    if (hasAlpha)
                    {
                        if (channelSize > 8)
                        {
                            pixelFormat = PixelFormat.Format64bppArgb;
                            dataType = DataType.GDT_UInt16;
                            pixelSpace = 6;
                        }
                        else
                        {
                            pixelFormat = PixelFormat.Format32bppArgb;
                            dataType = DataType.GDT_Byte;
                            pixelSpace = 4;
                        }

                        bandCount = 4;
                    }
                    else
                    {
                        if (channelSize > 8)
                        {
                            pixelFormat = PixelFormat.Format48bppRgb;
                            dataType = DataType.GDT_UInt16;
                            pixelSpace = 6;
                        }
                        else
                        {
                            pixelFormat = PixelFormat.Format24bppRgb;
                            dataType = DataType.GDT_Byte;
                            pixelSpace = 3;
                        }

                        bandCount = 3;
                    }
                }
            }

            Bitmap result;
            if (isIndexed)
            {
                result = new Bitmap(size.Width, size.Height, PixelFormat.Format8bppIndexed);
                if (colorTable != null)
                {
                    int colorCount = colorTable.GetCount();
                    ColorPalette pal = result.Palette;
                    for (int i = 0; i < colorCount; i++)
                    {
                        ColorEntry colorEntry = colorTable.GetColorEntry(i);
                        pal.Entries[i] = Color.FromArgb(colorEntry.c4, colorEntry.c1, colorEntry.c2, colorEntry.c3);
                    }

                    result.Palette = pal;
                }
                else
                {
                    ColorPalette pal = result.Palette;
                    //if (Symbolizer != null && Symbolizer.ColorRamp != null)
                    //{
                    //    for (int i = 0; i < 256; i++)
                    //        pal.Entries[i] = Symbolizer.ColorRamp.GetColor(i / 255.0f * 100.0f);
                    //}
                    //else
                    {
                        for (int i = 0; i < 256; i++)
                            pal.Entries[i] = Color.FromArgb(255, i, i, i);
                    }

                    result.Palette = pal;
                }
            }
            else
            {
                result = new Bitmap(size.Width, size.Height);
            }

            CPLErr errorCode = CPLErr.CE_None;
            Rectangle lockRect = new Rectangle(0, 0, size.Width, size.Height);
            BitmapData bitmapData = result.LockBits(lockRect, ImageLockMode.ReadWrite, pixelFormat);
            try
            {
                if (isFloat)
                {
                    int lineSpace = pixelSpace * size.Width;
                    float[] sourceBuffer = new float[lineSpace * size.Height];
                    byte[] targetBuffer = new byte[(int)bitmapData.Scan0];

                    errorCode = _ds.ReadRaster(
                        bounds.X,
                        bounds.Y,
                        bounds.Width,
                        bounds.Height,
                        sourceBuffer,
                        size.Width,
                        size.Height,
                        bandCount,
                        bandMap,
                        pixelSpace,
                        lineSpace, 1);

                    if (errorCode != CPLErr.CE_None)
                        return null;

                    for (int row = 0; row < size.Height; row++)
                    {
                        for (int col = 0; col < size.Width; col++)
                        {
                            float floatValue = sourceBuffer[row * size.Width + col];
                            if (floatValue < minimum)
                                floatValue = (float)minimum;
                            if (floatValue > maximum)
                                floatValue = (float)maximum;

                            double ratio = (floatValue - minimum) / (maximum - minimum);
                            byte byteValue = (byte)Math.Round(ratio * 255);
                            targetBuffer[row * bitmapData.Stride + col] = byteValue;
                        }
                    }
                }
                else
                {
                    errorCode = _ds.ReadRaster(
                        bounds.X,
                        bounds.Y,
                        bounds.Width,
                        bounds.Height,
                        bitmapData.Scan0,
                        size.Width,
                        size.Height,
                        dataType,
                        bandCount,
                        bandMap,
                        pixelSpace,
                        bitmapData.Stride, 1);

                    if (errorCode != CPLErr.CE_None)
                        return null;
                }
            }
            finally
            {
                result.UnlockBits(bitmapData);
            }

            if (blueNoDataValue >= 0.0 && redNoDataValue >= 0.0 && greenNoDataValue >= 0.0)
                result.MakeTransparent(Color.FromArgb(redNoDataValue, greenNoDataValue, blueNoDataValue));

            return result;
        }

        public Bitmap ReadRaster(Envelope envelope, Size size)
        {
            Rectangle bounds = UnProject(envelope);
            return ReadRaster(bounds, size);
        }

        public double ReadBand(int bandid, int pixelX, int pixelY)
        {
            double[] bOne = new double[1];
            Band band = _ds.GetRasterBand(bandid);
            band.ReadRaster(pixelX, pixelY, 1, 1, bOne, 1, 1, 0, 0);
            return bOne[0];
        }

        private Rectangle UnProject(Envelope envelope)
        {
            Coordinate p1 = UnProject(envelope.MinX, envelope.MinY);
            Coordinate p2 = UnProject(envelope.MaxX, envelope.MaxY);

            double left = Math.Min(p1.X, p2.X) - 1;
            double right = Math.Max(p1.X, p2.X) + 1;
            double top = Math.Min(p1.Y, p2.Y) - 1;
            double bottom = Math.Max(p1.Y, p2.Y) + 1;

            int[] corners = { (int)left, (int)top, (int)right, (int)bottom };
            for (int i = 0; i < 4; i++)
            {
                int extent = i % 2 == 0 ? RasterWidth : RasterHeight;
                if (corners[i] < 0)
                    corners[i] = 0;
                else if (corners[i] > extent)
                    corners[i] = extent;
            }

            return Rectangle.FromLTRB(corners[0], corners[1], corners[2], corners[3]);
        }
    }
}
