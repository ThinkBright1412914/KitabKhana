using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Model.ViewModel
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<CartViewModel> ListCart { get; set; }    

        public OrderHeader OrderHeader { get; set; }    
    }
}
