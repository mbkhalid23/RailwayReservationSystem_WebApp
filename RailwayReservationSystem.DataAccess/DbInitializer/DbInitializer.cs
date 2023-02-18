using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailwayReservationSystem.DataAccess.Data;
using RailwayReservationSystem.DataAccess.DBInitializer;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        //implement migrations if they are not applied yet
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 1)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            //create roles if they are not created

            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Passenger)).GetAwaiter().GetResult();

                //if roles are not created, create an admin user
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@rrs.com",
                    Email = "admin@rrs.com",
                    Name = "Admin",
                    CNIC = "61101-1234567-1",
                    DOB = DateTime.Parse("01-01-1960"),
                    Gender = "Male",
                    PhoneNumber = "0300-5621752"
                },"Admin123@").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@rrs.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }

        }
    }
}
