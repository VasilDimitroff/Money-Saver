namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MoneySaver.Common;
    using MoneySaver.Services.Data.Models.Users;

    public interface IUsersService
    {
        public Task<IEnumerable<UserDto>> GetAllUsersAsync();

        public Task ChangeUserRole(string userId, string newRoleId);

        public Task<string> GetAdminRoleId();
    }
}
