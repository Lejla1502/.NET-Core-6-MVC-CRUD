using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public CategoryController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> cat = _ctx.Categories;

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
                _ctx.Categories.Add(cat);
                _ctx.SaveChanges();
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
            Category cat = _ctx.Categories.Find(id);
            //category cat = _ctx.Categories.FirstOrDefault(u=>u.Id==id);
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
                _ctx.Categories.Update(cat);
                _ctx.SaveChanges();
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

            Category obj = _ctx.Categories.Find(id);

            if (obj == null)
                return NotFound();

            _ctx.Categories.Remove(_ctx.Categories.Find(id));
            _ctx.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
