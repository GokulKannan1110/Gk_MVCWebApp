using GkWebApp.Data;
using GkWebApp.Models;
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
                _context.SaveChanges();
                return RedirectToAction("Index", "Category");
            }

            return View();
            
        }
    }
}
