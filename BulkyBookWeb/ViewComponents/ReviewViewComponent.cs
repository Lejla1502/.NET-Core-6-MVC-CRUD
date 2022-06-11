using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.ViewComponents
{
    public class ReviewViewComponent:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync(int bookID)
        {
            ReviewVM reviewVM = new ReviewVM
            {
                Reviews = _unitOfWork.Review.GetAll(r => r.ProductId == bookID).ToList(),
                Title = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == bookID).Title
            };

            return View("", reviewVM);
            
        }
    }
}
