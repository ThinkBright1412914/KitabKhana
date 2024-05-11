using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model;
using KitabKhana.Model.ViewModel;
using KitabKhana.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace KitabKhana.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = RoleDefine.Role_Admin + "," + RoleDefine.Role_Company_User + "," + RoleDefine.Role_User_Only)]
    public class OrderController : Controller
    {
        private readonly iUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderViewModel orderVM { get; set; }
        public OrderController(iUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult OrderList()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            orderVM = new()
            {
                orderHeader = _unitOfWork.OrderHeader.GetById(x => x.Id == orderId, includeProperties: "ApplicationUser"),
                orderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderId == orderId, includeProperties: "Product"),
            };
            
            return View(orderVM);
        }


        [HttpPost]
        [Authorize(Roles = RoleDefine.Role_Admin + "," + RoleDefine.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {

            var orderHeaderDb = _unitOfWork.OrderHeader.GetById(x => x.Id == orderVM.orderHeader.Id, tracked:false);
            orderHeaderDb.Name = orderVM.orderHeader.Name;
            orderHeaderDb.PhoneNo = orderVM.orderHeader.PhoneNo;
            orderHeaderDb.Address = orderVM.orderHeader.Address;
            orderHeaderDb.City = orderVM.orderHeader.City;
            orderHeaderDb.PostalCode = orderVM.orderHeader.PostalCode;  
            orderHeaderDb.State = orderVM.orderHeader.State;

            if(orderVM.orderHeader.Carrier != null)
            {
                orderHeaderDb.Carrier = orderVM.orderHeader.Carrier;
            }

            if(orderVM.orderHeader.TrackingNUmber != null)
            {
                orderHeaderDb.TrackingNUmber = orderVM.orderHeader.TrackingNUmber;
            }

            _unitOfWork.OrderHeader.Update(orderHeaderDb);
            _unitOfWork.Save();
            TempData["success"] = "Updated Order Details Successfully";
            return RedirectToAction("Details","Order", new {orderId = orderHeaderDb.Id});
        }


        [HttpPost]
        [Authorize(Roles = RoleDefine.Role_Admin + "," + RoleDefine.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderVM.orderHeader.Id, RoleDefine.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Updated Order Details Successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderVM.orderHeader.Id });
        }


        [HttpPost]
        [Authorize(Roles = RoleDefine.Role_Admin + "," + RoleDefine.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder()
        {
            var orderHeaderDb = _unitOfWork.OrderHeader.GetById(x => x.Id == orderVM.orderHeader.Id, tracked: false);

            orderHeaderDb.TrackingNUmber = orderVM.orderHeader.TrackingNUmber;
            orderHeaderDb.Carrier = orderVM.orderHeader.Carrier;
            orderHeaderDb.OrderStatus = RoleDefine.StatusShipping;
            orderHeaderDb.ShippingDate = DateTime.Now;
            if(orderHeaderDb.PaymentStatus == RoleDefine.PaymentStatusDelayedPayment)
            {
                orderHeaderDb.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.OrderHeader.Update(orderHeaderDb);
            _unitOfWork.Save();
            TempData["success"] = "Order Shipped Successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderVM.orderHeader.Id });
        }


        [ActionName("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details_Pay_Now()
        {
            orderVM.orderHeader = _unitOfWork.OrderHeader.GetById(x => x.Id == orderVM.orderHeader.Id, includeProperties: "ApplicationUser");
            orderVM.orderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderId == orderVM.orderHeader.Id, includeProperties: "Product");

            var domain = "https://localhost:7054/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },

                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Admin/Order/PaymentConfirmation?orderHeaderId={orderVM.orderHeader.Id}",
                CancelUrl = domain + $"Admin/Order/Details?orderId={orderVM.orderHeader.Id}",
            };

            foreach (var item in orderVM.orderDetail)
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
            _unitOfWork.OrderHeader.UpdateStripePaymentStatus(orderVM.orderHeader.Id, session.Id);

            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetById(x => x.Id == orderHeaderId);

            if (orderHeader.PaymentStatus == RoleDefine.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    orderHeader.PaymentIntentId = session.PaymentIntentId;
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, RoleDefine.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

            List<CartViewModel> shopingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.DeleteRange(shopingCarts);
            _unitOfWork.Save();
            return View(orderHeaderId);
        }


        [HttpPost]
        [Authorize(Roles = RoleDefine.Role_Admin + "," + RoleDefine.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            var orderHeaderDb = _unitOfWork.OrderHeader.GetById(x => x.Id == orderVM.orderHeader.Id, tracked: false);
            if (orderHeaderDb.PaymentStatus == RoleDefine.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,

                    PaymentIntent = orderHeaderDb.PaymentIntentId,

                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderDb.Id, RoleDefine.StatusCancel, RoleDefine.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderDb.Id, RoleDefine.StatusCancel, RoleDefine.StatusCancel);
            }

            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderVM.orderHeader.Id });
        }



        #region For API
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeader;

            if (User.IsInRole(RoleDefine.Role_Admin) || User.IsInRole(RoleDefine.Role_Employee))
            {
                 orderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeader = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value,includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "inprocess":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == RoleDefine.StatusInProcess);
                    break;
                case "pending":
                    orderHeader = orderHeader.Where(x =>x.OrderStatus == RoleDefine.StatusPending);
                    break;
                case "completed":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == RoleDefine.StatusShipping);
                    break;
                case "approved":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == RoleDefine.StatusApproved);
                    break;
                default:                   
                    break;
            }
            return Json(new { Data = orderHeader });
        }

        #endregion
    }
}
