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
        public ActionResult Index(string shortname, string identifier, bool newDiscussion = false)
        {
            ViewBag.newDiscussion = newDiscussion;
            ViewBag.Title = identifier;
            ViewBag.diqus_shortname = shortname;
            ViewBag.disqus_id = identifier;
            return View();
        }

        // GET: Disqus/login
        [Route("disqus/login")]
        public ActionResult login()
        {
            return View();
        }

        // GET: Disqus/login
        [Route("disqus/new-discussion")]
        public ActionResult NewDiscussion()
        {
            return View();
        }
    }
}