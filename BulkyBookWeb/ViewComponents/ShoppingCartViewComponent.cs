using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.ViewComponents
{
    public class ShoppingCartViewComponent:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
                _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //if a user is logged in, get their session
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim!=null)
            {
                //if session is null only then we want to go to the db
                if(HttpContext.Session.GetInt32(StaticDetails.SessionCart)!=null)
                {
                    return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart));
                }
                else
                {
                    HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId==claim.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart));
                }
            }
            else
            {
                //this can happen in two instances - it can happen if the user comes to the website
                //and is not logged in 
                //or when a user is signed out

                //in any case, we need to clear out our session
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
