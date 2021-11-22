using AppointmentScheduling.Models;
using AppointmentScheduling.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.DbInitialSeed
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager,
                             RoleManager<IdentityRole> roleManager)
        {
            _db = applicationDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public void InitializeDb()
        {
            try // Check for pending migrations
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate(); // Run the migartions
                }
            }
            catch (Exception)
            {
                throw;
            }

            // Check for Admin Role
            if (_db.Roles.Any(x => x.Name == Helper.Admin))
            {
                return;
            }


            // Create the Roles
            _roleManager.CreateAsync(new IdentityRole(Helper.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Helper.Reception)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Helper.Doctor)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Helper.Patient)).GetAwaiter().GetResult();


            // Create the first Admin user
            _userManager.CreateAsync(new ApplicationUser { 
                
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                Name = "Super Admin",
                Telephone = "999"

            }, "Admin123*").GetAwaiter().GetResult();


            // Assign admin Role for the user
            ApplicationUser user = _db.Users.FirstOrDefault(u => u.Email == "admin@gmail.com");

            _userManager.AddToRoleAsync(user, Helper.Admin).GetAwaiter().GetResult();

        }
    }
}
