using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ProductHomePageVM
    {
        public List<Product> Bestsellers { get; set; }
        public List<Product> NewBooks { get; set; }
    }
}
