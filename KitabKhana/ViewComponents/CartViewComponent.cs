
using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KitabKhana.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly iUnitOfWork _unitOfWork;

        public CartViewComponent(iUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(RoleDefine.SessionCart) != null)
                {
                    return View(HttpContext.Session.GetInt32(RoleDefine.SessionCart));
                }
                else
                {
                    HttpContext.Session.SetInt32(RoleDefine.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32(RoleDefine.SessionCart));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }

    }
}
