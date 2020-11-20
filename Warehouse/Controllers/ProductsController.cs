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

namespace Warehouse.Controllers
{
    public class ProductsController : Controller
    {
        private Warehouse_DBEntities db = new Warehouse_DBEntities();

        // GET: Products
        public async Task<ActionResult> Index()
        {
            var product = db.Product.Include(p => p.Category).Include(p => p.Currency);
            return View(await product.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Product.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Category_id = new SelectList(db.Category, "Id", "Name");
            ViewBag.Currency_id = new SelectList(db.Currency, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Price,Category_id,Currency_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                Currency currency = await db.Currency.Where(i => i.Id == product.Currency_id).FirstOrDefaultAsync();
                product.Price_base = Math.Round(product.Price * currency.Rate, 3);
                bool flag = true;
                Random rnd = new Random();
                int barcode = 0;
                while (flag)
                {
                    barcode = rnd.Next(10000000, 99999999);
                    flag = db.Product.Any(i => i.Barcode == barcode);
                }
                product.Barcode = barcode;
                db.Product.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Category_id = new SelectList(db.Category, "Id", "Name", product.Category_id);
            ViewBag.Currency_id = new SelectList(db.Currency, "Id", "Name", product.Currency_id);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Product.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category_id = new SelectList(db.Category, "Id", "Name", product.Category_id);
            ViewBag.Currency_id = new SelectList(db.Currency, "Id", "Name", product.Currency_id);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Price_base,Price,Category_id,Currency_id,Barcode")] Product product)
        {
            if (ModelState.IsValid)
            {
                Currency currency = await db.Currency.Where(i => i.Id == product.Currency_id).FirstOrDefaultAsync();
                product.Price_base = Math.Round(product.Price * currency.Rate, 3);
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Category_id = new SelectList(db.Category, "Id", "Name", product.Category_id);
            ViewBag.Currency_id = new SelectList(db.Currency, "Id", "Name", product.Currency_id);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Product.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Product.FindAsync(id);
            db.Product.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public JsonResult IsItemNoAvailable(string Name, int ID = 0)
        {
            bool itemNoAlradyExist = db.Product.Any(item => item.Name == Name && item.Id != ID);

            if (itemNoAlradyExist)
            {
                return Json("The product name is already available. Try another product name", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
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
