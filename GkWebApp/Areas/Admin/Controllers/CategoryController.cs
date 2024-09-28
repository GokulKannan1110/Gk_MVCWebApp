using GkWebApp.DataAccess.Data;
using GkWebApp.DataAccess.Repository.IRepository;
using GkWebApp.Models;
using GkWebApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GkWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UtilityHelper.Role_Admin)]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context;
        //Now we have repository pattern in place, so we can replace the ApplicationDbContext with Category Repository.
        //private readonly ICategoryRepository _categoryRepo;

        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork UnitOfWork)
        {
            _unitOfWork = UnitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categoriesList = _unitOfWork.Category.GetAll().ToList();
            return View(categoriesList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Name and Display Order Cannot be the same.");
            }
            if (category.Name != null && category.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is a invalid name.");
            }
            if (ModelState.IsValid)
            {

                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["Success"] = "Category Created Successfully!";

                return RedirectToAction("Index", "Category");
            }

            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                TempData["Success"] = "Category Updated Successfully!";
                _unitOfWork.Save();
                return RedirectToAction("Index", "Category");
            }

            return View();

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category != null)
            {
                _unitOfWork.Category.Remove(category);
                TempData["Success"] = "Category Deleted Successfully!";
                _unitOfWork.Save();
            }
            return RedirectToAction("Index", "Category");
        }
    }
}
