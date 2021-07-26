namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models.Users;

    public interface IUsersService
    {
        public Task<IEnumerable<UserDto>> GetAllUsers();

        public Task ChangeUserRole(string newRoleId);

        public Task<string> GetAdminRoleId();
    }
}
