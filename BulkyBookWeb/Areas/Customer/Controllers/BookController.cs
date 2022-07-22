using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly ILogger<HomeController> _logger;

        public BookController(IUnitOfWork unitOfWork)
        {
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

            return View(productReviewVM);
        }

        //public IActionResult GetBestseller()
        //{
        //    //returns id 
        //    var something= _unitOfWork.OrderDetail.GetAll().GroupBy(x => x.ProductId).Select(s => new { Bestseller_PrdouctID = s.Key, Count = s.Count() }).OrderByDescending(y=>y.Count).First();
            
        //    return View(something.Bestseller_PrdouctID);
        //}

        [HttpGet()]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetBestseller()
        {
            var something=  _unitOfWork.OrderDetail.GetAll().GroupBy(x => x.ProductId).Select(s => new { Bestseller_PrdouctID = s.Key, Count = s.Count() }).OrderByDescending(y=>y.Count).First();


            if (something == null)
            {
                return NotFound();
            }

            return something.Bestseller_PrdouctID;
        }

    }
}
