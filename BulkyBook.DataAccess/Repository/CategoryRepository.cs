using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db):base(db)
        {
            _db= db;
        }
        

        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }

        public List<Category> GetPopularCategories()
        {
            var something = _db.OrderDetails.GroupBy(x => x.ProductId).Select(s => new { Bestseller_PrdouctID = s.Key, Count = s.Count() }).OrderByDescending(y => y.Count);

            List<Product> bestsellers = new();
            List<Category> popularCategories = new();

            foreach (var p in something)
            {
                bestsellers.Add((Product)_db.Products.Include(p => p.Category).Where(x => x.Id == p.Bestseller_PrdouctID));
            }

            if (bestsellers.Count() < 1)
            {
                return null;
            }

            foreach(var c in bestsellers)
            {
                popularCategories.Add((Category)_db.Categories.Where(x => x.Id == c.CategoryId));
            }
            

            return popularCategories;
        }
    }
}
