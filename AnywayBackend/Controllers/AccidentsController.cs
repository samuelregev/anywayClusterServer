using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AnywayBackend.Models;
using PagedList;

namespace AnywayBackend.Controllers
{
    public class AccidentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        // GET: Accidents
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*")]
        public async Task<ActionResult> Index(int? page, string sortOrder)
        {
            int pageSize = 50;
            int pageNumber = (page ?? 1);

            var Accidents = from a in db.Accidents 
                           select a;

            switch (sortOrder)
            {
                case "date":
                    Accidents = Accidents.OrderBy(a => a.date);
                    break;
                case "location":
                    Accidents = Accidents.OrderBy(a => a.latitude).ThenBy(a => a.longitude);
                    break;
                case "locationNdate":
                    Accidents = Accidents.OrderBy(a => a.latitude).ThenBy(a => a.longitude).ThenBy(a => a.date);
                    break;
                default:
                    Accidents = Accidents.OrderBy(a => a.latitude).ThenBy(a => a.longitude);
                    break;
            }

            

            
            return View(Accidents.ToPagedList(pageNumber, pageSize));
            //return View(await db.Accidents.ToListAsync());
        }

        // GET: Accidents/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accident accident = await db.Accidents.FindAsync(id);
            if (accident == null)
            {
                return HttpNotFound();
            }
            return View(accident);
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
