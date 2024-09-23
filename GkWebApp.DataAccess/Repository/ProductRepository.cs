using GkWebApp.DataAccess.Data;
using GkWebApp.DataAccess.Repository.IRepository;
using GkWebApp.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GkWebApp.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext context) : base(context) 
        {
            _db = context;
        }
        public void Update(Product product)
        {
            //This is the reason to not have the Update funcion in generic repository.
            //We may need to have some custom logics, so its better to have it under indivdual repository
           var productDBData = _db.Products.FirstOrDefault(u => u.Id == product.Id);
           if(productDBData != null)
            {
                productDBData.Title = product.Title;
                productDBData.Description = product.Description;
                productDBData.CategoryId = product.CategoryId;
                productDBData.ISBN = product.ISBN;
                productDBData.Author = product.Author;
                productDBData.ListPrice = product.ListPrice;
                productDBData.Price = product.Price;
                productDBData.Price50 = product.Price50;
                productDBData.Price100 = product.Price100;
                if (product.ImageUrl != null)
                {
                    productDBData.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
