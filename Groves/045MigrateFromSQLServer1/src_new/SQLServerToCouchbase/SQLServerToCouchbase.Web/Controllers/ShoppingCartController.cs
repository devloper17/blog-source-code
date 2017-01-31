﻿using System;
using System.Web.Mvc;
using CouchbaseServerDataAccess;
using SQLServerDataAccess;
using SQLServerToCouchbase.Core;
using SQLServerToCouchbase.Core.Shopping;

namespace SQLServerToCouchbase.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        readonly IShoppingCartRepository _shoppingRepo;

        public ShoppingCartController()
        {
            if (MvcApplication.UseSQL)
                _shoppingRepo = new SqlShoppingCartRepository();
            else
                _shoppingRepo = new CouchbaseShoppingCartRepository();
        }

        public ViewResult Index()
        {
            var carts = _shoppingRepo.GetTenLatestShoppingCarts();
            return View(carts);
        }

        public RedirectToRouteResult SeedData()
        {
            _shoppingRepo.SeedEmptyShoppingCart();
            return RedirectToAction("Index");
        }

        public ViewResult Cart(Guid id)
        {
            var cart = _shoppingRepo.GetCartById(id);
            cart.Id = id;
            return View(cart);
        }

        [HttpPost]
        public RedirectToRouteResult AddItem(Guid cartId, string name, decimal? price, int? quantity)
        {
            // normally validation would go here
            var item = new Item
            {
                Name = name,
                Price = price ?? 0,
                Quantity = quantity ?? 0
            };
            _shoppingRepo.AddItemToCart(cartId, item);
            return RedirectToAction("Cart", new { id = cartId });
        }
    }
}