using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.ViewComponents
{
    public class ReviewViewComponent:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ApplicationDbContext _db;

        public ReviewViewComponent(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync(int bookID)
        {
            ReviewVM reviewVM = new ReviewVM();
            reviewVM.Reviews = _db.Reviews.Include(y => y.ApplicationUser).Where(x => x.ProductId == bookID);
            //reviewVM.Reviews = _unitOfWork.Review.GetAll(r => r.ProductId == bookID, includeProperties: "Product");
            reviewVM.Title = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == bookID).Title;
         

            float sum = 0f;
            foreach(var r in reviewVM.Reviews)
            {
                sum += r.Rating;
            }

            float avg = sum / reviewVM.Reviews.Count();

            reviewVM.SumOfRatings = (decimal)sum;
            reviewVM.NumOfRatings = reviewVM.Reviews.Count();
            reviewVM.AvgRating = (decimal)avg;

            return View("", reviewVM);
            
        }
    }
}
