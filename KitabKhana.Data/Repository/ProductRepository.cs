

using KitabKhana.Data.Data;
using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Repository
{
    public class ProductRepository : Repository<Product>, iProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context) 
        {     
            _context = context;
        }

        public void Update(Product model)
        {
           var obj = _context.Products.FirstOrDefault(x=> x.Id == model.Id); 
            if (obj != null)
            {
                obj.Author = model.Author;
                obj.Title = model.Title;
                obj.Description = model.Description;
                obj.ISBN = model.ISBN;  
                obj.ListPrice = model.ListPrice;
                obj.Price = model.Price;
                obj.Price50 = model.Price50;
                obj.Price100 = model.Price100;
                obj.Category = model.Category;
                obj.CoverType = model.CoverType;
                if (model.ImageUrl != null)
                {
                    obj.ImageUrl = model.ImageUrl;
                }
            }
           
 
        }
    }
}
