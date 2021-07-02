namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models.ToDoLists;

    public interface IToDoListsService
    {
        public Task<string> AddAsync(string userId, string name, IEnumerable<string> listItems);

        public Task EditAsync(string userId, string listId, IEnumerable<string> listItems);

        public Task RemoveListAsync(string userId, string listId);

        public Task RemoveListItemAsync(string userId, string listItemId);

        public Task<IEnumerable<ToDoListDto>> GetAll(string userId);

        public Task ChangeItemStatusAsync(string userId, string listItemId);

        public Task<bool> IsUserOwnListAsync(string userId, string listId);
    }
}
