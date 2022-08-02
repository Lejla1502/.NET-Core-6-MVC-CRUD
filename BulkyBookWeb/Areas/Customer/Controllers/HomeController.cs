using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using BulkyBookWeb.Areas.Customer.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public IActionResult Index(int? CategoryId, string BookOrAuthorName)
        {
            //IEnumerable<Product> listOfProducts = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");

            ProductReviewVM productReviewVM = new ProductReviewVM();

            if (CategoryId == null && String.IsNullOrEmpty(BookOrAuthorName))
            {
                productReviewVM.Products = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType").Select(x => new ProductReviewVM.ProductInfo
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ISBN = x.ISBN,
                    Author = x.Author,
                    ListPrice = x.ListPrice,
                    Price = x.Price,
                    Price50 = x.Price50,
                    Price100 = x.Price100,
                    IsFavourite = x.IsFavourite,
                    ImageUrl = x.ImageUrl,
                    AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
                }).ToList();
            }
            else if (CategoryId != null && String.IsNullOrEmpty(BookOrAuthorName))
            {
                productReviewVM.Products = _unitOfWork.Product.GetAll(p => p.CategoryId == CategoryId, includeProperties: "Category,CoverType").Select(x => new ProductReviewVM.ProductInfo
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ISBN = x.ISBN,
                    Author = x.Author,
                    ListPrice = x.ListPrice,
                    Price = x.Price,
                    Price50 = x.Price50,
                    Price100 = x.Price100,
                    IsFavourite = x.IsFavourite,
                    ImageUrl = x.ImageUrl,
                    AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
                }).ToList();
            }
            else if (CategoryId == null && !String.IsNullOrEmpty(BookOrAuthorName))
            {
                productReviewVM.Products = _unitOfWork.Product.GetAll(p => p.Author.Contains(BookOrAuthorName) == true || p.Title.Contains(BookOrAuthorName) == true, includeProperties: "Category,CoverType").Select(x => new ProductReviewVM.ProductInfo
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ISBN = x.ISBN,
                    Author = x.Author,
                    ListPrice = x.ListPrice,
                    Price = x.Price,
                    Price50 = x.Price50,
                    Price100 = x.Price100,
                    IsFavourite = x.IsFavourite,
                    ImageUrl = x.ImageUrl,
                    AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
                }).ToList();
            }
            else
            {
                productReviewVM.Products = _unitOfWork.Product.GetAll(p => p.CategoryId == CategoryId && ((p.Author.Contains(BookOrAuthorName) == true) || (p.Title.Contains(BookOrAuthorName) == true)), includeProperties: "Category,CoverType").Select(x => new ProductReviewVM.ProductInfo
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ISBN = x.ISBN,
                    Author = x.Author,
                    ListPrice = x.ListPrice,
                    Price = x.Price,
                    Price50 = x.Price50,
                    Price100 = x.Price100,
                    IsFavourite = x.IsFavourite,
                    ImageUrl = x.ImageUrl,
                    AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
                }).ToList();
            }

            productReviewVM.CategoryList = _unitOfWork.Category.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
            );

            //var something = _unitOfWork.OrderDetail.GetAll().GroupBy(x => x.ProductId).Select(s => new { Bestseller_PrdouctID = s.Key, Count = s.Count() }).OrderByDescending(y => y.Count).Take(4);

            //productReviewVM.Bestsellers = new();

            //foreach (var p in something)
            //{
            //    productReviewVM.Bestsellers.Add(_unitOfWork.Product.GetFirstOrDefault(x => x.Id == p.Bestseller_PrdouctID));
            //}

            productReviewVM.Bestsellers = _unitOfWork.Product.GetBestsellers();
            productReviewVM.PopularCategories = _unitOfWork.Category.GetFourPopularCategories();
            productReviewVM.NewBooks = _unitOfWork.Product.GetAll().OrderByDescending(x => x.CreatedAt).Take(4).ToList();

           // ProductHomePageVM products= new ProductHomePageVM

            return View(productReviewVM);
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

        public IActionResult GetRecommendProductsComponent(int id)
        {
            return ViewComponent("RecommendProducts", new { bookID = id });
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