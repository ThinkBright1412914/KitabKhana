
using KitabKhana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Repository.IRepository
{
    public interface iProductRepository : iRepository<Product>
    {
        void Update(Product product);
    }
}
