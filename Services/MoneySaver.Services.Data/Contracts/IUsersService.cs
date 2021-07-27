namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Common;
    using MoneySaver.Services.Data.Models.Users;

    public interface IUsersService
    {
        public Task<UserDto> GetUserByIdAsync(string userId);

        public Task<IEnumerable<UserDto>> GetAllUsersAsync();

        public Task<string> ChangeUserRoleAsync(string userId, string newRoleId);

        public Task<string> MarkAsDeletedAsync(string userId);

        public Task<string> UndeleteAsync(string userId);

        public Task<string> GetAdminRoleIdAsync();
    }
}
