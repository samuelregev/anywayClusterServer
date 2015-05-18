using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AnywayBackend.Models;
using WebApi.OutputCache.V2;
using AnywayBackend.Classes;


//using System.Data;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Net.Http;
//using System.Web.Http.Description;

namespace AnywayBackend.Controllers
{
    public class AccidentsApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/all
        [Route("api/all")]
        [CacheOutput(ClientTimeSpan = int.MaxValue, ServerTimeSpan = int.MaxValue)]
        public IQueryable<Accident> GetAccidents()
        {
            return db.Accidents;
        }

        // GET: /api/?ne_lat=32.40978390688122&ne_lng=35.299824657577574&sw_lat=31.760538202995313&sw_lng=34.651631298202574&start_date=1356991200&end_date=1357077600&zoom_level=15&show_fatal=1&show_severe=1&show_light=1&show_inaccurate=1
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

            int locationAccuracy = 1;
            if (show_inaccurate == 1)
                locationAccuracy = 3;

            int showFatal = 0;
            if (show_fatal == 1)
                showFatal = SEVERITY_FATAL;

            int showSevere = 0;
            if (show_severe == 1)
                showSevere = SEVERITY_SEVERE;

            int showLight = 0;
            if (show_light == 1)
                showLight = SEVERITY_LIGHT;

            return db.Accidents.Where(s => 
                (s.severity == showFatal || s.severity == showSevere || s.severity == showLight) &&
                s.latitude >= sw_lat && s.latitude <= ne_lat &&
                s.longitude >= sw_lng && s.longitude <= ne_lng &&
                s.date >= startDate && s.date <= endDate &&
                s.locationAccuracy <= locationAccuracy);
        }

        // GET: /api/cluster?ne_lat=32.40978390688122&ne_lng=35.299824657577574&sw_lat=31.760538202995313&sw_lng=34.651631298202574&start_date=1356991200&end_date=1357077600&zoom_level=15&show_fatal=1&show_severe=1&show_light=1&show_inaccurate=1
        [Route("api/cluster")]
        [CacheOutput(ClientTimeSpan = int.MaxValue, ServerTimeSpan = int.MaxValue)]
        public List<AccidentsCluster> GetAccidentsByParametersClustered(double ne_lat, double ne_lng, double sw_lat, double sw_lng,
            string start_date, string end_date, int show_fatal, int show_severe, int show_light, int show_inaccurate, int zoom_level)
        {
            DateTime startDate = UnixTimeStampToDateTime(double.Parse(start_date));
            DateTime endDate = UnixTimeStampToDateTime(double.Parse(end_date));

            const int SEVERITY_FATAL = 1; // תאונה קטלנית
            const int SEVERITY_SEVERE = 2; // קשה
            const int SEVERITY_LIGHT = 3; // קלה

            int locationAccuracy = 1;
            if (show_inaccurate == 1)
                locationAccuracy = 3;

            int showFatal = 0;
            if (show_fatal == 1)
                showFatal = SEVERITY_FATAL;

            int showSevere = 0;
            if (show_severe == 1)
                showSevere = SEVERITY_SEVERE;

            int showLight = 0;
            if (show_light == 1)
                showLight = SEVERITY_LIGHT;

            IQueryable<Accident> data = db.Accidents.Where(s =>
                (s.severity == showFatal || s.severity == showSevere || s.severity == showLight) &&
                s.latitude >= sw_lat && s.latitude <= ne_lat &&
                s.longitude >= sw_lng && s.longitude <= ne_lng &&
                s.date >= startDate && s.date <= endDate &&
                s.locationAccuracy <= locationAccuracy);

            int cluster_count = 3;

            // force one marker when in zoom level showing all israel
            if (zoom_level <= 7)
                cluster_count = 1;

            double cluster_size_lat = (ne_lat - sw_lat) / cluster_count;
            double cluster_size_lng = (ne_lng - sw_lng) / cluster_count;

            List<AccidentsClusterDetails> theList = new List<AccidentsClusterDetails>();

            // create the clusters and set their bounderies
            for (int i = 0; i < cluster_count; i++)
            {
                for (int j = 0; j < cluster_count; j++)
                {
                    AccidentsClusterDetails acd = new AccidentsClusterDetails();
                    acd.ne_lat = ne_lat - i * cluster_size_lat;
                    acd.ne_lng = ne_lng - j * cluster_size_lng;

                    acd.sw_lat = ne_lat - (i + 1) * cluster_size_lat;
                    acd.sw_lng = ne_lng - (j + 1) * cluster_size_lng;

                    acd.lat = acd.ne_lat - ((acd.ne_lat - acd.sw_lat) / 2);
                    acd.lng = acd.ne_lng - ((acd.ne_lng - acd.sw_lng)) / 2;

                    acd.accidentsCount = 0;

                    theList.Add(acd);
                }
            }

            // count accidents in each cluster
            foreach (Accident a in data)
            {
                foreach (AccidentsClusterDetails acd in theList)
                {
                    if (a.latitude >= acd.sw_lat && a.latitude <= acd.ne_lat && 
                        a.longitude >= acd.sw_lng && a.longitude <= acd.ne_lng)
                    {
                        acd.accidentsCount++;
                        break;
                    }
                }
            }

            // remove empty clusters
            List<AccidentsClusterDetails> empty = theList.Where(s => s.accidentsCount == 0).ToList();
            foreach (AccidentsClusterDetails acd in empty)
                theList.Remove(acd);

            // convert to simple-er type to return fewer arguments
            List<AccidentsCluster> returnList = new List<AccidentsCluster>();
            foreach (AccidentsClusterDetails acd in theList)
                returnList.Add(new AccidentsCluster(acd));

            // force marker to center of israel in low zoom levels
            if (zoom_level <= 7)
            {
                returnList.First().lat = 31.774511;
                returnList.First().lng = 35.011642;
            }
            
            return returnList;
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