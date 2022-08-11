using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IProductRepository:IRepository<Product>
    {
        void Update(Product product);
        void UpdateStatus(int id);
        Product[] GetRecommended(string appUserId, int productId);

        List<Product> GetBestsellers();

        List<Product> GetNewBooks();
    }
}
