using KitabKhana.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Repository.IRepository
{
    public interface iShopingCartRepository : iRepository<CartViewModel>
    {
        int CartIncrement(CartViewModel cart , int count);
        int CartDecrement(CartViewModel cart , int count);
    }
}
