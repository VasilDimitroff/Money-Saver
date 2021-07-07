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

        public Task EditAsync(string userId, ToDoListDto list);

        public Task RemoveListAsync(string userId, string listId);

        public Task RemoveListItemAsync(string userId, string listItemId);

        public Task<IEnumerable<ToDoListDto>> GetAllAsync(string userId);

        public Task<IEnumerable<ToDoListDto>> GetAllActive(string userId);

        public Task<IEnumerable<ToDoListDto>> GetAllCompletedAsync(string userId);

        public Task<ToDoListDto> GetByIdAsync(string userId, string listId);

        public Task<string> GetListIdAsync(string listItemId);

        public Task ChangeItemStatusAsync(string userId, string listItemId);

        public Task<ToDoListDto> ChangeListStatusAsync(string userId, string listId);

        public Task<bool> IsUserOwnListAsync(string userId, string listId);
    }
}
