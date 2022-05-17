using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using BulkyBookWeb.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]

    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IHubContext<NotificationHub> _hubContext;
        public NotificationController(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            //var notificationList = _unitOfWork.Notification.GetAll();

            //var notificationVM = new NotificationVM
            //{
            //    Notification = notificationList,
            //    Count = notificationList.Count()
            //};

            return View();
        }

        public IActionResult GetNotification()
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //var userId=_unitOfWork.ApplicationUser.GetFirstOrDefault(u=>u.Id== claim.Value);

            var notification = _unitOfWork.Notification.GetUserNotifications();
            return Ok(new {NotificationApplicationUser=notification, Count=notification.Count });
        }

        public IActionResult ReadNotification(int notificationId)
        {
            _unitOfWork.Notification.ReadNotification(notificationId);
            _unitOfWork.Save();
            _hubContext.Clients.All.SendAsync("displayNotification", "");

            return Ok();
        }
    }
}
