
using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model;
using KitabKhana.Model.Base;
using KitabKhana.Model.ViewModel;
using KitabKhana.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace KitabKhana.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleDefine.Role_Admin + "," + RoleDefine.Role_Company_User)]
    public class ProductController : Controller
    {
        private readonly iUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(iUnitOfWork iUnitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = iUnitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {

            return View();
        }


        public IActionResult Upsert(int? id)
        {
            ProductViewModel ProductVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                }),
                CoverList = _unitOfWork.CoverType.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                }),
            };


            //Hard Code for test purpose
            ProductVM.GenreList = GetGenre();

            //ViewData["Category"] = CategoryList;
            //ViewBag.CategoryList = CategoryList;    
            //ViewBag.CoverType = CoverType;

            if (id == 0 || id == null)
            {
                return View(ProductVM);
            }
            else
            {
                ProductVM.Product = _unitOfWork.Product.GetById(x => x.Id == id);
                return View(ProductVM);
            }
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel model, IFormFile? imgFile)
        {        
            
            var filePath = _webHostEnvironment.WebRootPath;
            model.GenreList = GetGenre();
             
            if (ModelState.IsValid)
            {
                if (imgFile != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(filePath, @"Image\Product");
                    var extension = Path.GetExtension(imgFile.FileName);

                    if (model.Product.ImageUrl != null)
                    {
                        var oldImgPath = Path.Combine(filePath, model.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        imgFile.CopyTo(fileStream);
                    }
                    model.Product.ImageUrl = @"\Image\Product\" + fileName + extension;
                }

                if (model.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(model.Product);
                    TempData["success"] = "Product Created Succesfully";
					_unitOfWork.Save();
				}
                else
                {
                    _unitOfWork.Product.Update(model.Product);
                    TempData["success"] = "Product Edited Succesfully";
					_unitOfWork.Save();
				}
              
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }

        }

        //Hard Code for test purpose
        public List<SelectListItem> GetGenre()
        {
            List<SelectListItem > list = new List<SelectListItem>();    
            try
            {
                IEnumerable<SelectListItem> genres = from GenreEnum g in Enum.GetValues(typeof(GenreEnum))
                                                 select new SelectListItem { Text = g.ToString(), Value = g.ToString() };  
                return genres.ToList();
            }
            catch (Exception ex)
            {

            }
            return list;
        }

        #region For API
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json (new {Data = result});
        }



        [HttpDelete]
        public IActionResult DeletePost(int id)
        {
            var model = _unitOfWork.Product.GetById(x => x.Id == id);
            if (model != null)
            {
                var oldImgPath = Path.Combine(_webHostEnvironment.WebRootPath, model.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }

                _unitOfWork.Product.Delete(model);
                _unitOfWork.Save();

                return Json(new { success = true, message = "Sucessfully Deleted" });          
            }
            return Json(new { success = false, message = $"The current {id} was not found" });


        }

        #endregion


    }
}
