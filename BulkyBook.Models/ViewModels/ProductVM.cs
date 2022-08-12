using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public  class ProductVM
    {
        public Product Product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        [ValidateNever]
         
        public IEnumerable<SelectListItem> CoverTypeList { get; set; }
      
        [Required]
        public int AuthorId { get; set; }
        public SelectList AuthorList { get; set; }
        public int Author2Id { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Author2List { get; set; }
    }
}
