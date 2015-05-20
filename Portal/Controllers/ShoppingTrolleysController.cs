﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portal;
using Portal.Models;
using Portal.ViewModels;
using Portal.DTO;

namespace Portal.Controllers
{
    [Authorize]
    [Portal.Filter.AuthorizationFilter]
    public class ShoppingTrolleysController : Controller
    {
        private MarketContext db = new MarketContext();

        public ActionResult Home()
        {
            var parentCategory = (from p in this.db.ParentCategories
                                  select p).ToList();
            List<CategoryGroup> categoryGroupList = new List<CategoryGroup>();
            foreach (var p in parentCategory)
            {
                CategoryGroup categoryGroup = new CategoryGroup();
                var categories = (from item in this.db.SubCategories
                                  where item.ParentCategoryId == p.ParentCategoryId
                                  select item.CategoryName).ToList();
                categoryGroup.ParentCategory = p.CategoryName;
                categoryGroup.SubCategory = categories;

                categoryGroupList.Add(categoryGroup);
            }

            ViewBag.CategoryList = categoryGroupList;

            var userId = this.db.UserProfiles.Where(x => x.UserName == User.Identity.Name).Select(p => p.UserId).FirstOrDefault();
            var commodityInShoppingTrolley = (from commodity in this.db.CommodityInShoppingTrolleys
                                              where commodity.UserId == userId
                                              select commodity).ToList();
            List<CommodityInShoppingTrolleyViewModel> model = new List<CommodityInShoppingTrolleyViewModel>();
            foreach (var p in commodityInShoppingTrolley)
            {
                CommodityInShoppingTrolleyViewModel commodityInShoppingTrolleyViewModel = new CommodityInShoppingTrolleyViewModel();
                commodityInShoppingTrolleyViewModel.UserId = userId;
                commodityInShoppingTrolleyViewModel.CommodityId = p.CommodityId;
                commodityInShoppingTrolleyViewModel.CommodityName = p.Commodity.CommodityName;
                commodityInShoppingTrolleyViewModel.Color = p.Color;
                commodityInShoppingTrolleyViewModel.Size = p.Size;
                commodityInShoppingTrolleyViewModel.Quantity = p.Quantity;
                commodityInShoppingTrolleyViewModel.Description = p.Commodity.Description;
                commodityInShoppingTrolleyViewModel.Degree = p.Commodity.Degree;
                commodityInShoppingTrolleyViewModel.Price = (p.Quantity * p.UnitPrice);
                commodityInShoppingTrolleyViewModel.UnitPrice = p.UnitPrice;
                commodityInShoppingTrolleyViewModel.Capacity = p.Capacity;
                model.Add(commodityInShoppingTrolleyViewModel);
            }

            ViewBag.CommodityInShoppingTrolleys = model;
            ViewBag.Count = model.Count;

            var commodityInShoppingTrolleyList = (from shoppingTrolleyItem in this.db.ShoppingTrolleys
                                                  from commodityInShoppingTrolleyItem in this.db.CommodityInShoppingTrolleys
                                                  where shoppingTrolleyItem.UserId == commodityInShoppingTrolleyItem.UserId
                                                  && shoppingTrolleyItem.UserProfile.UserName == User.Identity.Name
                                                  select commodityInShoppingTrolleyItem).ToList();
            ViewBag.ShoppingTrolleysCount = commodityInShoppingTrolleyList.Count;
            return View();
        }

        [HttpPost]
        public JsonResult GetNotCheckedTotalCost(Cost cost)
        {
            int a = 0;
            int b = 0;
            if (!string.IsNullOrEmpty(cost.CurrentPrice))
            {
                a = Convert.ToInt32(cost.CurrentPrice);
            }

            if (!string.IsNullOrEmpty(cost.LastPrice))
            {
                b = Convert.ToInt32(cost.LastPrice);
            }

            int Cost = b - a;
            return this.Json(Cost);
        }

        [HttpPost]
        public JsonResult GetCheckedTotalCost(Cost cost)
        {
            int a = 0;
            int b = 0;
            if (!string.IsNullOrEmpty(cost.CurrentPrice))
            {
                a = Convert.ToInt32(cost.CurrentPrice);
            }

            if (!string.IsNullOrEmpty(cost.LastPrice))
            {
                b = Convert.ToInt32(cost.LastPrice);
            }

            int Cost = a + b;
            return this.Json(Cost);
        }

        public JsonResult GetAllShoppingTrolleysCost(int? id) 
        {
            double cost = 0;
            if(id == 1)
            {
                var commodityInShoppingTrolley = (from commodity in this.db.CommodityInShoppingTrolleys
                                                  from shoppingTrolley in this.db.ShoppingTrolleys
                                               where commodity.UserId == shoppingTrolley.UserId
                                               select commodity).ToList();
            foreach (var p in commodityInShoppingTrolley)
            {
                cost = cost + (p.Quantity.Value * p.UnitPrice.Value);
            }
            }

            return this.Json(cost, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SingleDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = this.db.UserProfiles.FirstOrDefault(e => e.UserName == User.Identity.Name).UserId;
            var commodityInShoppingTrolley = this.db.CommodityInShoppingTrolleys.SingleOrDefault(e => e.CommodityId == id && e.UserId == userId);
            this.db.CommodityInShoppingTrolleys.Remove(commodityInShoppingTrolley);
            var restcommodityInShoppingTrolley = this.db.CommodityInShoppingTrolleys.SingleOrDefault(e => e.CommodityId != id && e.UserId == userId);
            if (restcommodityInShoppingTrolley == null)
            {
                var shoppingTrolley = this.db.ShoppingTrolleys.SingleOrDefault(p => p.UserId == userId);
                this.db.ShoppingTrolleys.Remove(shoppingTrolley);
            }

            this.db.SaveChanges();
            return RedirectToAction("Home");
        }

        // GET: ShoppingTrolleys
        public ActionResult Index()
        {
            var shoppingTrolleys = db.ShoppingTrolleys.Include(s => s.UserProfile);
            return View(shoppingTrolleys.ToList());
        }

        // GET: ShoppingTrolleys/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ShoppingTrolley shoppingTrolley = db.ShoppingTrolleys.Find(id);
            if (shoppingTrolley == null)
            {
                return HttpNotFound();
            }

            return View(shoppingTrolley);
        }

        // GET: ShoppingTrolleys/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View();
        }

        // POST: ShoppingTrolleys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,CreationDate,UpdateDate")] ShoppingTrolley shoppingTrolley)
        {
            if (ModelState.IsValid)
            {
                shoppingTrolley.CreationDate = DateTime.Now;
                db.ShoppingTrolleys.Add(shoppingTrolley);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", shoppingTrolley.UserId);
            return View(shoppingTrolley);
        }

        // GET: ShoppingTrolleys/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingTrolley shoppingTrolley = db.ShoppingTrolleys.Find(id);
            if (shoppingTrolley == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", shoppingTrolley.UserId);
            return View(shoppingTrolley);
        }

        // POST: ShoppingTrolleys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,CreationDate,UpdateDate")] ShoppingTrolley shoppingTrolley)
        {
            if (ModelState.IsValid)
            {
                shoppingTrolley.UpdateDate = DateTime.Now;
                db.Entry(shoppingTrolley).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", shoppingTrolley.UserId);
            return View(shoppingTrolley);
        }

        // GET: ShoppingTrolleys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingTrolley shoppingTrolley = db.ShoppingTrolleys.Find(id);
            if (shoppingTrolley == null)
            {
                return HttpNotFound();
            }
            return View(shoppingTrolley);
        }

        // POST: ShoppingTrolleys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ShoppingTrolley shoppingTrolley = db.ShoppingTrolleys.Find(id);
            db.ShoppingTrolleys.Remove(shoppingTrolley);
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
    }
}
