﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public  class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [RegularExpression(@"^(?:ISBN(?:-13)?:?\ )?(?=[0-9]{13}$|(?=(?:[0-9]+[-\ ]){4})[-\ 0-9]{17}$)97[89][-\ ]?[0-9]{1,5}[-\ ]?[0-9]+[-\ ]?[0-9]+[-\ ]?[0-9]$", ErrorMessage = "Acceptable format: 978-0-306-40615-6 or 978 0 306 40615 6")]
        [Remote(action: "VerifyISBN", controller: "Product", AdditionalFields = nameof(Id))]
        public string ISBN { get; set; }
        
        public string Author { get; set; }

        public bool Featured { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name = "List Price")]

        public double ListPrice { get; set; }
        [Required]
        [Range(1, 10000)]
        [Display(Name = "Price for 1-50")]

        public double Price { get; set; }  //price if a person wants to buy just one product
        [Required]
        [Range(1, 10000)]
        [Display(Name = "Price for 51-100")]

        public double Price50 { get; set; } //special pricing if a person is buying 50 books
        [Required]
        [Range(1, 10000)]
        [Display(Name = "Price for 100+")]

        public double Price100 { get; set; } //special pricing if a person is buying 100 books
        [ValidateNever]
        public string ImageUrl { get; set; }
        [Required]
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        [Required]
        [Display(Name = "Cover Type")]

        public int CoverTypeId { get; set; }
        [ValidateNever]
        public CoverType CoverType { get; set; }

        public bool IsFavourite { get; set; }

        public DateTime CreatedAt { get; set; }

        [ValidateNever]
        public List<Review> Reviews { get; set; }

        public List<AuthorProduct> AuthorProducts { get; set; }

    }
}
