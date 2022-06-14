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
            //var some = _db.Reviews.GroupJoin(_db.ApplicationUsers,
            //    review => review.ApplicationUserId,
            //    user=>user.Id,
            //    (x, y) => new { Review = x, ApplicationUser = y }).SelectMany(
            //x => x.ApplicationUser,
            //(x, y) => new { user = x.ApplicationUser, review = y });

            //   ctx.Materijal.GroupJoin(ctx.NormativStavka,
            //              materijal => materijal.Id,
            //              normativstavka => normativstavka.MaterijalId,
            //              (x, y) => new { Materijal = x, NormativStavka = y }).SelectMany(
            //x => x.NormativStavka.DefaultIfEmpty(),
            //(x, y) => new { materijal = x.Materijal, NormativStavka = y })

            ReviewVM reviewVM = new ReviewVM();
            //var o = _db.Reviews.ToList().SelectMany(reviews1=>reviews1.ApplicationUser,
            //    (reviews, users)=> new { review, user});
            
            //var some1= _db.ApplicationUsers.GroupJoin(_db.Reviews,
            //    user => user.Id,
            //    review => review.ApplicationUserId,
            //    (x, y) => new { User = x, Review = y }
            //    ).SelectMany(x=>x.Review.DefaultIfEmpty(),
            //    (x,y)=> new { usr=x.User, Review=y });

            //reviewVM.Reviews =_db.ApplicationUsers.GroupJoin(_db.Reviews,
            //    user=>user.Id,
            //    review=>review.ApplicationUserId,
            //    (x,y) => new { User=x, Review=y }
            //    ); 
            //Reviews.Include(y => y.ApplicationUser).Where(x => x.ProductId == bookID);
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
