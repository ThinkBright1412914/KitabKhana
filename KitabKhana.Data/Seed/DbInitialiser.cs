using KitabKhana.Data.Data;
using KitabKhana.Data.Repository.IRepository;
using KitabKhana.Model;
using KitabKhana.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Seed
{
    public class DbInitialiser : IDbInitialiser
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;


        public DbInitialiser
            (
                UserManager<IdentityUser> userManager,
                RoleManager<IdentityRole> roleManager,
                ApplicationDbContext context
           )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public void Initialise()
        {
            try
            {
                if(_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch
            {

            }

            if (!_roleManager.RoleExistsAsync(RoleDefine.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(RoleDefine.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(RoleDefine.Role_User_Only)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(RoleDefine.Role_Company_User)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(RoleDefine.Role_Employee)).GetAwaiter().GetResult();

                //_userManager.CreateAsync(new ApplicationUser
                //{
                //    UserName = "Admin",
                //    Name = "Admin",
                //    Email = "Admin@gmail.com",
                //    State = "3",
                //    PhoneNumber = "0771122",
                //    PostalCode = "12345",
                //    Address = "Thankot",
                //    City = "Kathmandu",
                //},"Admin@123");

                //ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "Admin@gmail.com");

                //_userManager.AddToRoleAsync(user ,RoleDefine.Role_Admin).GetAwaiter().GetResult();

            }
            return;

        }
    }
}
