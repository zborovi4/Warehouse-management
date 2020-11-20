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
    public class In_warehouseController : Controller
    {
        private Warehouse_DBEntities db = new Warehouse_DBEntities();

        // GET: In_warehouse
        public async Task<ActionResult> Index()
        {
            //var in_warehouse = db.In_warehouse.Include(i => i.Product).Include(i => i.Warehouse);
            //return View(await in_warehouse.ToListAsync());

            return View(await db.Warehouse.ToListAsync());
        }

        // GET: In_warehouse/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var warehouses = await db.In_warehouse.Where(i => i.Warehouse_id == id).ToListAsync();
            if (warehouses == null)
            {
                return HttpNotFound();
            }
            List<Product> products = new List<Product>();
            foreach (var item in warehouses)
            {
                products.Add(await db.Product.FindAsync(item.Product_id));
                products[products.Count() - 1].Quantity = item.Quantity;
                products[products.Count() - 1].Warehouse_id = id;
            }
            if (products == null)
            {
                return HttpNotFound();
            }

            ViewBag.Wh_id = id;

            return View(products);
        }

        // GET: In_warehouse/Create
        public ActionResult Create(int? id)
        {
            ViewBag.Products = new SelectList(db.Product, "Id", "Name");
            //ViewBag.Category_id = new SelectList(db.Category, "Id", "Name");
            //ViewBag.Currency_id = new SelectList(db.Currency, "Id", "Name");
            ViewBag.Wh_id = id;
            return View();
        }

        // POST: In_warehouse/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Quantity,Warehouse_id,Product_id")] In_warehouse in_Warehouse)
        {
            if (ModelState.IsValid)
            {
                In_warehouse warehouse = await db.In_warehouse.Where(i => i.Warehouse_id == in_Warehouse.Warehouse_id && i.Product_id == in_Warehouse.Product_id).FirstOrDefaultAsync();
                if (warehouse == null)
                {
                    db.In_warehouse.Add(in_Warehouse);
                }
                else
                {
                    warehouse.Quantity += in_Warehouse.Quantity;
                    db.Entry(warehouse).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            //ViewBag.Category_id = new SelectList(db.Category, "Id", "Name", product.Category_id);
            //ViewBag.Currency_id = new SelectList(db.Currency, "Id", "Name", product.Currency_id);
            return View();
        }

        // GET: In_warehouse/Edit/5
        public async Task<ActionResult> Edit(int? id, int? wh_id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Product.FindAsync(id);
            product.Quantity = db.In_warehouse.Where(i => i.Warehouse_id == wh_id && i.Product_id == product.Id).Select(x => x.Quantity).FirstOrDefault();
            product.Warehouse_id = wh_id;
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category_id = new SelectList(db.Category, "Id", "Name", product.Category_id);
            ViewBag.Currency_id = new SelectList(db.Currency, "Id", "Name", product.Currency_id);
            return View(product);
        }

        // POST: In_warehouse/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Price_base,Price,Category_id,Currency_id,Barcode,Quantity,Warehouse_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                In_warehouse in_Warehouse = db.In_warehouse.Where(i => i.Warehouse_id == product.Warehouse_id && i.Product_id == product.Id).FirstOrDefault();
                in_Warehouse.Quantity = product.Quantity;
                Currency currency = await db.Currency.Where(i => i.Id == product.Currency_id).FirstOrDefaultAsync();
                product.Price_base = Math.Round(product.Price * currency.Rate, 3);
                db.Entry(product).State = EntityState.Modified;
                db.Entry(in_Warehouse).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = product.Warehouse_id });
            }
            ViewBag.Category_id = new SelectList(db.Category, "Id", "Name", product.Category_id);
            ViewBag.Currency_id = new SelectList(db.Currency, "Id", "Name", product.Currency_id);
            return View(product);
        }

        // GET: In_warehouse/Delete/5
        public async Task<ActionResult> Delete(int? id, int? wh_id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            In_warehouse in_warehouse = await db.In_warehouse.Where(i => i.Product_id == id && i.Warehouse_id == wh_id).FirstOrDefaultAsync();
            if (in_warehouse == null)
            {
                return HttpNotFound();
            }
            return View(in_warehouse);
        }

        // POST: In_warehouse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            In_warehouse in_warehouse = await db.In_warehouse.FindAsync(id);
            db.In_warehouse.Remove(in_warehouse);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult GetProduct(int id, int wh_id)
        {
            //int wh_id = ViewBag.Wh_id;
            // ViewBag.Cur_Quantity = db.In_warehouse.Where(i => i.Product_id == id && i.Warehouse_id == wh_id).Select(x => x.Quantity).FirstOrDefault();
            Product product = db.Product.Where(x => x.Id == id).FirstOrDefault();
            product.Quantity = db.In_warehouse.Where(i => i.Product_id == id && i.Warehouse_id == wh_id).Select(x => x.Quantity).FirstOrDefault();
            return PartialView(product);
        }

        public ActionResult Amount(int? wh_id)
        {
            var listCurrency = new SelectList(db.Currency, "Id", "Name");
            ViewBag.Wh_id = wh_id;
            return View(listCurrency);
        }

        [HttpPost]
        public ActionResult CalculateAmount(int? id, int? wh_id)
        {
            var products = db.Product.Join(db.In_warehouse, p => p.Id, w => w.Product_id,
                                                    (p, w) => new
                                                    {
                                                        Price_base = p.Price_base,
                                                        Price = p.Price,
                                                        Quantity = w.Quantity,
                                                        Warehouse_id = w.Warehouse_id
                                                    }).Where(x => x.Warehouse_id == wh_id).ToList();
            //decimal currency = db.Currency.Where(c => c.Id == id).Select(x => x.Rate).FirstOrDefault();
            Currency currency = db.Currency.Find(id);
            decimal rate = currency.Rate;
            decimal amount = 0;
            switch (currency.Code)
            {
                case "USD":
                    foreach (var item in products)
                    {
                        amount += item.Price_base * item.Quantity;
                    }
                    break;
                case "EUR":
                    foreach (var item in products)
                    {
                        amount += item.Price_base / rate * item.Quantity;
                    }
                    break;
                case "UAH":
                    foreach (var item in products)
                    {
                        amount += item.Price_base / rate * item.Quantity;
                    }
                    break;
                default:
                    break;

            }
            Amount _amount = new Amount() { Code = currency.Code, WhAmount = Math.Round(amount, 3) };
            return View(_amount);
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
