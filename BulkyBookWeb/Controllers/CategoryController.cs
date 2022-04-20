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
            if (ModelState.IsValid)
            {
                _ctx.Categories.Add(cat);
                _ctx.SaveChanges();

                return RedirectToAction("Index");
            }
            else
                return View("Create",cat);
        }
    }
}
