using KitabKhana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Repository.IRepository
{
	public interface iOrderHeaderRepository : iRepository<OrderHeader>
	{
		void Update(OrderHeader model);

		void UpdateStatus(int id, string orderStatus , string? paymentStatus =  null);

		void UpdateStripePaymentStatus(int id, string SessionId);
	}
}
