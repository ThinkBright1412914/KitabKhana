using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Model.ViewModel
{
    public class OrderViewModel
    {
        public OrderHeader orderHeader {  get; set; }

        public IEnumerable<OrderDetail> orderDetail { get; set; }
    }
}
