using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]

    public class ApplicationUserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApplicationUserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

         public IActionResult Index()
        {
            IEnumerable<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll();
            return View(users);
        }

        public IActionResult RedirectToIdentity(string userId)
        {
            string url = "/Identity/Account/Manage?userId=" + userId;
            return LocalRedirect(url);
        }
    }
}
