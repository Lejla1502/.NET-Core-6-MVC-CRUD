using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
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
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            switch (status)
            {
                case "pending":
                    orderList = orderList.Where(x=>x.OrderStatus==StaticDetails.StatusPending);
                    break;
                case "inprocess":
                    orderList = orderList.Where(x => x.OrderStatus == StaticDetails.StatusInProcess);
                    break;
                case "completed":
                    orderList = orderList.Where(x => x.OrderStatus == StaticDetails.StatusShipped);
                    break;
                case "approved":
                    orderList = orderList.Where(x => x.OrderStatus == StaticDetails.StatusApproved);
                    break;
                default:
                    break;
            }

            
            return Json(new { data = orderList });
        }
        #endregion
    }
}
