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
            _db.Products.Update(product);
        }
    }
}
