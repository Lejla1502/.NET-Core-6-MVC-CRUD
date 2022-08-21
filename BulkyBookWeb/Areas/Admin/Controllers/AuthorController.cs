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

        public IActionResult Upsert(int? id)
        {
            Author obj = new Author();
            if (id == 0 | id == null)
            { 
                //ViewBag.CategoryList = CategoryList;
                ////same thing as above, just different approach
                //ViewData["CoverTypeList"] = CoverTypeList;

                return PartialView(obj);
            }
            else
            {
                obj = _unitOfWork.Author.GetFirstOrDefault(x => x.Id == id);
                if (obj == null)
                    return NotFound();

                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Author obj)
        {
            //to check whether the model is valid or not
            //if (ModelState.IsValid)
            //{
                if (obj.Id == 0)
                {
                    _unitOfWork.Author.Add(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Author.Update(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Product edited successfully";
                    return RedirectToAction("Index");
                }

           // }
           // return View(obj);
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            //see if we can add more fileds/properties to Authors
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
            var objFromDB = _unitOfWork.Author.GetFirstOrDefault(x => x.Id == id);
            if (objFromDB == null)
                return Json(new { success = false, message = "Error while deleting" });

            _unitOfWork.Author.Remove(objFromDB);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Successfully deleted company" });
        }

        #endregion
    }
}
