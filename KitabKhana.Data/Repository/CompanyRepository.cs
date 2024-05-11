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
    public class CompanyRepository : Repository<Company>, iCompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context) : base(context)
        {

            _context = context;
        }

        public void Update(Company company)
        {
            _context.Update(company);
            //var model = _context.Companys.FirstOrDefault(x => x.Id == company.Id);
            //if (model != null)
            //{
            //    model.Name = company.Name;
            //    model.Address = company.Address;
            //    model.City = company.City;
            //    model.PostalCode = company.PostalCode;
            //    model.PhoneNo = company.PhoneNo;
            //    model.State = company.State;
                
            //}
        }
    }
}
