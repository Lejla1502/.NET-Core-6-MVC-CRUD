using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
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
            IEnumerable<OrderHeader> orders = _unitOfWork.OrderHeader.GetAll();
            return View(orders);
        }

        #region API CALLS
        public IActionResult GetAll()
        {
            var orderList = _unitOfWork.OrderHeader.GetAll();
            return Json(new { data = orderList });
        }
        #endregion
    }
}
