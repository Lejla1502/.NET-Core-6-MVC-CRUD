using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class AuthorProductRepository : Repository<AuthorProduct>, IAuthorProductRepository
    {
        private readonly ApplicationDbContext _db;
        public AuthorProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(AuthorProduct authorProduct)
        {
            _db.AuthorProducts.Update(authorProduct);
        }
    }
}
