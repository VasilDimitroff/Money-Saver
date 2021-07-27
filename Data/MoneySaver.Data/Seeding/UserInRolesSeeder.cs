using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoneySaver.Common;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Data.Seeding
{
    public class UserInRolesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == "v.b.dimitrow@gmail.com");

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
