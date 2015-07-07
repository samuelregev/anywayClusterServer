using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AnywayBackend.ViewModels;
using AnywayBackend.Models;
using System.Globalization;

namespace AnywayBackend.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult Statistics()
        {
            ViewBag.Message = "Anyway accidents data";

            IQueryable<EnrollmentDateGroup> data = from accident in db.Accidents
                                                   group accident by accident.date into dateGroup
                                                   select new EnrollmentDateGroup()
                                                   {
                                                       EnrollmentDate = dateGroup.Key,
                                                       AccidentCount = dateGroup.Count()
                                                   };
            return View(data.ToList());
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult Contact()
        {
            return View();
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}