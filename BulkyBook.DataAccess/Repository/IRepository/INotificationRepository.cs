using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface INotificationRepository:IRepository<Notification>
    {
        void Add(Notification notification, string userId);
        void Update(Notification notification);
        void ReadNotification(int id);
        public List<NotificationApplicationUser> GetUserNotifications();

    }
}
