using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ApplicationUserVM
    {
        public ApplicationUser AppUser { get; set; }
        public string? Role { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> RolesList { get; set; }

        public int? CompanyId { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CompanyList { get; set; }
    }
}
