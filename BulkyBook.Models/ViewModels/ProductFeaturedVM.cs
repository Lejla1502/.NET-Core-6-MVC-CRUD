using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ProductFeaturedVM
    {
        public int ProductId { get; set; }
        public IEnumerable<SelectListItem> Products { get; set; }
    }
}
