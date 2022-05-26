using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Product obj)
        {
            //here we use a better practice
            //instead of working directly with DB model, we place it in variable and then update
            //properties we need
            //this way we can restrict Update

            var objFromDb = _db.Products.FirstOrDefault(x=>x.Id== obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title= obj.Title;
                objFromDb.Description= obj.Description;
                objFromDb.ISBN= obj.ISBN;
                objFromDb.Author= obj.Author;
                objFromDb.ListPrice= obj.ListPrice;
                objFromDb.Price= obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.CategoryId= obj.CategoryId;
                objFromDb.CoverTypeId= obj.CoverTypeId;

                if (obj.ImageUrl != null)
                    objFromDb.ImageUrl = obj.ImageUrl;
            }
        }

        public void UpdateStatus(int id)
        {
            
            var productFromDb = _db.Products.FirstOrDefault(x => x.Id == id);
           
            if (productFromDb != null)
            {
                if (productFromDb.IsFavourite)
                    productFromDb.IsFavourite = false;
                else
                    productFromDb.IsFavourite = true;
                
            }
            //_db.OrderHeaders.Update(orderFromDb);
            
        }
    }
}
