using GkWebApp.DataAccess.Data;
using GkWebApp.DataAccess.Repository.IRepository;
using GkWebApp.Models;
using GkWebApp.Models.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GkWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _context;
        //Now we have repository pattern in place, so we can replace the ApplicationDbContext with Product Repository.
        //private readonly IProductRepository _ProductRepo;

        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork UnitOfWork)
        {
            _unitOfWork = UnitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> productsList = _unitOfWork.Product.GetAll().ToList();
            
            return View(productsList);
        }

        public IActionResult Create()
        {
            //Here I need the list of Categories to be passed to this page, to display it in a dropdown.

            //First we can retrieve the Categories and using the EF Core feature -Projections, we can convert the object of Category to an object of SelectListItem
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            //ViewBag.CategoryList = CategoryList;
            ViewData["CategoryList"] = CategoryList;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product Product)
        {
            if (Product.Title == Product.Description.ToString())
            {
                ModelState.AddModelError("name", "Title and Description Cannot be the same.");
            }
            if (Product.Title != null && Product.Title.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is a invalid title.");
            }
            if (ModelState.IsValid)
            {

                _unitOfWork.Product.Add(Product);
                _unitOfWork.Save();
                TempData["Success"] = "Product Created Successfully!";

                return RedirectToAction("Index", "Product");
            }

            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? Product = _unitOfWork.Product.Get(c => c.Id == id);
            if (Product == null)
            {
                return NotFound();
            }
            return View(Product);
        }

        [HttpPost]
        public IActionResult Edit(Product Product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(Product);
                TempData["Success"] = "Product Updated Successfully!";
                _unitOfWork.Save();
                return RedirectToAction("Index", "Product");
            }

            return View();

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? Product = _unitOfWork.Product.Get(c => c.Id == id);
            if (Product == null)
            {
                return NotFound();
            }
            return View(Product);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? Product = _unitOfWork.Product.Get(c => c.Id == id);
            if (Product != null)
            {
                _unitOfWork.Product.Remove(Product);
                TempData["Success"] = "Product Deleted Successfully!";
                _unitOfWork.Save();
            }
            return RedirectToAction("Index", "Product");
        }
    }
}
