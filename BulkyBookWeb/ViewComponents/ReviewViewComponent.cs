using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            reviewVM.Review = new BulkyBook.Models.Review();
            reviewVM.Review.ProductId = bookID;

            if(_unitOfWork.Review.GetAll()!=null)
                reviewVM.Reviews = _unitOfWork.Review.GetAll(r => r.ProductId == bookID, includeProperties: "ApplicationUser");
            reviewVM.Title = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == bookID).Title;

            if (reviewVM.Reviews.Count() > 0)
            {
                int sum = 0;
                foreach (var r in reviewVM.Reviews)
                {
                    sum += r.Rating;
                }

                int avg = (int)(sum / reviewVM.Reviews.Count());

                reviewVM.SumOfRatings = sum;
                reviewVM.NumOfRatings = reviewVM.Reviews.Count();
                reviewVM.AvgRating = avg;
            }
            return View("", reviewVM);
           
           
        }
    }
}
