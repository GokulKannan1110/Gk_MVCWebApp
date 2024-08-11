using GkWebApp.Data;
using GkWebApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GkWebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext Context)
        {
            _context = Context;
        }

        public IActionResult Index()
        {
            List<Category> categoriesList = _context.Categories.ToList();
            return View(categoriesList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if(category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Name and Display Order Cannot be the same.");
            }
            if (category.Name != null && category.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is a invalid name.");
            }
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                TempData["Success"] = "Category Created Successfully!";
                _context.SaveChanges();
                return RedirectToAction("Index", "Category");
            }

            return View();
            
        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == id);
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                TempData["Success"] = "Category Updated Successfully!";
                _context.SaveChanges();
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
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                TempData["Success"] = "Category Deleted Successfully!";
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Category");
        }
    }
}
