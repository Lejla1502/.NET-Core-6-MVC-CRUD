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
            IEnumerable<ShoppingCart> cartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            OrderTotal = 0;

            foreach(var item in ShoppingCartVM.ListCart)
            {
                item.Price = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                OrderTotal += item.Price * item.Count;
            }

            ShoppingCartVM.CartTotal = OrderTotal;

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            //IEnumerable<ShoppingCart> cartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product");

            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //ShoppingCartVM = new ShoppingCartVM()
            //{
            //    ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
            //};

            //OrderTotal = 0;

            //foreach (var item in ShoppingCartVM.ListCart)
            //{
            //    item.Price = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
            //    OrderTotal += item.Price * item.Count;
            //}

            //ShoppingCartVM.CartTotal = OrderTotal;

            return View();
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
