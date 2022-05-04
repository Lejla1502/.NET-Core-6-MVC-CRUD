using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public double OrderTotal { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };

            OrderTotal = 0;

            foreach(var item in ShoppingCartVM.ListCart)
            {
                item.Price = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                OrderTotal += item.Price * item.Count;
            }

            ShoppingCartVM.OrderHeader.OrderTotal = OrderTotal;

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(a => a.Id == claim.Value);

            ShoppingCartVM.OrderHeader.Name=ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber=ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress=ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City=ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State=ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode=ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            OrderTotal = 0;

            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                OrderTotal += item.Price * item.Count;
            }

            ShoppingCartVM.OrderHeader.OrderTotal = OrderTotal;

            return View(ShoppingCartVM);
        }


        public IActionResult IncrementQuantity(int id)
        {
            ShoppingCart sc = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == id);
            _unitOfWork.ShoppingCart.IncrementCount(sc, 1);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DecrementQuantity(int id)
        {
            ShoppingCart sc = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == id);
            if (sc.Count > 1)
            {
                _unitOfWork.ShoppingCart.DecrementCount(sc, 1);
            }
            else
            {
                _unitOfWork.ShoppingCart.Remove(sc);

            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            ShoppingCart sc = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == id);
            _unitOfWork.ShoppingCart.Remove(sc);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        private double GetPriceByQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
                return price;
            else
            {
                if (quantity <= 100)
                    return price50;
                return price100;
            }
        }
    }
}
