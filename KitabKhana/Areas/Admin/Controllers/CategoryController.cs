
using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model;
using KitabKhana.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitabKhana.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleDefine.Role_Admin)]
    public class CategoryController : Controller
    {

        private readonly iUnitOfWork _unitOfWork;

        public CategoryController(iUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();
            return View(objCategoryList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category model)
        {
            if (model.Name == model.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The order cannot be same as name.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(model);
                _unitOfWork.Save();
                TempData["success"] = "Successfully Created";
                return RedirectToAction("Index");
            }
            return View(model);

        }



        //get
        public IActionResult Edit(int id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            var model = _unitOfWork.Category.GetById(x => x.Id == id);

            if (model == null)
            {
                return BadRequest();
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(model);
                _unitOfWork.Save();
                TempData["success"] = "Successfully Edited";
                return RedirectToAction("Index");
            }
            return View(model);
        }



        //get
        public IActionResult Delete(int id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            var model = _unitOfWork.Category.GetById(c => c.Id == id);

            if (model == null)
            {
                return BadRequest();
            }
            return View(model);
        }


        [HttpPost]
        public IActionResult DeletePost(int id)
        {
            var model = _unitOfWork.Category.GetById(x => x.Id == id);
            if (model != null)
            {
                _unitOfWork.Category.Delete(model);
                _unitOfWork.Save();
                TempData["success"] = "Successfully Deleted";
                return RedirectToAction("Index");
            }


            return NotFound();


        }



    }
}
