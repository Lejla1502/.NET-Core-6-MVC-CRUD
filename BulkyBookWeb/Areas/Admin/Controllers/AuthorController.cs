using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class AuthorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuthorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //IEnumerable<Author> authors = _unitOfWork.Author.GetAll();

            return View();
        }



        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var authorsList = new AuthorVM { 
                Authors=_unitOfWork.Author.GetAll().Select(x=>new AuthorVM.AuthorInfo
                {
                    Id=x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    NumOfBooks =2
                }).ToList()
            };

            return Json(new { data = authorsList.Authors });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var objFromDB = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
            if (objFromDB == null)
                return Json(new { success = false, message = "Error while deleting" });

            _unitOfWork.Company.Remove(objFromDB);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Successfully deleted company" });
        }

        #endregion
    }
}
