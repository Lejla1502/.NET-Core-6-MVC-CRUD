using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll();
            return View(products);
        }


        public IActionResult Upsert(int? id)
        {
            if (id == 0 | id == null)
            {
                IEnumerable<SelectListItem> CategoryList= _unitOfWork.Category.GetAll().Select(
                    u=>new SelectListItem
                    {
                        Text=u.Name,
                        Value=u.Id.ToString()
                    });

                IEnumerable<SelectListItem> CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
                   u => new SelectListItem
                   {
                       Text = u.Name,
                       Value = u.Id.ToString()
                   });

                ViewBag.CategoryList = CategoryList;
                //same thing as above, just different approach
                ViewData["CoverTypeList"] = CoverTypeList;

                return View();
            }
            else
            {
                var objFromDb = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
                if (objFromDb == null)
                    return NotFound();

                return View(objFromDb);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id != 0 | obj.Id != null)
                {
                    _unitOfWork.Product.Update(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Product edited successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Product.Add(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction("Index");
                }
            }
            return View("Edit",obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == 0 | id == null)
                return NotFound();

            var objFromDb = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            if (objFromDb == null)
                return NotFound();

            _unitOfWork.Product.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
