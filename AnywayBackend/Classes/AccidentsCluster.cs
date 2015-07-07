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

        public AccidentsCluster(double lat, double lng, int count = 1)
        {
            this.lat = lat;
            this.lng = lng;
            this.count = count;
        }

        public void plusOne()
        {
            count++;
        }
    }
}