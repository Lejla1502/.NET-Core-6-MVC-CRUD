﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using BulkyBookWeb.Areas.Customer.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Customer.Controllers
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> listOfProducts = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            
            return View(listOfProducts);
        }

        public IActionResult DisplayFavourites()
        {
            IEnumerable<Product> listOfProducts = _unitOfWork.Product.GetAll(x=>x.IsFavourite==true,includeProperties: "Category,CoverType");

            return View(listOfProducts);
        }

        public IActionResult ChatDemo()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Details(int productId)
        {
            ShoppingCart cartObj = new ShoppingCart
            {
                Product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == productId, includeProperties: "Category,CoverType"),
                Count = 1,
                ProductId = productId
            };

            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartFromDB = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.ProductId == shoppingCart.ProductId && x.ApplicationUserId == claim.Value);

            if (cartFromDB == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cartFromDB, shoppingCart.Count);
                _unitOfWork.Save();

            }


            return RedirectToAction(nameof(Index), nameof(CartController).Replace("Controller", ""));
        }

        public IActionResult GetReviewComponent(int id)
        {
            return ViewComponent("Review", new {bookID=id});
        }

        [HttpPost]
        public IActionResult PostReviews(Review review)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            review.ApplicationUserId = claim.Value;

            review.CreatedAt = DateTime.Now;

            _unitOfWork.Review.Add(review);
            _unitOfWork.Save();

            return ViewComponent("Review", new { bookID = review.ProductId });
        }

        //public IActionResult GetReviews(int bookID)
        //{
        //    ReviewVM reviewVM = new ReviewVM
        //    {
        //        Reviews = _unitOfWork.Review.GetAll(r => r.ProductId == bookID).ToList(),
        //        Title = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == bookID).Title
        //    };

        //    return View("_Review", reviewVM);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult PostReviews()
        //{
        //    return View("_Review");
        //}

        public IActionResult AddToFavourite(int id, bool isFromFavouritesPage)
        {
            _unitOfWork.Product.UpdateStatus(id);
            _unitOfWork.Save();

            if (!isFromFavouritesPage)
                return RedirectToAction("Index");
            else
                return RedirectToAction("DisplayFavourites");
        }
        public IActionResult RemoveFromFavourite(int id, bool isFromFavouritesPage)
        {
            _unitOfWork.Product.UpdateStatus(id);
            _unitOfWork.Save();

            if (!isFromFavouritesPage)
                return RedirectToAction("Index");
            else
                return RedirectToAction("DisplayFavourites");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}