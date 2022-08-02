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

        public List<Category> GetFourPopularCategories()
        {
            var something = _db.OrderDetails.GroupBy(x => x.ProductId).Select(s => new { Bestseller_PrdouctID = s.Key, Count = s.Count() }).OrderByDescending(y => y.Count).ToList();

            List<Product> bestsellers = new();
            List<Category> popularCategories = new();

            foreach (var p in something)
            {
                bestsellers.Add(_db.Products.Find(p.Bestseller_PrdouctID));
            }

            if (bestsellers.Count() < 1)
            {
                return null;
            }

            //join bestseller products with thir categories and return the categories
            var mix = bestsellers.Join(_db.Categories,
                pr => pr.CategoryId,
                cat => cat.Id,
                (x, y) => new { Proizvod = x, Category = y }).GroupBy(x => new { x.Category.Name }).Select(s => new { Key= s.Key, Value=s.Count() }).ToList();

            //

            foreach(var c in mix)
            {
                Category category = _db.Categories.FirstOrDefault(cat => cat.Name == c.Key.Name);
                popularCategories.Add(category);
            }
            
            //solve it through join? on the tables of Products and Categories
            //and then to combine them into one table - one parameter category, other- count

            //group categories by a product and return top for which contain the highest number of (sold)
            //products in it
            return popularCategories.Take(4).ToList();
        }
    }
}
