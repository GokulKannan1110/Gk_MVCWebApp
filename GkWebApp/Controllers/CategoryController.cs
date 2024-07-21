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
    }
}
