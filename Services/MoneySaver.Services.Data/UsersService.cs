namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models.Roles;
    using MoneySaver.Services.Data.Models.Users;

    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext dbContext;
        


        public UsersService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task ChangeUserRole(string userId, string newRoleId)
        {
            var user = await this.dbContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == userId);
            var role = await this.dbContext.Roles.FirstOrDefaultAsync(ur => ur.Id == newRoleId);

            if (user == null)
            {
                throw new ArgumentException(GlobalConstants.UserWithThisIdNotExist);
            }

            // role is null when newRoleId is null; When newRoleId is null, then should make user with regular access
            if (newRoleId == "Regular_User")
            {
                foreach (var currentRole in user.Roles)
                {
                    user.Roles.Remove(currentRole);
                }

                await this.dbContext.SaveChangesAsync();
                return;
            }

            if (role == null)
            {
                throw new ArgumentException(GlobalConstants.RoleWithThisIdNotExist);
            }

            IdentityUserRole<string> userRole = new IdentityUserRole<string>
            {
                RoleId = role.Id,
                UserId = user.Id,
            };

            user.Roles.Add(userRole);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<string> GetAdminRoleId()
        {
            string adminRoleName = GlobalConstants.AdministratorRoleName;

            var adminRole = await this.dbContext.Roles
                .FirstOrDefaultAsync(ur => ur.Name.ToLower() == adminRoleName.ToLower());

            if (adminRole == null)
            {
                throw new ArgumentException(GlobalConstants.RoleWithThisNameNotExist);
            }

            return adminRole.Id;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await this.dbContext.Users.Include(u => u.Roles).ToListAsync();

            var usersDto = new List<UserDto>();

            foreach (var user in users)
            {
                var userDto = new UserDto
                {
                    Id = user.Id,
                    CreatedOn = user.CreatedOn,
                    Username = user.UserName,
                };

                foreach (var role in user.Roles)
                {
                    var roleDto = await this.dbContext.Roles
                        .Select(r => new RoleDto
                        {
                            Id = r.Id,
                            Name = r.Name,
                        })
                        .FirstOrDefaultAsync(x => x.Id == role.RoleId);

                    userDto.Roles.Add(roleDto);
                }

                usersDto.Add(userDto);
            }

            return usersDto;
        }
    }
}
