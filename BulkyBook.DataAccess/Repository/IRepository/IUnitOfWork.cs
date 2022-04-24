using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        //here we will create all of the repositories
        //plus global method Save

        ICategoryRepository Category { get; }
        public void Save();
    }
}
