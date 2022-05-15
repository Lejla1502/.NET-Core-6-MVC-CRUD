using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class NotificationVM
    {
        public IEnumerable<Notification> Notification { get; set; } 
        public int Count { get; set; }
    }
}
