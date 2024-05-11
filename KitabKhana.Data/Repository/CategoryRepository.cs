

using KitabKhana.Data.Data;
using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Repository
{
    public class CategoryRepository : Repository<Category> , iCategoryRepository
    {
      
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context; 
        }

        public void Update(Category model)
        {
            _context.Categories.Update(model);
        }

        public IEnumerable<Category> GetAll()
        {
           return _context.Categories.ToList();
        }
    }
}
