using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class AuthorProduct
    {
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
