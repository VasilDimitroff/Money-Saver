namespace MoneySaver.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data.Models;

    public class UserInRolesSeeder : ISeeder
    {
        public UserInRolesSeeder()
        {
        }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == "v.b.dimitrow@gmail.com");

            //var user1 = new ApplicationUser()
            //{
            //};

            //user.Roles.Add(new IdentityUserRole<string>()
            //{
            //    RoleId = dbContext.Roles.FirstOrDefault(r => r.Name == "a").Id,
            //    UserId = user.Id,
            //});

            //dbContext.Add(user);
            //dbContext.SaveChanges();

            if (user == null)
            {
                return;
            }

            var adminRole = await dbContext.Roles
                .FirstOrDefaultAsync(ur => ur.Name.ToUpper() == GlobalConstants.AdministratorRoleName.ToUpper());

            if (dbContext.UserRoles.Any(ur => ur.RoleId == adminRole.Id && ur.UserId == user.Id))
            {
                return;
            }

            IdentityUserRole<string> userRole = new IdentityUserRole<string>();
            userRole.UserId = user.Id;
            userRole.RoleId = adminRole.Id;

            user.Roles.Add(userRole);
            await dbContext.SaveChangesAsync();
        }
    }
}
