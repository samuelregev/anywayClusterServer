using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnywayBackend.Controllers
{
    public class DisqusController : Controller
    {
        // GET: Disqus
        [Route("disqus/{shortname}")]
        public ActionResult Index(string shortname, string lat, string lng)
        {
            string discussion_id = "(" + lat + ", " + lng + ")";
            ViewBag.Title = discussion_id;
            ViewBag.diqus_shortname = shortname;
            ViewBag.disqus_id = discussion_id;
            return View();
        }

        // GET: Disqus/login
        public ActionResult login()
        {
            return View();
        }
    }
}