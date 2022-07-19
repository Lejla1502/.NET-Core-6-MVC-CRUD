using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
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
            IEnumerable<Author> authors = _unitOfWork.Author.GetAll();

            return View(authors);
        }



        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var authorsList = _unitOfWork.Author.GetAll();
            

            return Json(new { data = authorsList });
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
