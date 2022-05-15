using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.ViewComponents
{
    public class OrderNotificationViewComponent:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderNotificationViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //if a user is logged in, get their session
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                //if session is null only then we want to go to the db
                var appUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);
                return View(appUser.Name.ToString());
            }
            else
            {
                //this can happen in two instances - it can happen if the user comes to the website
                //and is not logged in 
                //or when a user is signed out

                //in any case, we need to clear out our session

                return View("");
            }
        }
    }
}
