using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ProductReviewVM
    {

        //public Product Product { get; set; }
        //public int Rating { get; set; }

        public IEnumerable<ProductInfo> Products { get; set; }
        public List<Product> Bestsellers { get; set; }
        public List<Category> PopularCategories { get; set; }
        public List<Product> NewBooks { get; set; }

        public Product FeaturedProduct { get; set; }

        public class ProductInfo
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string ISBN { get; set; }
            public string Author { get; set; }

            public double ListPrice { get; set; }

            public double Price { get; set; }  //price if a person wants to buy just one product


            public double Price50 { get; set; } //special pricing if a person is buying 50 books


            public double Price100 { get; set; } //special pricing if a person is buying 100 books

            public bool IsFavourite { get; set; }
            public string ImageUrl { get; set; }

            public DateTime CreatedAt { get; set; }

            public int AvgRating { get; set; }
        }

        public int CategoryId { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public int AuthorId { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> AuthorList { get; set; }

        public string BookTitle { get; set; }
        //public string AuthorName { get; set; }
    }
}
