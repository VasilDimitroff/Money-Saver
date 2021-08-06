namespace MoneySaver.Web.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.Controllers;
    using MoneySaver.Web.ViewModels.ToDoLists;
    using Moq;
    using Xunit;

    public class ToDoListsControllerTests
    {
        private readonly IToDoListsService toDoListsService;
        private readonly FakeUserManager userManager;
        private DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder;
        private ApplicationDbContext db;
        private ToDoListsController controller;
        private AddToDoListInputModel inputModel;
        private ToDoListViewModel viewModel;

        public ToDoListsControllerTests()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase("toDoListsDatabase");
            this.db = new ApplicationDbContext(this.optionsBuilder.Options);
            this.toDoListsService = new ToDoListsService(this.db);
            this.userManager = new FakeUserManager();
            this.controller = new ToDoListsController(this.toDoListsService, this.userManager);
        }

        [Fact]
        public void ControllerShouldCreateToDoListSucessfully()
        {
            this.FillDatabase();

            this.inputModel = new AddToDoListInputModel
            {
                Name = "Test List",
                ListItems = new List<string>(),
            };

            this.inputModel.ListItems.Add("test ListItem 1");
            this.inputModel.ListItems.Add("test ListItem 2");

            var result = this.controller.Add(this.inputModel);

            Assert.Equal(4, this.db.ToDoLists.Count());
        }

        [Fact]
        public void GetAddShouldReturnView()
        {
            this.FillDatabase();

            var result = this.controller.Add();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task EditGetShouldReturnViewWithModel()
        {
            this.FillDatabase();

            var result = await this.controller.Edit("list1");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ToDoListViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Shoplist", model.Name);
            Assert.Equal(3, model.Items.Count());
        }

        [Fact]
        public async Task EditPostShouldEditListCorrectly()
        {
            this.FillDatabase();

            this.viewModel = new ToDoListViewModel
            {
                Id = "list1",
                Name = "Shoplist Edited",
                Status = ViewModels.ToDoLists.Enums.StatusType.Completed,
                ListItems = new List<string>(),
            };

            this.viewModel.ListItems.Add("test item");

            var result = await this.controller.Edit(this.viewModel, "/ToDoLists/AllLists");

            var viewResult = Assert.IsType<RedirectResult>(result);

            var list = this.db.ToDoLists.FirstOrDefault(x => x.Id == "list1");

            Assert.Equal("Shoplist Edited", list.Name);
            Assert.Equal(4, list.ListItems.Count());
            Assert.True(list.ListItems.Any(li => li.Name == "test item"));
            Assert.True(list.Status == StatusType.Completed);
        }

        [Fact]
        public async Task DeleteShouldDeleteListCorrectly()
        {
            this.FillDatabase();

            var result = await this.controller.Delete("list1", "/ToDoLists/AllLists", 5);

            var list = this.db.ToDoLists.FirstOrDefault(x => x.Id == "list1");

            Assert.Null(list);
            var viewResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(2, this.db.ToDoLists.Count());
        }

        [Fact]
        public async Task DeleteItemShouldDeleteListItemCorrectlyAndReturnRedirect()
        {
            this.FillDatabase();

            var listItemBeforeDelete = this.db.ToDoItems.FirstOrDefault(x => x.Id == "list1Item1");

            Assert.Equal("Tomatoes", listItemBeforeDelete.Name);

            var list = this.db.ToDoLists.FirstOrDefault(x => x.ListItems.Any(li => li.Id == "list1Item1"));

            var result = await this.controller.DeleteItem("list1Item1", "Shoplist", "/ToDoLists/AllLists");

            var listItemAfterDelete = this.db.ToDoItems.FirstOrDefault(x => x.Id == "list1Item1");

            Assert.Null(listItemAfterDelete);
            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.Equal(2, list.ListItems.Count());
        }

        [Fact]
        public async Task AllListsShouldReturnViewWithValidModel()
        {
            this.FillDatabase();

            var result = await this.controller.AllLists(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ToDoListViewModel>>(viewResult.ViewData.Model);

            Assert.Equal(3, model.Count());
        }

        [Fact]
        public async Task ChangeItemStatusShouldWorkCorrectlyWhenIsSettedToCompleted()
        {
            this.FillDatabase();

            var itemBeforeChangeStatus = this.db.ToDoItems.FirstOrDefault(li => li.Id == "list1Item1");

            Assert.True(itemBeforeChangeStatus.Status == StatusType.Active);

            var result = await this.controller.ChangeItemStatus("list1Item1", "Shoplist", "/ToDoLists/AllLists");

            var itemAfterChangeStatus = this.db.ToDoItems.FirstOrDefault(li => li.Id == "list1Item1");

            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.True(itemAfterChangeStatus.Status == StatusType.Completed);
        }

        [Fact]
        public async Task ChangeItemStatusShouldWorkCorrectlyWhenIsSettedToActive()
        {
            this.FillDatabase();

            var itemBeforeChangeStatus = this.db.ToDoItems.FirstOrDefault(li => li.Id == "list1Item3");

            Assert.True(itemBeforeChangeStatus.Status == StatusType.Completed);

            var result = await this.controller.ChangeItemStatus("list1Item3", "Shoplist", "/ToDoLists/AllLists");

            var itemAfterChangeStatus = this.db.ToDoItems.FirstOrDefault(li => li.Id == "list1Item3");

            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.True(itemAfterChangeStatus.Status == StatusType.Active);
        }

        [Fact]
        public async Task ChangeListStatusShouldWorkCorrectlyWhenIsSettedToActive()
        {
            this.FillDatabase();

            var listBeforeChangeStatus = this.db.ToDoLists.FirstOrDefault(li => li.Id == "list3");

            Assert.True(listBeforeChangeStatus.Status == StatusType.Completed);

            var result = await this.controller.ChangeListStatus("list3", "Todo List", "/ToDoLists/AllLists");

            var listAfterChangeStatus = this.db.ToDoLists.FirstOrDefault(li => li.Id == "list3");

            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.True(listAfterChangeStatus.Status == StatusType.Active);
        }

        [Fact]
        public async Task ChangeListStatusShouldWorkCorrectlyWhenIsSettedToCompleted()
        {
            this.FillDatabase();

            var listBeforeChangeStatus = this.db.ToDoLists.FirstOrDefault(li => li.Id == "list1");

            Assert.True(listBeforeChangeStatus.Status == StatusType.Active);

            var result = await this.controller.ChangeListStatus("list1", "Todo List", "/ToDoLists/AllLists");

            var listAfterChangeStatus = this.db.ToDoLists.FirstOrDefault(li => li.Id == "list1");

            var viewResult = Assert.IsType<RedirectResult>(result);

            Assert.True(listAfterChangeStatus.Status == StatusType.Completed);
        }

        private void FillDatabase()
        {
            this.CleanDatabase();
            this.AddUser();
            this.AddToDoLists();
            this.AddToDoItems();
        }

        private void CleanDatabase()
        {
            foreach (var item in this.db.ToDoItems)
            {
                this.db.ToDoItems.Remove(item);
                this.db.SaveChanges();
            }

            foreach (var list in this.db.ToDoLists)
            {
                this.db.ToDoLists.Remove(list);
                this.db.SaveChanges();
            }

            foreach (var user in this.db.Users)
            {
                this.db.Users.Remove(user);
                this.db.SaveChanges();
            }

            this.db.SaveChanges();
        }

        private void AddToDoItems()
        {
            var list1 = this.db.ToDoLists.Find("list1");

            var list1Items = new List<ToDoItem>();

            // add 3 list items
            list1Items.Add(
                new ToDoItem
                {
                    Id = "list1Item1",
                    CreatedOn = DateTime.UtcNow,
                    Name = "Tomatoes",
                    Status = StatusType.Active,
                    ToDoListId = "list1",
                });

            list1Items.Add(
                new ToDoItem
                {
                    Id = "list1Item2",
                    CreatedOn = DateTime.UtcNow,
                    Name = "Milk",
                    Status = StatusType.Active,
                    ToDoListId = "list1",
                });

            list1Items.Add(
               new ToDoItem
               {
                   Id = "list1Item3",
                   CreatedOn = DateTime.UtcNow,
                   Name = "Meat",
                   Status = StatusType.Completed,
                   ToDoListId = "list1",
               });

            this.db.ToDoItems.AddRange(list1Items);
            this.db.SaveChanges();

            var list2 = this.db.ToDoLists.Find("list2");

            var list2Items = new List<ToDoItem>();

            // add 3 list items
            list2Items.Add(
                new ToDoItem
                {
                    Id = "list2Item1",
                    CreatedOn = DateTime.UtcNow,
                    Name = "Play Station",
                    Status = StatusType.Completed,
                    ToDoListId = "list2",
                });

            list2Items.Add(
                new ToDoItem
                {
                    Id = "list2Item2",
                    CreatedOn = DateTime.UtcNow,
                    Name = "Monitor",
                    Status = StatusType.Active,
                    ToDoListId = "list2",
                });

            this.db.ToDoItems.AddRange(list2Items);
            this.db.SaveChanges();

            var list3 = this.db.ToDoLists.Find("list3");

            var list3Items = new List<ToDoItem>();

            // add 3 list items
            list3Items.Add(
                new ToDoItem
                {
                    Id = "list3Item1",
                    CreatedOn = DateTime.UtcNow,
                    Name = "Meeting",
                    Status = StatusType.Active,
                    ToDoListId = "list3",
                });

            list3Items.Add(
                new ToDoItem
                {
                    Id = "list3Item2",
                    CreatedOn = DateTime.UtcNow,
                    Name = "Editing",
                    Status = StatusType.Active,
                    ToDoListId = "list3",
                });

            list3Items.Add(
               new ToDoItem
               {
                   Id = "list3Item3",
                   CreatedOn = DateTime.UtcNow,
                   Name = "Design",
                   Status = StatusType.Active,
                   ToDoListId = "list3",
               });

            list3Items.Add(
              new ToDoItem
              {
                  Id = "list3Item4",
                  CreatedOn = DateTime.UtcNow,
                  Name = "Call to boss",
                  Status = StatusType.Active,
                  ToDoListId = "list3",
              });

            this.db.ToDoItems.AddRange(list3Items);
            this.db.SaveChanges();
        }

        private IEnumerable<ToDoList> AddToDoLists()
        {
            var lists = new List<ToDoList>();

            var list1 = new ToDoList
            {
                ApplicationUserId = "userId",
                CreatedOn = DateTime.UtcNow,
                Id = "list1",
                Name = "Shoplist",
                Status = StatusType.Active,
            };

            var list2 = new ToDoList
            {
                ApplicationUserId = "userId",
                CreatedOn = DateTime.UtcNow,
                Id = "list2",
                Name = "Wishlist",
                Status = StatusType.Active,
            };

            var list3 = new ToDoList
            {
                ApplicationUserId = "userId",
                CreatedOn = DateTime.UtcNow,
                Id = "list3",
                Name = "Todo List",
                Status = StatusType.Completed,
            };

            lists.Add(list1);
            lists.Add(list2);
            lists.Add(list3);

            this.db.ToDoLists.AddRange(lists);
            this.db.SaveChanges();

            return lists;
        }

        private ApplicationUser AddUser()
        {
            var user = new ApplicationUser()
            {
                Id = "userId",
                Email = "v.b.dimitrow@gmail.com",
                IsDeleted = false,
                PasswordHash = "UnhashedPass2021",
                UserName = "v.b.dimitrow@gmail.com",
                CreatedOn = DateTime.UtcNow,
                NormalizedUserName = "V.B.DIMITROW@GMAIL.COM",
                NormalizedEmail = "V.B.DIMITROW@GMAIL.COM",
                EmailConfirmed = false,
                SecurityStamp = "SecurityStamp",
                ConcurrencyStamp = "ConcurrencyStamp",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
            };

            this.db.Users.Add(user);
            this.db.SaveChanges();

            return user;
        }
    }
}
