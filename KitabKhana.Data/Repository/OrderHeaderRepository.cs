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
	public class OrderHeaderRepository : Repository<OrderHeader> , iOrderHeaderRepository
	{
		private readonly ApplicationDbContext _context;
		
		public OrderHeaderRepository(ApplicationDbContext context) :base(context) 
		{
			_context = context;
		}

		public void Update(OrderHeader model)
		{
			_context.OrderHeader.Update(model);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb = _context.OrderHeader.FirstOrDefault(x => x.Id == id);
			if (orderFromDb != null)
			{
				orderFromDb.OrderStatus = orderStatus;
				if(paymentStatus != null)
				{
					orderFromDb.PaymentStatus = paymentStatus;
				}
			}
		}

        public void UpdateStripePaymentStatus(int id, string SessionId)
        {
            var orderFromDb = _context.OrderHeader.FirstOrDefault(x => x.Id == id);
			orderFromDb.PaymentDate = DateTime.Now;
			orderFromDb.SessionId = SessionId;
			
        }

        
	}
}
