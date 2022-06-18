using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
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
                IEnumerable<Product> productsList = _unitOfWork.Product.GetRecommended(claim.Value, bookID);
                return View("", productsList);
            }
            else
            {
                IEnumerable<Product> prodList = null;
                return View("", prodList);
            }
        }
    }
}
