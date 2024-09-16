﻿using GkWebApp.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GkWebApp.DataAccess.Repository.IRepository
{
    public interface IProductRepository
    {
        void Update(Product product);
    }
}