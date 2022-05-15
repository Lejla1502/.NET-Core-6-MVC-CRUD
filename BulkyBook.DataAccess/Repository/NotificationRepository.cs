using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class NotificationRepository:Repository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _db;
        public NotificationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //this will probably need fixing later
        public void Add(Notification notification, string userId)
        {
            _db.Notifications.Add(notification);    
            _db.SaveChanges();

            var userNotification = new NotificationApplicationUser();
            userNotification.Date = DateTime.Now;
            userNotification.ApplicationUserId = userId;
            userNotification.NotificationId= notification.Id;

            _db.NotificationApplicationUsers.Add(userNotification);
            _db.SaveChanges();
        }

        public void Update(Notification notification)
        {
            _db.Notifications.Update(notification);
        }

        public void ReadNotification(int id)
        {
            var notificationFromDb = _db.Notifications.FirstOrDefault(x => x.Id == id);
            notificationFromDb.IsRead = true;
        }
    }
}
