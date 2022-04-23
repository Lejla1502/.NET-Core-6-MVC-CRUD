using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public  interface IRepository<T> where T:class
    {
        //this interface should be generic and should be able to handle all of the classes

        IEnumerable<T> GetAll();

        void Add(T entity);

        T FindById(int id);

    }
}
