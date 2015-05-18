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
        /*
        // GET: /Home/Import
        public ActionResult Import()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<ActionResult> Import(HttpPostedFileBase file)
        {
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

                if (fileExtension == ".awdat")
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);

                    StreamReader fileRead = new System.IO.StreamReader(@fileLocation);
                    string line = string.Empty;
                    string format = "yyyy-MM-dd h:mm:ss";
                    CultureInfo provider = CultureInfo.InvariantCulture;

                    while ((line = fileRead.ReadLine()) != null)
                    {
                        String[] fileds = line.Split('\t');
                        try
                        {
                            //fileds[0]  ID - not in use
                            //fileds[1]  DATE 2013-01-02 00:00:00
                            //fileds[2]  LATITUDE
                            //fileds[3]  LONGITUDE
                            //fileds[4]  TYPE
                            //fileds[5]  SUBTYPE
                            //fileds[6]  SEVERITY
                            //fileds[7]  LOCATION ACCURACY

                            Accident a = new Accident();
                            a.date = DateTime.ParseExact(fileds[1], format, provider);
                            a.latitude = Convert.ToDouble(fileds[2]);
                            a.longitude = Double.Parse(fileds[3]);
                            a.type = Convert.ToInt32(fileds[4]);
                            a.subtype = Convert.ToInt32(fileds[5]);
                            a.severity = Convert.ToInt32(fileds[6]);
                            a.locationAccuracy = Convert.ToInt32(fileds[7]);

                            db.Accidents.Add(a);
                            await db.SaveChangesAsync();
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.Write("IndexOutOfRangeException in line \"" + line + "\"\n");
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("{0} is not in the correct format.", fileds[1]);
                        }
                    }

                    
                    fileRead.Close();

                }
            }
            return View();
        }
        */
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}