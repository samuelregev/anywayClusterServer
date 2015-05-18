using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnywayBackend.Models
{
    public class Accident
    {
        public long AccidentId { get; set; }
        public DateTime date { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int type { get; set; }
        public int subtype { get; set; }
        public int severity { get; set; }
        public int locationAccuracy { get; set; }
    }
}