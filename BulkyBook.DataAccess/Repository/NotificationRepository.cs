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
