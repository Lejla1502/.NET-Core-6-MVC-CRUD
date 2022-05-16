using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]

    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public NotificationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //GetNotification()
        public IActionResult Index()
        {
            var notificationList = _unitOfWork.Notification.GetAll();

            var notificationVM = new NotificationVM
            {
                Notification = notificationList,
                Count = notificationList.Count()
            };

            return View(notificationVM);
        }

        //public IActionResult GetNotification()
        //{

        //}

        public IActionResult ReadNotification(int notificationId)
        {
            _unitOfWork.Notification.ReadNotification(notificationId);
            _unitOfWork.Save();

            return Ok();
        }
    }
}
