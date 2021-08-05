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

        private void FillDatabase()
        {
            this.CleanDatabase();
            this.AddUser();
            this.AddToDoLists();
            this.AddToDoItems();
        }

        private void CleanDatabase()
        {
            foreach (var list in this.db.ToDoLists)
            {
                foreach (var item in list.ListItems)
                {
                    this.db.ToDoItems.Remove(item);
                }

                this.db.SaveChanges();
                this.db.ToDoLists.Remove(list);
            }

            foreach (var user in this.db.Users)
            {
                this.db.Users.Remove(user);
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
