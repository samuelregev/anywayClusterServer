using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnywayBackend.Models;

namespace AnywayBackend.Classes
{
    public class AccidentsCluster
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int count { get; set; }

        public AccidentsCluster(AccidentsClusterDetails acd)
        {
            this.lat = acd.lat;
            this.lng = acd.lng;
            this.count = acd.accidentsCount;
        }
    }
}