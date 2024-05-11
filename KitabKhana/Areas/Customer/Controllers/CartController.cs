using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model;
using KitabKhana.Model.ViewModel;
using KitabKhana.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace KitabKhana.Areas.Customer.Controllers
{

    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly iUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ShoppingCartViewModel shopingCartVM { get; set; }


        public CartController(iUnitOfWork unitOfWork , IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shopingCartVM = new ShoppingCartViewModel()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };


            foreach (var item in shopingCartVM.ListCart)
            {
                item.Price = PriceBasedOnQuanitity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                shopingCartVM.OrderHeader.OrderTotal += (item.Count * item.Price);

            }

            return View(shopingCartVM);
        }

        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shopingCartVM = new ShoppingCartViewModel()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };

            shopingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetById(x => x.Id == claim.Value);
            shopingCartVM.OrderHeader.Name = shopingCartVM.OrderHeader.ApplicationUser.Name;
            shopingCartVM.OrderHeader.Address = shopingCartVM.OrderHeader.ApplicationUser.Address;
            shopingCartVM.OrderHeader.State = shopingCartVM.OrderHeader.ApplicationUser.State;
            shopingCartVM.OrderHeader.PostalCode = shopingCartVM.OrderHeader.ApplicationUser.PostalCode;
            shopingCartVM.OrderHeader.PhoneNo = shopingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var item in shopingCartVM.ListCart)
            {
                item.Price = PriceBasedOnQuanitity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                shopingCartVM.OrderHeader.OrderTotal += (item.Count * item.Price);

            }
            return View(shopingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shopingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product");

            shopingCartVM.OrderHeader.PaymentStatus = RoleDefine.PaymentStatusPending;
            shopingCartVM.OrderHeader.OrderStatus = RoleDefine.StatusPending;
            shopingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shopingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var item in shopingCartVM.ListCart)
            {
                item.Price = PriceBasedOnQuanitity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                shopingCartVM.OrderHeader.OrderTotal += (item.Count * item.Price);
            }

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetById(x => x.Id == claim.Value);

            if(applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                shopingCartVM.OrderHeader.OrderStatus = RoleDefine.StatusPending;
                shopingCartVM.OrderHeader.PaymentStatus = RoleDefine.PaymentStatusPending;
            }
            else
            {
                shopingCartVM.OrderHeader.OrderStatus = RoleDefine.StatusApproved;
                shopingCartVM.OrderHeader.PaymentStatus = RoleDefine.PaymentStatusDelayedPayment;
            }

            _unitOfWork.OrderHeader.Add(shopingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var item in shopingCartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = item.ProductId,
                    OrderId = shopingCartVM.OrderHeader.Id,
                    Count = item.Count,
                    Price = item.Price,
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }


            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:7054/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                {
                    "card",
                },

                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?Id={shopingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"Customer/Cart/Index",
                };

                foreach (var item in shopingCartVM.ListCart)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title,
                            },
                        },
                        Quantity = item.Count,
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentStatus(shopingCartVM.OrderHeader.Id, session.Id);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            else
            {
                return RedirectToAction("OrderConfirmation", "Cart", new { id = shopingCartVM.OrderHeader.Id });
            }



            //_unitOfWork.ShoppingCart.DeleteRange(shopingCartVM.ListCart);
            //_unitOfWork.Save();
            //return RedirectToAction("Index", "Home");
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetById(x => x.Id == id,includeProperties:"ApplicationUser");

            if (orderHeader.PaymentStatus != RoleDefine.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    orderHeader.PaymentIntentId = session.PaymentIntentId;
                    _unitOfWork.OrderHeader.UpdateStatus(id, RoleDefine.StatusApproved, RoleDefine.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Kitab Khana", "<p> New Order Created </p>");
            List<CartViewModel> shopingCarts = _unitOfWork.ShoppingCart.GetAll(x =>x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.DeleteRange(shopingCarts);
            HttpContext.Session.Clear();
            _unitOfWork.Save();
            return View(id);
        }

        public JsonResult plus(int ids)
        {
            var cartId = _unitOfWork.ShoppingCart.GetById(x => x.ProductId == ids);
            _unitOfWork.ShoppingCart.CartIncrement(cartId, 1);
            _unitOfWork.Save();
            return Json("Ok");  
        }

        public IActionResult minus(int id)
        {
            var cartId = _unitOfWork.ShoppingCart.GetById(x => x.ProductId == id);
            if(cartId.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Delete(cartId);
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartId.ApplicationUserId).ToList().Count -1;
                HttpContext.Session.SetInt32(RoleDefine.SessionCart,count);
            }
            _unitOfWork.ShoppingCart.CartDecrement(cartId, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult remove(int id)
        {
            var cartId = _unitOfWork.ShoppingCart.GetById(x => x.ProductId == id);          
            _unitOfWork.ShoppingCart.Delete(cartId);
            _unitOfWork.Save();
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartId.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(RoleDefine.SessionCart, count);
            return RedirectToAction(nameof(Index));
        }


        private double PriceBasedOnQuanitity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else
            {
                if (quantity <= 100)
                {
                    return price50;
                }
                return price100;
            }

        }
    }
}
