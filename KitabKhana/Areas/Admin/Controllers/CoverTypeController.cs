
using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model;
using KitabKhana.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitabKhana.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleDefine.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly iUnitOfWork _unitOfWork;

        public CoverTypeController(iUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<CoverType> result = _unitOfWork.CoverType.GetAll();
            return View(result);
        }


        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(model);
                _unitOfWork.Save();
                TempData["suceess"] = "Successfully Created";
                return RedirectToAction("Index");
            }
            return View(model);
        }


        public IActionResult Edit(int Id)
        {
            if (Id == 0 || Id == null)
            {
                return NotFound();
            }
            var model = _unitOfWork.CoverType.GetById(x => x.Id == Id);
            if (model != null)
            {
                return View(model);
            }
            return BadRequest();
        }


        [HttpPost]
        public IActionResult Edit(CoverType model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Update(model);
                _unitOfWork.Save();
                TempData["success"] = "Updated Succesfully";
                return RedirectToAction("Index");
            }
            return View(model);
        }


        public IActionResult Delete(int Id)
        {
            var result = _unitOfWork.CoverType.GetById(x => x.Id == Id);
            if (result != null)
            {
                return View(result);
            }

            return NotFound();
        }


        public IActionResult DeletePost(int Id)
        {
            var model = _unitOfWork.CoverType.GetById(x => x.Id == Id);
            if (model != null)
            {
                _unitOfWork.CoverType.Delete(model);
                _unitOfWork.Save();
                TempData["success"] = "Deleted Successfully";
                return RedirectToAction("Index");
            }
            return NotFound();
        }



    }
}
