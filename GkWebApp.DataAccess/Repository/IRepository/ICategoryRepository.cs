using GkWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GkWebApp.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRespository<Category>
    {
        void Update(Category category);
        void Save();
    }
}
