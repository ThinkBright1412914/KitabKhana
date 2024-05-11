using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Repository.IRepository
{
    public interface iUnitOfWork
    {
        public iCategoryRepository Category { get; }

        public iCoverTypeRepository CoverType { get; }

        public iProductRepository Product { get; }

        public iCompanyRepository Company { get; }

        public iApplicationUserRepository ApplicationUser { get; }

        public iShopingCartRepository ShoppingCart { get; }

        public iOrderDetailRepository OrderDetail { get; }

        public iOrderHeaderRepository OrderHeader { get; }

        public void Save();

    }
}
