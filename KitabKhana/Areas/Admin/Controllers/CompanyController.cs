
using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model;
using KitabKhana.Model.ViewModel;
using KitabKhana.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace KitabKhana.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleDefine.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly iUnitOfWork _unitOfWork;
     

        public CompanyController(iUnitOfWork iUnitOfWork)
        {
            _unitOfWork = iUnitOfWork;      
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Upsert(int id)
        {
            Company company = new();
           
            //ViewData["Category"] = CategoryList;
            //ViewBag.CategoryList = CategoryList;    
            //ViewBag.CoverType = CoverType;

            if (id == 0 || id == null)
            {
                return View(company);
            }
            else
            {
                company = _unitOfWork.Company.GetById(x => x.Id == id);
                return View(company);
            }
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company model)
        {


                if (model.Id == 0)
                {
                    _unitOfWork.Company.Add(model);
                    TempData["success"] = "Company Created Succesfully";
                }
                else
                {
                    _unitOfWork.Company.Update(model);
                    TempData["success"] = "Company Edited Succesfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            
          

        }


        #region For API
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _unitOfWork.Company.GetAll();
            return Json (new {Data = result});
        }



        [HttpDelete]
        public IActionResult DeletePost(int id)
        {
            var model = _unitOfWork.Company.GetById(x => x.Id == id);
            if (model != null)
            {              
                _unitOfWork.Company.Delete(model);
                _unitOfWork.Save();

                return Json(new { success = true, message = "Sucessfully Deleted" });          
            }
            return Json(new { success = false, message = $"The current {id} was not found" });


        }

        #endregion




    }
}
