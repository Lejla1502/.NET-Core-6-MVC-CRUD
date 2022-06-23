using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.ViewComponents
{
    public class RecommendProductsViewComponent:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ApplicationDbContext _db;

        public RecommendProductsViewComponent(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync(int bookID)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
               // IEnumerable<Product> productsList = _unitOfWork.Product.GetRecommended(claim.Value, bookID);

                ProductRecommendVM productRecommendVM = new ProductRecommendVM()
                {
                    Products = _unitOfWork.Product.GetRecommended(claim.Value, bookID).Select(x => new ProductRecommendVM.ProductInfo
                    {
                        Id=x.Id,
                        Title = x.Title,
                        Author = x.Author,
                        ImageUrl = x.ImageUrl,
                        Price = x.Price,
                        AvgRating = (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count() > 0) ? (_unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Select(s => s.Rating).Sum() / _unitOfWork.Review.GetAll(y => y.ProductId == x.Id).Count()) : 0
                    }).ToArray()
                };

                return View("", productRecommendVM.Products);
            }
            else
            {
                //IEnumerable<Product> prodList = null;
                ProductRecommendVM productRecommendVM = new ProductRecommendVM();
                productRecommendVM.Products = null;
                return View("", productRecommendVM.Products);
            }
        }
    }
}
