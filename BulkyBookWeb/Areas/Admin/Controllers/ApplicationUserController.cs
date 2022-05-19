using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]

    public class ApplicationUserController : Controller
    {
        private UserManager<IdentityUser> _usrManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IUnitOfWork _unitOfWork;
        public ApplicationUserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> usrManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _usrManager = usrManager;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll();
            return View(users);
        }

        public async Task<IActionResult> Update(string userId)
        {
            var appUserFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == userId, includeProperties: "Company");
            var user = await _usrManager.FindByIdAsync(userId);
            string rolename =  _usrManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

            int companyID = 0;
            if (rolename == "Company")
            {
                companyID = (int)appUserFromDb.CompanyId;
            }
            ApplicationUserVM appUserVM = new ApplicationUserVM
            {
                AppUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == userId),
                Role = rolename,
                RolesList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
                CompanyId = companyID==null?null:companyID,
                CompanyList = _unitOfWork.Company.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            return View(appUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ApplicationUserVM appUserVM)
        {
            if (appUserVM.Role != "Company")
                appUserVM.CompanyId = null;

            var appUserFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == appUserVM.AppUser.Id);

            appUserFromDb.PhoneNumber = appUserVM.AppUser.PhoneNumber;
            appUserFromDb.StreetAddress = appUserVM.AppUser.StreetAddress;
            appUserFromDb.City = appUserVM.AppUser.City;
            appUserFromDb.State = appUserVM.AppUser.State;
            appUserFromDb.CompanyId = appUserVM.CompanyId;
            
            //how to change the role??????????

            _unitOfWork.ApplicationUser.Update(appUserFromDb);

            _unitOfWork.Save();

            return RedirectToAction("Update", new { userId=appUserVM.AppUser.Id });
        }
        
    }
}
