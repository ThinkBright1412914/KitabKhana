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
	public class OrderDetailRepository : Repository<OrderDetail> ,  iOrderDetailRepository
	{
		private readonly ApplicationDbContext _context;

		public OrderDetailRepository(ApplicationDbContext context) : base(context) 
		{
			_context = context;
		}

		public void Update(OrderDetail model)
		{
			_context.OrderDetail.Update(model);
		}
	}
}
