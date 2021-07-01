using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoneySaver.Services.Data.Contracts
{
    public interface IToDoListsService
    {
        public Task<string> AddAsync(string userId, string name, IEnumerable<string> listItems);

        public Task EditAsync(string userId, string listId, IEnumerable<string> listItems);

        public Task RemoveListAsync(string userId, string listId);

        public Task RemoveListItemAsync(string userId, string listItemId);

        public Task<bool> IsUserOwnListAsync(string userId, string listId);
    }
}
