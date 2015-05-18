using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnywayBackend.Models
{
    public class AccidentsClusterDetails
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int accidentsCount { get; set; }

        public double sw_lat { get; set; }
        public double sw_lng { get; set; }
        public double ne_lat { get; set; }
        public double ne_lng { get; set; }
    }
}