using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Warehouse.Models;
using System.Xml.Linq;
using System.Globalization;

namespace Warehouse.Controllers
{
    public class CurrenciesController : Controller
    {
        private Warehouse_DBEntities db = new Warehouse_DBEntities();

        // GET: Currencies
        public async Task<ActionResult> Index()
        {
            return View(await db.Currency.ToListAsync());
        }

        // GET: Currencies/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = await db.Currency.FindAsync(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // GET: Currencies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Currencies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Code,Rate")] Currency currency)
        {
            if (ModelState.IsValid)
            {
                currency.Date = DateTime.Now;
                db.Currency.Add(currency);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(currency);
        }

        // GET: Currencies/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = await db.Currency.FindAsync(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // POST: Currencies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,Rate")] Currency currency)
        {
            if (ModelState.IsValid)
            {
                currency.Date = DateTime.Now;
                db.Entry(currency).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(currency);
        }

        // GET: Currencies/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = await db.Currency.FindAsync(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // POST: Currencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Currency currency = await db.Currency.FindAsync(id);
            db.Currency.Remove(currency);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> UpdateRates()
        {
            WebClient client = new WebClient();
            var xml = client.DownloadString("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange");
            XDocument xdoc = XDocument.Parse(xml);
            var el = xdoc.Element("exchange").Elements("currency");
            var u = el.Where(x => x.Element("cc").Value == "USD").Select(x => x.Element("rate").Value).FirstOrDefault();
            var e = el.Where(x => x.Element("cc").Value == "EUR").Select(x => x.Element("rate").Value).FirstOrDefault();
            CultureInfo culture = new CultureInfo("en-US");
            decimal usd = Convert.ToDecimal(u, culture);
            decimal eur = Convert.ToDecimal(e, culture);

            decimal uah = 1 / usd;
            eur = usd / eur;
            usd = 1;

            List<Currency> currencies = db.Currency.Select(x => x).ToList();
            foreach(var item in currencies)
            {
                if(item.Code == "UAH")
                {
                    item.Rate = Math.Round(uah, 3);
                    item.Date = DateTime.Now.ToUniversalTime();
                    db.Entry(item).State = EntityState.Modified;
                }
                if (item.Code == "USD")
                {
                    item.Rate = usd;
                    item.Date = DateTime.Now;
                    db.Entry(item).State = EntityState.Modified;
                }
                if (item.Code == "EUR")
                {
                    item.Rate = Math.Round(eur, 3);
                    item.Date = DateTime.Now;
                    db.Entry(item).State = EntityState.Modified;
                }
            }
            await db.SaveChangesAsync();
            await UpdatePrice();

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> UpdatePrice()
        {
            var products = await db.Product.ToListAsync();
            var currencys = await db.Currency.ToListAsync();
            foreach(var item in products)
            {
                item.Price = Math.Round(item.Price_base / item.Currency.Rate, 3);
                db.Entry(item).State = EntityState.Modified;
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
