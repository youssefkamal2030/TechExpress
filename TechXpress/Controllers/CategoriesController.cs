using Microsoft.AspNetCore.Mvc;
using TechXpress.Models;
using TechXpress.Repositories;

namespace TechXpress.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public CategoriesController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var categories = _unitOfWork.Categories.GetAll();
            return View(categories);
        }

        public IActionResult Details(int id)
        {
            var category = _unitOfWork.Categories.GetByID(id);
            if (category == null) return NotFound();
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Categories category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Categories.Create(category);
                _unitOfWork.Save();
                return RedirectToAction("Index","Home");
            }
            return View(category);
        }

        public IActionResult Edit(int id)
        {
            var category = _unitOfWork.Categories.GetByID(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
       
        public IActionResult Edit(int id ,Categories category)
        {
            
            category.CategoryID = id;
            if (ModelState.IsValid)
            {
                _unitOfWork.Categories.Update(category);
                _unitOfWork.Save();
                return RedirectToAction("Index","Home");
            }
            return View(category);
        }

        public IActionResult Delete(int id)
        {
            var category = _unitOfWork.Categories.GetByID(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _unitOfWork.Categories.GetByID(id);
            if (category == null) return NotFound();

            _unitOfWork.Categories.Delete(category);
            _unitOfWork.Save();
            return RedirectToAction("Index","Home");
        }
    }
}
