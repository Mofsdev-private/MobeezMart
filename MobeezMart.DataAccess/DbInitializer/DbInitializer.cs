using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MobeezMart.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MobeezMart.Utility;
using MobeezMart.Models;

namespace MobeezMart.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly ApplicationDbContext _db;


        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManger,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManger = roleManger;
            _db = db;
        }

        public void Initialize()
        {
            //migrations if they are not applied

            try
            {
                if ( _db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch(Exception e) { 
            }
            //create roles if they are not created

            if (!_roleManger.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManger.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManger.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManger.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();
                _roleManger.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();

                //if roles are not created, then create admin user

                _userManager.CreateAsync(new ApplicationUser
                {

                    UserName = "mdevprjcts@gmail.com",
                    Email = "mdevprjcts@gmail.com",
                    PhoneNumber = "1234567890",
                    StreetAddress = "123 N Ave",
                    State = "IL",
                    PostalCode = "12345",
                    City = "Chiacgo",


                }, "AdminM2822441#").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "mdevprjcts@gmail.com");

                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }
            return;


        }
    }
}
