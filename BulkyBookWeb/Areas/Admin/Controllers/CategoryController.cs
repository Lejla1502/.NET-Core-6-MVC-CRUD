using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> cat = _unitOfWork.Category.GetAll();

            return View(cat);
        }

        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(Category cat)
        {
            if (cat.Name == cat.DisplayOrder.ToString())
            { 
                //we're putting name as key beacuse we want to display error
                //for input field "Name" of category
                ModelState.AddModelError("name", "Name and order cannot be the same"); 
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(cat);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            else
                return View("Create",cat);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null | id == 0)
            {
                return NotFound();   
            }
            //Category cat = _ctx.Categories.Find(id);
            Category cat = _unitOfWork.Category.GetFirstOrDefault(u=>u.Id==id);
            //Category cat = _ctx.Categories.SingleOrDefault(u => u.Id == id);

            if (cat == null)
                return NotFound();

            return View(cat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAfterEdit(Category cat)
        {

            if (cat.Name == cat.DisplayOrder.ToString())
            {
                //we're putting name as key beacuse we want to display error
                //for input field "Name" of category
                ModelState.AddModelError("name", "Name and order cannot be the same");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(cat);
                _unitOfWork.Save();
                TempData["success"] = "Category edited successfully";
                return RedirectToAction("Index");
            }
            else
                return View("Edit", cat);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Delete(int? id)
        {

            if (id == null | id == 0)
            {
                return NotFound();
            }

            Category obj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
                return NotFound();

            _unitOfWork.Category.Remove(_unitOfWork.Category.GetFirstOrDefault(u => u.Id == id));
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
