using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<Currency> currency;
            using (Warehouse_DBEntities db = new Warehouse_DBEntities())
            {
                currency = db.Currency.ToList();
            }
            if(currency== null)
            {
               return HttpNotFound();
            }
            ViewBag.Date = currency.FirstOrDefault().Date.ToString("d");
            return View(currency);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}