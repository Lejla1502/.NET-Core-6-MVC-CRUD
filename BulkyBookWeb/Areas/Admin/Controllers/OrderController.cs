using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeader>  orderList = _unitOfWork.OrderHeader.GetAll(includeProperties:"ApplicationUser");

            

            return Json(new { data = orderList });
        }
        #endregion
    }
}
