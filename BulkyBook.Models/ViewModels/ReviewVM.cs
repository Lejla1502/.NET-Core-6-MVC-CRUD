using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ReviewVM
    {
        public IEnumerable<Review> Reviews { get; set; }
        public string Title { get; set; }
    }
}
