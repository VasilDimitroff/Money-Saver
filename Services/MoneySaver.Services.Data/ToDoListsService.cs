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
    using MoneySaver.Services.Data.Models.ToDoLists;

    public class ToDoListsService : IToDoListsService
    {
        private readonly ApplicationDbContext dbContext;

        public ToDoListsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string userId, string name, IEnumerable<string> listItems)
        {
            bool isNullValues = listItems.Any(x => string.IsNullOrWhiteSpace(x) || x == null);

            if (isNullValues)
            {
                throw new ArgumentException(GlobalConstants.ListContainsEmptyItems);
            }

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
                    Status = StatusType.Active,
                };

                list.ListItems.Add(item);
            }

            await this.dbContext.ToDoLists.AddAsync(list);
            await this.dbContext.SaveChangesAsync();

            return list.Id;
        }

        public async Task EditAsync(string userId, ToDoListDto list)
        {
            if (!await this.IsUserOwnListAsync(userId, list.Id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditList);
            }

            var targetList = await this.dbContext.ToDoLists.FirstOrDefaultAsync(x => x.Id == list.Id);

            if (targetList == null)
            {
                throw new ArgumentException(GlobalConstants.ListNotExist);
            }

            bool isNullValues = list.ListItems.Any(x => string.IsNullOrWhiteSpace(x.Name) || x == null);

            if (isNullValues)
            {
                throw new ArgumentException(GlobalConstants.ListContainsEmptyItems);
            }

            targetList.Name = list.Name;
            targetList.Status = list.Status;
            targetList.ListItems = list.ListItems.Select(li => new ToDoItem
            {
                Id = li.Id == null ? Guid.NewGuid().ToString() : li.Id,
                Name = li.Name,
                Status = li.Status,
                ToDoListId = list.Id,
            })
                .ToHashSet();

            await this.dbContext.SaveChangesAsync();
        }

        public async Task RemoveListAsync(string userId, string listId)
        {
            if (!await this.IsUserOwnListAsync(userId, listId))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditList);
            }

            var list = await this.dbContext.ToDoLists.Include(l => l.ListItems).FirstOrDefaultAsync(x => x.Id == listId);

            if (list == null)
            {
                throw new ArgumentException(GlobalConstants.ListNotExist);
            }

            var itemsForDelete = new List<ToDoItem>();

            foreach (var item in list.ListItems)
            {
                itemsForDelete.Add(item);
            }

            this.dbContext.ToDoItems.RemoveRange(itemsForDelete);

            this.dbContext.ToDoLists.Remove(list);

            await this.dbContext.SaveChangesAsync();
        }

        public async Task RemoveListItemAsync(string userId, string listItemId)
        {
            var listItem = await this.dbContext.ToDoItems.FirstOrDefaultAsync(li => li.Id == listItemId);

            if (listItem == null)
            {
                throw new ArgumentException(GlobalConstants.ListItemWithThisIdNotExist);
            }

            var listId = listItem.ToDoListId;

            if (!await this.IsUserOwnListAsync(userId, listId))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditListItem);
            }

            this.dbContext.ToDoItems.Remove(listItem);

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ToDoListDto>> GetAllActive(string userId)
        {
            var lists = await this.dbContext.ToDoLists
                .Where(l => l.ApplicationUserId == userId && l.Status == StatusType.Active)
                .Select(l => new ToDoListDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Status = l.Status,
                    CreatedOn = l.CreatedOn,
                    ListItems = l.ListItems.Select(li => new ToDoItemDto
                    {
                        Id = li.Id,
                        Name = li.Name,
                        Status = li.Status,
                    })
                    .ToList(),
                })
                .ToListAsync();

            return lists;
        }

        public async Task ChangeItemStatusAsync(string userId, string listItemId)
        {
            var listItem = await this.dbContext.ToDoItems.FirstOrDefaultAsync(li => li.Id == listItemId);

            if (listItem == null)
            {
                throw new ArgumentException(GlobalConstants.ListItemWithThisIdNotExist);
            }

            var listId = listItem.ToDoListId;

            if (!await this.IsUserOwnListAsync(userId, listId))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditListItem);
            }

            if (listItem.Status == StatusType.Active)
            {
                listItem.Status = StatusType.Completed;
            }
            else
            {
                listItem.Status = StatusType.Active;
            }

            //  this.dbContext.Update(listItem);
            await this.dbContext.SaveChangesAsync();
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
