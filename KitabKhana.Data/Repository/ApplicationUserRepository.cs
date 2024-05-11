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
    public class ApplicationUserRepository : Repository<ApplicationUser> , iApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
