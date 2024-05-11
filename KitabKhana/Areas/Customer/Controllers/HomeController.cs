
using KitabKhana.Data.Repository;
using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model;
using KitabKhana.Model.ViewModel;
using KitabKhana.Models;
using KitabKhana.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace KitabKhana.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly iUnitOfWork _unitOfWork;
        private readonly ProductConverter _converter;
       

        public HomeController(ILogger<HomeController> logger , iUnitOfWork unitOfWork , ProductConverter converter)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _converter = converter;
        }
    
        public IActionResult Index(string status)
        {
            List<ProductViewModel> listVM = new List<ProductViewModel>();
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
           
            if (status != null)
            {
                switch (status)
                {
                    case "All":
                        productList = productList.ToList();
                        break;

                    case "LowToHigh":
                        productList = productList.OrderBy(p => p.Price).ToList();
                        break;

                    case "HighToLow":
                        productList = productList.OrderByDescending(p => p.Price).ToList();
                        break;

                    case "Romance":
                        productList = productList.Where(x => x.CategoryId == 1).ToList();
                        break;

                    case "SciFi":
                        productList = productList.Where(x => x.CategoryId == 2).ToList();
                        break;

                    case "Thriller":
                        productList = productList.Where(x => x.CategoryId == 5).ToList();
                        break;

                    case "Story":
                        productList = productList.Where(x => x.CategoryId == 4).ToList();
                        break;

                    case "Inspirational":
                        productList = productList.Where(x => x.CategoryId == 3).ToList();
                        break;

                    case "War":
                        productList = productList.Where(x => x.CategoryId == 6).ToList();
                        break;

                    case "Novel":
                        productList = productList.Where(x => x.CategoryId == 7).ToList();
                        break;
                }
            }

            foreach(var item in productList)
            {
                var list = _converter.ConvertToModel(item);              
                listVM.Add(list);
            }

            return View(listVM);
        }

        public IActionResult Details(int productId)
        {
            CartViewModel model = new()
            {
                ProductId = productId,
                Count = 1,
                Product = _unitOfWork.Product.GetById(x => x.Id == productId)
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(CartViewModel shoppingCart)
        {
            if(shoppingCart.Count < 1)
            {
                return View(shoppingCart);
            }
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            CartViewModel cartFromDb = _unitOfWork.ShoppingCart.GetById(x => x.ApplicationUserId == claim.Value && x.ProductId == shoppingCart.ProductId);
            
            if(cartFromDb == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(RoleDefine.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
            }
            else
            {
                _unitOfWork.ShoppingCart.CartIncrement(cartFromDb, shoppingCart.Count);
                _unitOfWork.Save();
            }

         

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public JsonResult GetCategory()
        {
            var result = _unitOfWork.Category.GetAll().ToList();
            return Json(result);    
        }
    }
}
