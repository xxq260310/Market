using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portal;
using Portal.Models;

namespace Portal.Controllers
{
    [Authorize]
    [Portal.Filter.AuthorizationFilter]
    public class OrdersController : Controller
    {
        private MarketContext db = new MarketContext();

        [Authorize(Roles = "Administrator")]
        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.OrderDetail);
            return View(orders.ToList());
        }

        [Authorize(Roles="Administrator")]
        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        [Authorize(Roles="Administrator")]
        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.OrderId = new SelectList(db.OrderDetails, "OrderId", "OrderId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,BuyerId,Contact,TotalCost,Address,State,Delivery,ConsigneeName,SellerId,CreationDate,UpdateDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.CreationDate = DateTime.Now;
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(db.OrderDetails, "OrderId", "OrderId", order.OrderId);
            return View(order);
        }

        [Authorize(Roles = "Administrator")]
        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderId = new SelectList(db.OrderDetails, "OrderId", "OrderId", order.OrderId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,BuyerId,Contact,TotalCost,Address,State,Delivery,ConsigneeName,SellerId,CreationDate,UpdateDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.UpdateDate = DateTime.Now;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderId = new SelectList(db.OrderDetails, "OrderId", "OrderId", order.OrderId);
            return View(order);
        }

        [Authorize(Roles = "Administrator")]
        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
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

        public ActionResult Home()
        {
            throw new NotImplementedException();
        }
    }
}
