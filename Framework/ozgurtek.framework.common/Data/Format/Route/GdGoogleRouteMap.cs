using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ozgurtek.framework.common.Data.Format.Route
{
    public sealed class GdGoogleRouteMap : GdMemoryTable
    {

        public string ApiKey { get; set; }
        public IGdProxy Proxy { get; set; }//todo: Tunahan kullan....

        public void LoadFrom(Coordinate start, Coordinate end)
        {
            if (string.IsNullOrEmpty(ApiKey))
                throw new Exception("Required ApiKey");

            string startStr = string.Format("{0},{1}", start.Y.ToString(CultureInfo.InvariantCulture), start.X.ToString(CultureInfo.InvariantCulture));
            string endStr = string.Format("{0},{1}", end.Y.ToString(CultureInfo.InvariantCulture), end.X.ToString(CultureInfo.InvariantCulture));
            string baseUrl = "https://maps.googleapis.com/maps/api/directions/json";
            string url = string.Format("{0}?origin={1}&destination={2}&key={3}&language=tr", baseUrl, startStr, endStr, ApiKey);
            HttpWebRequest request = WebRequest.CreateHttp(url);
            string responseText;
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    var encoding = Encoding.UTF8;
                    using (var reader = new System.IO.StreamReader(stream, encoding))
                    {
                        responseText = reader.ReadToEnd();
                    }
                }
            }

            DecodedResponse decodedResponse = JsonConvert.DeserializeObject<DecodedResponse>(responseText);
            List<List<Step>> routes = new List<List<Step>>();
            foreach (Route route in decodedResponse.routes)
            {
                //mainPathCoordinates = GooglePolylineConverter.Decode(route.overview_polyline.points).ToList();
                List<Step> steps = new List<Step>();
                foreach (Leg leg in route.legs)
                {
                    steps.AddRange(leg.steps);
                }
                routes.Add(steps);
            }

            Load(routes);
        }

        private void Load(List<List<Step>> routes)
        {
            CreateSchema();

            int objectId = 1;
            int routeId = 1;
            foreach (List<Step> route in routes)
            {
                foreach (Step step in route)
                {
                    Coordinate[] coordinates = GooglePolylineConverter.Decode(step.polyline.points).ToArray();
                    string maneuver = step.maneuver;
                    int distance = step.distance.value;
                    int duration = step.duration.value;
                    LineString geometry = new LineString(coordinates);
                    var htmlInstruction = step.html_instructions;
                    GdRowBuffer rowBuffer = new GdRowBuffer();
                    rowBuffer.Put("objectid", objectId);
                    rowBuffer.Put("routeid", routeId);
                    rowBuffer.Put("geometry", geometry);
                    rowBuffer.Put("distance", distance);
                    rowBuffer.Put("duration", duration);
                    rowBuffer.Put("maneuver", maneuver);
                    rowBuffer.Put("htmlInstruction", htmlInstruction);

                    Insert(rowBuffer);

                    objectId++;
                }
                routeId++;
            }
        }

        private void CreateSchema()
        {
            CreateField(new GdField("objectid", GdDataType.Integer));
            CreateField(new GdField("routeid", GdDataType.Integer));
            CreateField(new GdField("geometry", GdDataType.Geometry));
            CreateField(new GdField("distance", GdDataType.Integer));
            CreateField(new GdField("duration", GdDataType.Integer));
            CreateField(new GdField("maneuver", GdDataType.String));
            CreateField(new GdField("htmlInstruction", GdDataType.String));
        }

        #region Json Deserialize classes

        private class GeocodedWaypoint
        {
            public string geocoder_status { get; set; }
            public string place_id { get; set; }
            public List<string> types { get; set; }
        }

        private class Northeast
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        private class Southwest
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        private class Bounds
        {
            public Northeast northeast { get; set; }
            public Southwest southwest { get; set; }
        }

        private class Distance
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        private class Duration
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        private class EndLocation
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        private class StartLocation
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        private class Distance2
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        private class Duration2
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        private class EndLocation2
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        private class Polyline
        {
            public string points { get; set; }
        }

        private class StartLocation2
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        private class Step
        {
            public Distance2 distance { get; set; }
            public Duration2 duration { get; set; }
            public EndLocation2 end_location { get; set; }
            public string html_instructions { get; set; }
            public Polyline polyline { get; set; }
            public StartLocation2 start_location { get; set; }
            public string travel_mode { get; set; }
            public string maneuver { get; set; }
        }

        private class Leg
        {
            public Distance distance { get; set; }
            public Duration duration { get; set; }
            public string end_address { get; set; }
            public EndLocation end_location { get; set; }
            public string start_address { get; set; }
            public StartLocation start_location { get; set; }
            public List<Step> steps { get; set; }
            public List<object> traffic_speed_entry { get; set; }
            public List<object> via_waypoint { get; set; }
        }

        private class OverviewPolyline
        {
            public string points { get; set; }
        }

        private class Route
        {
            public Bounds bounds { get; set; }
            public string copyrights { get; set; }
            public List<Leg> legs { get; set; }
            public OverviewPolyline overview_polyline { get; set; }
            public string summary { get; set; }
            public List<object> warnings { get; set; }
            public List<object> waypoint_order { get; set; }
        }

        private class DecodedResponse
        {
            public List<GeocodedWaypoint> geocoded_waypoints { get; set; }
            public List<Route> routes { get; set; }
            public string status { get; set; }
        }

        #endregion
    }

    /// <summary>
    /// Google Polyline Converter (Encoder and Decoder)
    /// </summary>
    static class GooglePolylineConverter
    {
        /// <summary>
        /// Decodes the specified polyline string.
        /// </summary>
        /// <param name="polylineString">The polyline string.</param>
        /// <returns>A list with Locations</returns>
        public static IEnumerable<Coordinate> Decode(string polylineString)
        {
            if (string.IsNullOrEmpty(polylineString))
                throw new ArgumentNullException(nameof(polylineString));

            var polylineChars = polylineString.ToCharArray();
            var index = 0;

            var currentLat = 0;
            var currentLng = 0;

            while (index < polylineChars.Length)
            {
                // Next lat
                var sum = 0;
                var shifter = 0;
                int nextFiveBits;
                do
                {
                    nextFiveBits = polylineChars[index++] - 63;
                    sum |= (nextFiveBits & 31) << shifter;
                    shifter += 5;
                } while (nextFiveBits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                // Next lng
                sum = 0;
                shifter = 0;
                do
                {
                    nextFiveBits = polylineChars[index++] - 63;
                    sum |= (nextFiveBits & 31) << shifter;
                    shifter += 5;
                } while (nextFiveBits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && nextFiveBits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                double y = Convert.ToDouble(currentLat) / 1E5;
                double x = Convert.ToDouble(currentLng) / 1E5;
                yield return new Coordinate(x, y);
                //{
                //    lat = Convert.ToDouble(currentLat) / 1E5,
                //    lng = Convert.ToDouble(currentLng) / 1E5
                //};
            }
        }

        /// <summary>
        /// Encodes the specified locations list.
        /// </summary>
        /// <param name="locations">The locations.</param>
        /// <returns>The polyline string.</returns>
        private static string Encode(IEnumerable<Coordinate> locations)
        {
            var str = new StringBuilder();

            var encodeDiff = (Action<int>)(diff =>
            {
                var shifted = diff << 1;
                if (diff < 0)
                    shifted = ~shifted;

                var rem = shifted;

                while (rem >= 0x20)
                {
                    str.Append((char)((0x20 | (rem & 0x1f)) + 63));

                    rem >>= 5;
                }

                str.Append((char)(rem + 63));
            });

            var lastLat = 0;
            var lastLng = 0;

            foreach (var point in locations)
            {
                var lat = (int)Math.Round(point.Y * 1E5);
                var lng = (int)Math.Round(point.X * 1E5);

                encodeDiff(lat - lastLat);
                encodeDiff(lng - lastLng);

                lastLat = lat;
                lastLng = lng;
            }

            return str.ToString();
        }
    }
}
