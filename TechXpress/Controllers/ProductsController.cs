using Microsoft.AspNetCore.Mvc;
using TechXpress.Models;
using TechXpress.Repositories;

namespace TechXpress.Controllers
{
    public class ProductsController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public ProductsController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int categoryId)
        {
           var products = _unitOfWork.Products.Get_Products_by_Category(categoryId);
            ViewData["CategoryId"] = categoryId;
            return View(products);
        }
        public IActionResult Details(int id)
        {
            var product = _unitOfWork.Products.GetByID(id);
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Create(int categoryId)
        {
            ViewData["CategoryId"] = categoryId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Products product, int categoryId)
        {
            product.CategoryID = categoryId;
            if (ModelState.IsValid)
            {
                _unitOfWork.Products.Create(product);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index), new { categoryId = categoryId });
            }
            return View(product);
            }

        public IActionResult Edit(int id)
        {
            var product = _unitOfWork.Products.GetByID(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Products product)
        {
            if (id != product.ProductID) return BadRequest();

            if (ModelState.IsValid)
            {
                _unitOfWork.Products.Update(product);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.Products.GetByID(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _unitOfWork.Products.GetByID(id);
            if (product == null) return NotFound();

            _unitOfWork.Products.Delete(product);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
