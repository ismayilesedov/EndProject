﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchuseOnlineShop.Data;
using SchuseOnlineShop.Models;
using SchuseOnlineShop.Services.Interfaces;
using SchuseOnlineShop.ViewModels.Cart;
using System.Data;

namespace SchuseOnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public CartController(IProductService productService, ICartService cartService,AppDbContext context, UserManager<AppUser> userManager)
        {
            _productService = productService;
            _cartService = cartService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login","Account");
            var cart = await _context.Carts
               .Include(m => m.CartProducts)
               .ThenInclude(m => m.Product)
               .ThenInclude(m => m.Brand)
               .Include(m => m.CartProducts)
               .ThenInclude(m => m.Product)
               .ThenInclude(m => m.ProductImages)
               .FirstOrDefaultAsync(m => m.AppUserId == user.Id);

            CartIndexVM model = new CartIndexVM();

            if (cart == null) return View(model);

            foreach (var dbCartProducts in cart.CartProducts)
            {
                CartDetailVM basketProduct = new CartDetailVM
                {
                    Id = dbCartProducts.Id,
                    Name = dbCartProducts.Product.Name,
                    Image = dbCartProducts.Product.ProductImages.FirstOrDefault(m => m.IsMain)?.ImgName,
                    Brand = dbCartProducts.Product.Brand,
                    Count = dbCartProducts.Count,
                    Price = dbCartProducts.Product.DiscountPrice,
                    Total = (dbCartProducts.Product.DiscountPrice * dbCartProducts.Count),
                };
                model.CartDetails.Add(basketProduct);

            }

            return View(model);
        }





        [HttpPost]
        public async Task<IActionResult> AddCart(CartAddVM basketAddVM)
        {
            if (!ModelState.IsValid) return BadRequest(basketAddVM);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var product = await _context.Products.FindAsync(basketAddVM.Id);
            if (product == null) return NotFound();

            var basket = await _context.Carts.FirstOrDefaultAsync(m => m.AppUserId == user.Id);

            if (basket == null)
            {
                basket = new Cart
                {
                    AppUserId = user.Id
                };

                await _context.Carts.AddAsync(basket);
                await _context.SaveChangesAsync();
            }

            var basketProduct = await _context.CartProducts
                .FirstOrDefaultAsync(bp => bp.ProductId == product.Id && bp.CartId == basket.Id);

            if (basketProduct != null)
            {
                basketProduct.Count++;
            }

            else
            {
                basketProduct = new CartProduct
                {
                    CartId = basket.Id,
                    ProductId = product.Id,
                    Count = 1
                };

                await _context.CartProducts.AddAsync(basketProduct);
            }

            await _context.SaveChangesAsync();
            return Ok(await _context.CartProducts.Where(bp => bp.Cart.AppUserId == user.Id).SumAsync(bp => bp.Count));
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var basketProduct = await _context.CartProducts
                .FirstOrDefaultAsync(bp => bp.Id == id
                && bp.Cart.AppUserId == user.Id);

            if (basketProduct == null) return NotFound();

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketProduct.ProductId);
            if (product == null) return NotFound();




            _context.CartProducts.Remove(basketProduct);
            await _context.SaveChangesAsync();
            return Ok(await _context.CartProducts.Where(bp => bp.Cart.AppUserId == user.Id).SumAsync(bp => bp.Count));
        }



        public async Task<IActionResult> CartProductCountChange(int basketId, int quantity)
        {
            if (basketId < 1) return NotFound();
            CartProduct item = _context.CartProducts.FirstOrDefault(m => m.Id == basketId);
            if (item is null) return NotFound();

            item.Count = item.Count + quantity;

            if (item.Count == 0) return RedirectToAction("Index", "Cart");

            Product product = await _productService.GetFullDataByIdAsync(item.ProductId);


            _context.SaveChanges();
            return RedirectToAction("Index", "Cart");
        }



    }
}
