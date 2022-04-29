using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyBookWeb.Customer.Controllers
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> listOfProducts = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            
            return View(listOfProducts);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart cartObj = new ShoppingCart
            {
                Product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id, includeProperties: "Category,CoverType"),
                Count = 1
            };

            return View(cartObj);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}