using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _ctx;
        public CategoryController(ICategoryRepository ctx)
        {
            _ctx = ctx;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> cat = _ctx.GetAll();

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
                _ctx.Add(cat);
                _ctx.Save();
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
            Category cat = _ctx.GetFirstOrDefault(u=>u.Id==id);
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
                _ctx.Update(cat);
                _ctx.Save();
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

            Category obj = _ctx.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
                return NotFound();

            _ctx.Remove(_ctx.GetFirstOrDefault(u => u.Id == id));
            _ctx.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
