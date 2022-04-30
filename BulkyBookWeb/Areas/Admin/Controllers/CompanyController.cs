using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //IEnumerable<Company> obj = _unitOfWork.Company.GetAll();

            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company obj = new Company();
            if (id == 0 | id == null)
            {
                //ViewBag.CategoryList = CategoryList;
                ////same thing as above, just different approach
                //ViewData["CoverTypeList"] = CoverTypeList;

                return View(obj);
            }
            else
            {
                obj = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
                if (obj == null)
                    return NotFound();

                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
               
                if (obj.Id == 0)
                {
                    _unitOfWork.Company.Add(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Product edited successfully";
                    return RedirectToAction("Index");
                }

            }
            return View(obj);
        }




        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var companiesList = _unitOfWork.Company.GetAll();
            return Json(new { data = companiesList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var objFromDB=_unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
            if (objFromDB == null)
                return Json(new { success=false, message = "Error while deleting" });
            
            _unitOfWork.Company.Remove(objFromDB);
            _unitOfWork.Save();

            return Json(new { success = true, message="Successfully deleted company" });
        }

        #endregion
    }
}
