namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models.ToDoLists;
    using MoneySaver.Web.ViewModels.ToDoLists;
    using MoneySaver.Web.ViewModels.ToDoLists.Enums;

    [Authorize]
    public class ToDoListsController : Controller
    {
        private readonly IToDoListsService toDoListsService;
        private readonly UserManager<ApplicationUser> userManager;

        public ToDoListsController(IToDoListsService toDoListsService, UserManager<ApplicationUser> userManager)
        {
            this.toDoListsService = toDoListsService;
            this.userManager = userManager;
        }

        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddToDoListInputModel input)
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.toDoListsService.AddAsync(user.Id, input.Name, input.ListItems);

            return this.Redirect("/ToDoLists/All");
        }

        //TODO: create view component to exclude script for adding new items => id must be different in every list
        //In categories and everwhere where use modal for edit/delete - TOO!
        [HttpPost]
        public async Task<IActionResult> Edit(ToDoListViewModel list, string returnUrl, int itemsToShow = 5)
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var listItems = new List<ToDoItemViewModel>();

            if (list.Items.Count() > 0)
            {
                listItems.AddRange(list.Items);
            }

            foreach (var item in list.ListItems)
            {
                var listItemFromStrToViewModel = new ToDoItemViewModel()
                {
                    Name = item,
                    Status = StatusType.Active,
                };

                listItems.Add(listItemFromStrToViewModel);
            }

            list.Items = listItems;

            var listAsDto = new ToDoListDto()
            {
                ListItems = list.Items.Select(x => new ToDoItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = Enum.Parse<MoneySaver.Data.Models.Enums.StatusType>(x.Status.ToString()),
                }),
                Id = list.Id,
                Name = list.Name,
                Status = Enum.Parse<MoneySaver.Data.Models.Enums.StatusType>(list.Status.ToString()),
            };

            await this.toDoListsService.EditAsync(user.Id, listAsDto);

            return this.Redirect($"/ToDoLists/All?items={itemsToShow}");
        }


        public async Task<IActionResult> Delete(string id, int itemsToShow = 5)
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.toDoListsService.RemoveListAsync(user.Id, id);

            return this.Redirect($"/ToDoLists/All?items={itemsToShow}");
        }

        public async Task<IActionResult> DeleteItem(string id, string returnUrl, int itemsToShow = 5)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            await this.toDoListsService.RemoveListItemAsync(user.Id, id);

            if (returnUrl == "/ToDoLists/All")
            {
                return this.Redirect($"/ToDoLists/All?items={itemsToShow}");
            }

            return this.Content("No");
        }

        public async Task<IActionResult> All(int items = 5)
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var dtoLists = await this.toDoListsService.GetAllActive(user.Id);

            var model = new List<ToDoListViewModel>();

            model = dtoLists.Select(l => new ToDoListViewModel
            {
                Id = l.Id,
                Name = l.Name,
                Status = Enum.Parse<StatusType>(l.Status.ToString()),
                CreatedOn = l.CreatedOn,
                ItemsToShow = items,
                Items = l.ListItems.Select(li => new ToDoItemViewModel
                {
                    Id = li.Id,
                    Name = li.Name,
                    Status = Enum.Parse<StatusType>(li.Status.ToString()),
                })
                .ToList(),
            })
                .ToList();
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeItemStatus(string id, string returnUrl, int itemsToShow = 5)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (returnUrl == "/ToDoLists/All")
            {
                await this.toDoListsService.ChangeItemStatusAsync(user.Id, id);
                return this.Redirect($"/ToDoLists/All?items={itemsToShow}");
            }

            return this.Content("No");
        }
    }
}
