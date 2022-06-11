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
        IApplicationUserRepository ApplicationUser { get; }
        ICategoryRepository Category { get; }
        ICompanyRepository Company { get; }
        ICoverTypeRepository CoverType { get; }
        IProductRepository Product { get; } 
        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }
        INotificationRepository Notification { get; }
        IReviewRepository Review { get; }
        public void Save();
    }
}
