using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AnywayBackend.Models;
using WebApi.OutputCache.V2;
using AnywayBackend.Classes;

namespace AnywayBackend.Controllers
{
    public class AccidentsApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // get accidents list by parameters, just fro testing purposes

        // GET: /api/?ne_lat=32.4&ne_lng=35.2&sw_lat=31.7&sw_lng=34.4&start_date=1356991200&end_date=1357077600&zoom_level=15&show_fatal=1&show_severe=1&show_light=1&show_inaccurate=1
        [Route("api")]
        [CacheOutput(ClientTimeSpan = int.MaxValue, ServerTimeSpan = int.MaxValue)]
        public IQueryable<Accident> GetAccidentsByParameters(double ne_lat, double ne_lng, double sw_lat, double sw_lng,
            string start_date, string end_date, int show_fatal, int show_severe, int show_light, int show_inaccurate, int zoom_level)
        {
            DateTime startDate = UnixTimeStampToDateTime(double.Parse(start_date));
            DateTime endDate = UnixTimeStampToDateTime(double.Parse(end_date));

            const int SEVERITY_FATAL = 1; // תאונה קטלנית
            const int SEVERITY_SEVERE = 2; // קשה
            const int SEVERITY_LIGHT = 3; // קלה

            int locationAccuracy = show_inaccurate == 1 ? 3 : 1;
            int showFatal = show_fatal == 1 ? SEVERITY_FATAL : 0;
            int showSevere = show_severe == 1 ? SEVERITY_SEVERE : 0;
            int showLight = show_light == 1 ? SEVERITY_LIGHT : 0;

            return db.Accidents.Where(s => 
                (s.severity == showFatal || s.severity == showSevere || s.severity == showLight) &&
                s.latitude >= sw_lat && s.latitude <= ne_lat &&
                s.longitude >= sw_lng && s.longitude <= ne_lng &&
                s.date >= startDate && s.date <= endDate &&
                s.locationAccuracy <= locationAccuracy);
        }

        // this is where the magic happend, calculating the accidents clusters 
        // GET: /api/cluster?ne_lat=32.40978390688122&ne_lng=35.299824657577574&sw_lat=31.760538202995313&sw_lng=34.651631298202574&start_date=1356991200&end_date=1357077600&zoom_level=15&show_fatal=1&show_severe=1&show_light=1&show_inaccurate=1
        [Route("api/cluster")]
        [CacheOutput(ClientTimeSpan = int.MaxValue, ServerTimeSpan = int.MaxValue)]
        public List<AccidentsCluster> GetAccidentsByParametersClustered(double ne_lat, double ne_lng, double sw_lat, double sw_lng,
            string start_date, string end_date, int show_fatal, int show_severe, int show_light, int show_inaccurate, int zoom_level)
        {
            // get date range (as unix time stamp) and convert it to DateTime objects
            DateTime startDate = UnixTimeStampToDateTime(double.Parse(start_date));
            DateTime endDate = UnixTimeStampToDateTime(double.Parse(end_date));

            const int SEVERITY_FATAL = 1;
            const int SEVERITY_SEVERE = 2;
            const int SEVERITY_LIGHT = 3;

            int locationAccuracy = show_inaccurate == 1 ? 3 : 1;
            int showFatal = show_fatal == 1 ? SEVERITY_FATAL : 0;
            int showSevere = show_severe == 1 ? SEVERITY_SEVERE : 0;
            int showLight = show_light == 1 ? SEVERITY_LIGHT : 0;

            IQueryable<Accident> data = db.Accidents.Where(s =>
                (s.severity == showFatal || s.severity == showSevere || s.severity == showLight) &&
                s.latitude >= sw_lat && s.latitude <= ne_lat &&
                s.longitude >= sw_lng && s.longitude <= ne_lng &&
                s.date >= startDate && s.date <= endDate &&
                s.locationAccuracy <= locationAccuracy);

            List<AccidentsCluster> clusters = new List<AccidentsCluster>();

            // if zoom level is < 7 (showing all Israel map) avoid calculatin, just show one marker with all data
            if (zoom_level < 7)
            {
                clusters.Add(new AccidentsCluster(31.774511, 35.011642, data.Count()));
                return clusters;
            }

            // Calculate maximum radius of cluster (in meters), different for each zoom level
            // 2^(16-(7-zoom))
            double max_cluster_radius = 65536;
            for (int i = 7; i < zoom_level; i++)
                max_cluster_radius /= 2;
            
            bool match;
            foreach (Accident a in data)
            {
                // look for a cluster that the distance between the accident and the 
                // cluster center is smaller then max_cluster_radius
                match = false;
                foreach (AccidentsCluster ac in clusters)
                {
                    // calculate distance between current cluster and the accident
                    // if distance is smaller then maxDistance count it as belong to this marker
                    double distanceBetween = distance(ac.lat, ac.lng, a.latitude, a.longitude);
                    if (distanceBetween < max_cluster_radius) {
                        ac.plusOne();
                        match = true;
                        break;
                    }
                }

                // could not find such cluster, create a new cluster.
                // the center of the new cluster will be the accident location
                if (!match)
                    clusters.Add(new AccidentsCluster(a.latitude, a.longitude));
            }
            
            return clusters;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        // return - distance between two lat/lng points in meters
        private double distance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            dist = dist * 1.609344;
            dist = dist * 1000;

            return (dist);
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}