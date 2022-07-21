using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly ILogger<HomeController> _logger;

        public BookController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {

            return View();
        }
    }
}
