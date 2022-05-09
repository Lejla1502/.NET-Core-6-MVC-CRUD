using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderId == orderId, includeProperties: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles =StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked:false);

            orderHeader.Name = OrderVM.OrderHeader.Name;
            orderHeader.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeader.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeader.City = OrderVM.OrderHeader.City;
            orderHeader.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeader.State = OrderVM.OrderHeader.State;

            if(OrderVM.OrderHeader.Carrier!=null)
            {
                orderHeader.Carrier=OrderVM.OrderHeader.Carrier;
            }
            if(OrderVM.OrderHeader.TrackingNumber!=null)
                orderHeader.TrackingNumber=OrderVM.OrderHeader.TrackingNumber;

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully";

            return RedirectToAction("Details", "Order", new {orderId=orderHeader.Id});
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {

            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, StaticDetails.StatusInProcess);
            _unitOfWork.Save();

            TempData["Success"] = "Order Status Updated Successfully";

            return RedirectToAction("Details", "Order", new {orderId=OrderVM.OrderHeader.Id});
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked: false);

            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = StaticDetails.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();

            TempData["Success"] = "Order Status Updated Successfully";

            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked: false);

            if (OrderVM.OrderHeader.PaymentStatus==StaticDetails.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason =RefundReasons.RequestedByCustomer,
                    PaymentIntent=orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);
            }

            _unitOfWork.Save();

            TempData["Success"] = "Order Cancelled Successfully";

            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderList;

            if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
            {
                orderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                orderList = _unitOfWork.OrderHeader.GetAll(a => a.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }

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
