
using KitabKhana.Data.Data;
using KitabKhana.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Repository
{
    public class UnitOfWork : iUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context) {
            _context = context;
            Category = new CategoryRepository(context);
            CoverType = new CoverTypeRepository(context);
            Product = new ProductRepository(context);          
            Company = new CompanyRepository(context);
            ShoppingCart = new ShoppingCartRepository(context);
            ApplicationUser = new ApplicationUserRepository(context);
            OrderHeader = new OrderHeaderRepository(context);
            OrderDetail = new OrderDetailRepository(context);
        }

        public iCategoryRepository Category { get; private set; }

        public iCoverTypeRepository CoverType { get; private set; }

        public iProductRepository Product { get; private set; }

        public iCompanyRepository Company { get; private set; }

        public iApplicationUserRepository ApplicationUser {  get; private set; }

        public iShopingCartRepository ShoppingCart { get; private set; }

		public iOrderDetailRepository OrderDetail { get; private set; } 

		public iOrderHeaderRepository OrderHeader {  get; private set; }

		public void Save()
        {
            _context.SaveChanges();
        }
    }
}
