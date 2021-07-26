namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
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

        public async Task ChangeUserRole(string newRoleId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetAdminRoleId()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
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
            }

            return usersDto;
        }
    }
}
