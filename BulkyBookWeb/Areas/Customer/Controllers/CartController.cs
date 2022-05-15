using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using BulkyBookWeb.Customer.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using System.IO;
using System.Text;
using System.Data;
using MimeKit;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _environment = hostingEnvironment;
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


            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += item.Price * item.Count;
            }

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

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += item.Price * item.Count;
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var appUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);

            //preparing OrderHeader object for DB
            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product");

            if (ShoppingCartVM.ListCart.Count() < 1)
                return RedirectToAction("Index", "Home");

            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += item.Price * item.Count;
            }

            if (appUser.CompanyId == 0 || appUser.CompanyId == null)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;
            }
            else
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
            }

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = _unitOfWork.ApplicationUser.GetFirstOrDefault(a => a.Id == claim.Value).Id;


            //pushing OrderHeader object to DB
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            
            //------------>>>>>>>>>
            appUser.Name = ShoppingCartVM.OrderHeader.Name;
            appUser.StreetAddress = ShoppingCartVM.OrderHeader.StreetAddress;
            appUser.City = ShoppingCartVM.OrderHeader.City;
            appUser.State = ShoppingCartVM.OrderHeader.State;
            appUser.PhoneNumber = ShoppingCartVM.OrderHeader.PhoneNumber;
            appUser.PostalCode = ShoppingCartVM.OrderHeader.PostalCode;
            _unitOfWork.ApplicationUser.Update(appUser);
            _unitOfWork.Save();

            //------->>>>>>> is it neccessary to also update data for ApplicationUser if Admin decides 
            //to change it (because like this, parameters like Name, Street, etc. will be "updated" 
            //only for OrderHeader
            //and we will deal with inconsistrency
            //because user will be able to change personal data for every order, but originnal personal
            //data will remain the same


            //preparing OrderDetails object to DB
            foreach (var item in ShoppingCartVM.ListCart)
            {
                OrderDetail od = new OrderDetail()
                {
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    ProductId = item.Product.Id,
                    Count = item.Count,
                    Price = item.Price
                };

                //pushing OrderDetail object to DB
                _unitOfWork.OrderDetail.Add(od);
                _unitOfWork.Save();
            }

            if (appUser.CompanyId == 0 || appUser.CompanyId == null)
            {
                //stripe settings
                var domain = "https://localhost:44311/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                };

                foreach (var item in ShoppingCartVM.ListCart)
                {
                    options.LineItems.Add(
                     new SessionLineItemOptions
                     {
                         PriceData = new SessionLineItemPriceDataOptions
                         {
                             UnitAmount = (long)(item.Price * 100), //20.00 - > 2000,
                             Currency = "usd",
                             ProductData = new SessionLineItemPriceDataProductDataOptions
                             {
                                 Name = item.Product.Title
                             },

                         },
                         Quantity = item.Count
                     }

                    );
                }

                var service = new SessionService();
                Session session = service.Create(options);  //creating a session for Stripe payment

                //when the session is created, we have sessionId and PaymentIntentId, so we update
                //OrderHeader based on those parameters
                //so that in OrderConfirmation we retrieve them and check if payment was successful
                _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);   //redirecting to Stripe portal
            }
            else
                return RedirectToAction("OrderConfirmation", new { id = ShoppingCartVM.OrderHeader.Id });

        }

        public IActionResult OrderConfirmation(int id)
        {
            //to check whether order was successful, we need to retrieve OrderHeader
            //more precisely sessionId and paymentIntentId

            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");

            if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                //check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                    _unitOfWork.Save(); 
                }
            }

            var orderDetailsList = _unitOfWork.OrderDetail.GetAll(u=>u.OrderId==orderHeader.Id, includeProperties:"Product");

            //var webRoot = _environment.WebRootPath;

            //var pathToFile = _environment.WebRootPath
            //                + Path.DirectorySeparatorChar.ToString()
            //                + "Templates"
            //                + Path.DirectorySeparatorChar.ToString()
            //                + "EmailTemplate"
            //                + Path.DirectorySeparatorChar.ToString()
            //                + "Confirm_Order.html";

            //var builder = new BodyBuilder();
            //using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
            //{

            //    builder.HtmlBody = SourceReader.ReadToEnd();

            //}

            //var mailBodyTemplate = System.IO.File.ReadAllText("wwwroot/Templates/EmailTemplate/Confirm_Order.html");
            //var tableRowTemplate = System.IO.File.ReadAllText("wwwroot/Templates/EmailTemplate/Order_Confirm_TableRow.html");

            //var tableRows = new StringBuilder();
            //var totalPrice = 0;
            //foreach (DataRow Row in Tables[0].Rows)
            //{
            //    totalPrice += Convert.ToInt32(Row["Price"]);
            //    tableRows.AppendFormat(tableRowTemplate, Row["Name"], Row["UOM"], Row["Quantity"], Row["UnitPrice"], Row["Price"]);
            //}
            //var mailBody = string.Format(mailBodyTemplate, tableRows.ToString(), totalPrice);
            //// Send your mail body
            ///

            var totalPrice = 0;

            string emailBody = @"<div class=""container"">
    <div class=""row"">
        <h1 class=""text-align"" style=""padding-bottom:20px;""><i class=""bi bi-check2-square""></i></h1>
        <h1>Thank You For Your Order!</h1>
        <p>You've successfully completed your order. Thank you for shopping at our store. Below
            are detailed information about the order you've purchased':
        </p>

    </div>
    <div class=""row"">
        <table class=""table table-responsive"">
            <thead>
                <tr>
                    <th>
                        Order Confirmation #
                    </th>
                    <th>" + orderHeader.Id + @"</th>
                </tr>
            </thead>
            <tbody>
                ";
            foreach (var item in orderDetailsList)
                    {
                        emailBody += @"<tr>";
                        emailBody += @"<td> " + item.Product.Title + "(  " + item.Count + @" )  </td >" ;

                        emailBody += @"<td>" + item.Price.ToString("c") + @"</td>";
                        emailBody += @"</tr>";
                         
                    }
            emailBody += @"
            </tbody>
            <tfoot>
                <tr>
                    <td>TOTAL</td>
                    <td>"+(orderHeader.OrderTotal).ToString("c");
            emailBody+= @"</td>
                </tr>
            </tfoot>
        </table>
    </div>
    <div class=""row"">
         <table class=""table table-responsive"">
            <thead>
                <tr>
                    <th>
                        Delivery Address
                    </th>
                    <th>
                        Estimated Delivery Date
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>"+orderHeader.StreetAddress + @"</td>
                    <td>" + orderHeader.ShippingDate + @"</td>
                </tr>
                 <tr>
                    <td>" + orderHeader.City + " , " + orderHeader.State + @"</td>
                    <td></td>
                </tr>
                <tr>
                    <td>" + orderHeader.PostalCode + @"</td>
                    <td></td>
                </tr>
            </tbody>
        </table>
    </div>
</div>";
            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book", emailBody);

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            HttpContext.Session.SetInt32(StaticDetails.SessionCart, 0);

            return View(id);
        }

        public IActionResult IncrementQuantity(int id)
        {
            ShoppingCart sc = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == id);
            _unitOfWork.ShoppingCart.IncrementCount(sc, 1);
            _unitOfWork.Save();

            HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == sc.ApplicationUserId).ToList().Count);


            return RedirectToAction(nameof(Index));
        }

        public IActionResult DecrementQuantity(int id)
        {
            ShoppingCart sc = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == id);
            if (sc.Count > 1)
            {
                _unitOfWork.ShoppingCart.DecrementCount(sc, 1);
                _unitOfWork.Save();

                HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == sc.ApplicationUserId).ToList().Count);

            }
            else
            {
                _unitOfWork.ShoppingCart.Remove(sc);
                _unitOfWork.Save();

                HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == sc.ApplicationUserId).ToList().Count);

            }


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            ShoppingCart sc = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == id);
            _unitOfWork.ShoppingCart.Remove(sc);
            _unitOfWork.Save();

            HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == sc.ApplicationUserId).ToList().Count);


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
