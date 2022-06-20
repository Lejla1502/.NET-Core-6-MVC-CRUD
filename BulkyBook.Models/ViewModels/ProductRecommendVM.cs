using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ProductRecommendVM
    {
        public ProductInfo[] Products { get; set; }

        public class ProductInfo
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Author { get; set; } //special pricing if a person is buying 50 books
            public string ImageUrl { get; set; }
            public double Price { get; set; }
            public int AvgRating { get; set; }
        }
    }
}
