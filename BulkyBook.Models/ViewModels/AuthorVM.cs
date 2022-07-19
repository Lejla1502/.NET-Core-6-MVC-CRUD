using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class AuthorVM
    {
        public IEnumerable<AuthorInfo> Authors { get; set; }

        public class AuthorInfo
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string NumOfBooks { get; set; }

        }
    }
}