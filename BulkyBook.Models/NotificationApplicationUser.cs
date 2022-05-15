using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public  class NotificationApplicationUser
    {
        public DateTime Date { get; set; }

        public int NotificationId { get; set; }
        public Notification Notification { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
