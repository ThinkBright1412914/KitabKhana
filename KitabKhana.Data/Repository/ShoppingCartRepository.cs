using KitabKhana.Data.Data;
using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Repository
{
    public class ShoppingCartRepository : Repository<CartViewModel>, iShopingCartRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context; 
        }

        public int CartDecrement(CartViewModel cart, int count)
        {
            cart.Count -= count;
            return cart.Count;
        }

        public int CartIncrement(CartViewModel cart, int count)
        {
           cart.Count += count;
            return cart.Count;
        }
    }
}
