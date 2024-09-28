using GkWebApp.DataAccess.Data;
using GkWebApp.DataAccess.Repository;
using GkWebApp.DataAccess.Repository.IRepository;
using GkWebApp.Models;
using GkWebApp.Models.Models;
using GkWebApp.Models.ViewModels;
using GkWebApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;

namespace GkWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UtilityHelper.Role_Admin)]
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _context;
        //Now we have repository pattern in place, so we can replace the ApplicationDbContext with Product Repository.
        //private readonly IProductRepository _ProductRepo;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork UnitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = UnitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }

        public IActionResult Index()
        {
            List<Product> productsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            
            return View(productsList);
        }

        public IActionResult Upsert(int? id)
        {
            //Here I need the list of Categories to be passed to this page, to display it in a dropdown.

            //First we can retrieve the Categories and using the EF Core feature -Projections, we can convert the object of Category to an object of SelectListItem
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
            //    .GetAll().Select(u => new SelectListItem
            //    {
            //        Text = u.Name,
            //        Value = u.Id.ToString()
            //    });

            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if(id ==0 || id == null)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(p => p.Id == id);
                return View(productVM);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile file)
        {
            if (productVM.Product.Title == productVM.Product.Description.ToString())
            {
                ModelState.AddModelError("name", "Title and Description Cannot be the same.");
            }
            if (productVM.Product.Title != null && productVM.Product.Title.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is a invalid title.");
            }
            ModelState.Remove("file");
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if( !string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        productVM.Product.ImageUrl = @"\images\product\" + filename;
                    }                    
                    using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + filename;
                    
                }

                if(productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["Success"] = "Product Created Successfully!";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["Success"] = "Product Updated Successfully!";

                }
                _unitOfWork.Save();
                
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
            
        }

       
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? Product = _unitOfWork.Product.Get(c => c.Id == id);
        //    if (Product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(Product);
        //}


        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? Product = _unitOfWork.Product.Get(c => c.Id == id);
        //    if (Product != null)
        //    {
        //        _unitOfWork.Product.Remove(Product);
        //        TempData["Success"] = "Product Deleted Successfully!";
        //        _unitOfWork.Save();
        //    }
        //    return RedirectToAction("Index", "Product");
        //}

        #region APICALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> productsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return Json(new { data = productsList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToDelete = _unitOfWork.Product.Get(p => p.Id == id);
            if (productToDelete == null)
            {
                return Json(new { success = false, message = "Error Occured"});
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.ImageUrl.TrimStart('\\'));

            if(System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToDelete);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
