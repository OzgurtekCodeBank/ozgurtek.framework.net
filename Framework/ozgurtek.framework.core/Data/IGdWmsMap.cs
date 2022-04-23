using System;
using NetTopologySuite.Geometries;

namespace ozgurtek.framework.core.Data
{
    public interface IGdWmsMap
    {
        string ConnectionString { get; }

        string Name { get; }

        string Title { get; set; }

        Envelope Envelope { get; }

        Uri GetUri(Envelope world, int width, int height);

        int Srid { get; set; }

        IGdHttpDownloadInfo HttpDownloadInfo { get; }

        IGdTable GetFeatureInfo(Envelope envelope, int width, int height, int x, int y, int featureCount = 1);

        string Format { get; set; }

        string Styles { get; set; }

        string GetMapParameters { get; set; }

        string GetFeatureInfoParameters { get; set;}

        IGdWmsMap[] Maps { get; set; }
    }
}