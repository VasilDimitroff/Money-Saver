namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Contracts;

    public class ToDoListsService : IToDoListsService
    {
        private readonly ApplicationDbContext dbContext;

        public ToDoListsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string userId, string name, IEnumerable<string> listItems)
        {
            bool hasActiveListWithThisName = await this.dbContext.ToDoLists
                .AnyAsync(l => l.Name.ToLower() == name.ToLower() && l.Status == StatusType.Active);

            if (hasActiveListWithThisName)
            {
                throw new ArgumentException(GlobalConstants.ActiveListWithThisNameAlreadyExisting);
            }

            var list = new ToDoList()
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = userId,
                Name = name,
                Status = StatusType.Active,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
            };

            var items = new HashSet<ToDoItem>();

            foreach (var listItem in listItems)
            {
                var item = new ToDoItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = listItem,
                    ToDoListId = list.Id,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                };

                list.ListItems.Add(item);
            }

            await this.dbContext.ToDoLists.AddAsync(list);
            await this.dbContext.SaveChangesAsync();

            return list.Id;
        }

        public async Task EditAsync(string userId, string name, IEnumerable<string> listItems)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveListAsync(string userId, string listId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveListItemAsync(string userId, string listItemId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserOwnListAsync(string userId, string listId)
        {
            var list = await this.dbContext.ToDoLists
                .Where(l => l.Id == listId && l.ApplicationUserId == userId)
                .FirstOrDefaultAsync();

            if (list == null)
            {
                return false;
            }

            return true;
        }
    }
}
