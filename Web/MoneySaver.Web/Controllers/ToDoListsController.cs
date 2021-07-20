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

        public ToDoListsController(
            IToDoListsService toDoListsService,
            UserManager<ApplicationUser> userManager)
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
            if (!this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.toDoListsService.AddAsync(user.Id, input.Name, input.ListItems);

            return this.Redirect("/ToDoLists/AllLists");
        }

        // TODO: create view component to exclude script for adding new items => id must be different in every list
        // In categories and everwhere where use modal for edit/delete - TOO!
        public async Task<IActionResult> Edit(string id)
        {
            if (!this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var dto = await this.toDoListsService.GetByIdAsync(user.Id, id);

            var model = new ToDoListViewModel()
            {
                Id = dto.Id,
                CreatedOn = dto.CreatedOn,
                Name = dto.Name,
                Status = Enum.Parse<StatusType>(dto.Status.ToString()),
                Items = dto.ListItems.Select(li => new ToDoItemViewModel
                {
                    Id = li.Id,
                    Name = li.Name,
                    Status = Enum.Parse<StatusType>(li.Status.ToString()),
                    CreatedOn = li.CreatedOn,
                })
                .OrderBy(li => li.CreatedOn)
                .ToList(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ToDoListViewModel list, string returnUrl)
        {
            if (!this.ModelState.IsValid)
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

            if (returnUrl == "/ToDoLists/AllLists")
            {
                return this.Redirect($"/ToDoLists/AllLists");
            }
            else if (returnUrl == $"/ToDoLists/Edit/{list.Id}")
            {
                return this.Redirect($"/ToDoLists/Edit/{list.Id}");
            }

            return this.Content("No");
        }

        public async Task<IActionResult> Delete(string id, string returnUrl, int itemsToShow = 5)
        {
            if (!this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.toDoListsService.RemoveListAsync(user.Id, id);

            return this.Redirect($"/ToDoLists/AllLists");
        }

        public async Task<IActionResult> DeleteItem(string id, string divId, string returnUrl)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var listId = await this.toDoListsService.GetListIdAsync(id);

            await this.toDoListsService.RemoveListItemAsync(user.Id, id);

            if (returnUrl == "/ToDoLists/AllLists")
            {
                return this.Redirect($"/ToDoLists/AllLists#{divId}");
            }
            else if (returnUrl == $"/ToDoLists/Edit/{listId}")
            {
                return this.Redirect($"/ToDoLists/Edit/{listId}");
            }
            else
            {
                return this.Redirect($"/Home/Error");
            }
        }

        public async Task<IActionResult> AllLists(int getStatus = 1)
        {
            // if getStatus == 1 => Return all lists
            // if getStatus == 2 => return Active
            // if getStatus == 3 => return Completed
            var user = await this.userManager.GetUserAsync(this.User);

            List<ToDoListDto> dtoLists = new List<ToDoListDto>();

            if (getStatus == 1)
            {
                dtoLists = (List<ToDoListDto>)await this.toDoListsService.GetAllAsync(user.Id);
            }

            if (getStatus == 2)
            {
               dtoLists = (List<ToDoListDto>)await this.toDoListsService.GetAllActive(user.Id);
            }

            if (getStatus == 3)
            {
                dtoLists = (List<ToDoListDto>)await this.toDoListsService.GetAllCompletedAsync(user.Id);
            }

            var model = new List<ToDoListViewModel>();

            model = dtoLists.Select(l => new ToDoListViewModel
            {
                Id = l.Id,
                Name = l.Name,
                Status = Enum.Parse<StatusType>(l.Status.ToString()),
                CreatedOn = l.CreatedOn,
                GetStatus = getStatus,
                Items = l.ListItems.Select(li => new ToDoItemViewModel
                {
                    Id = li.Id,
                    Name = li.Name,
                    Status = Enum.Parse<StatusType>(li.Status.ToString()),
                    CreatedOn = li.CreatedOn,
                })
                .OrderBy(li => li.CreatedOn)
                .ToList(),
            })
                .OrderBy(li => li.CreatedOn)
                .ToList();

            switch (getStatus)
                {
                    case 1: this.ViewData["Status"] = "All"; break;
                    case 2: this.ViewData["Status"] = "Active"; break;
                    case 3: this.ViewData["Status"] = "Completed"; break;
                }

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeItemStatus(string id, string divId, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.toDoListsService.ChangeItemStatusAsync(user.Id, id);

            var listId = await this.toDoListsService.GetListIdAsync(id);

            if (returnUrl == "/ToDoLists/AllLists")
            {
                return this.Redirect($"/ToDoLists/AllLists#{divId}");
            }
            else if (returnUrl == $"/ToDoLists/Edit/{listId}")
            {
                return this.Redirect($"/ToDoLists/Edit/{listId}");
            }

            return this.Redirect($"/ToDoLists/AllLists");
        }

        public async Task<IActionResult> ChangeListStatus(string id, string divId, string returnUrl, int getStatus = 1)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            var result = await this.toDoListsService.ChangeListStatusAsync(user.Id, id);

            if (returnUrl == "/ToDoLists/AllLists")
            {
                return this.Redirect($"/ToDoLists/AllLists?getstatus={getStatus}#{divId}");
            }
            else if (returnUrl == $"/ToDoLists/Edit/{id}")
            {
                return this.Redirect($"/ToDoLists/Edit/{id}");
            }
            else if (returnUrl == "/")
            {
                return this.Redirect("/#active-lists");
            }

            return this.Redirect($"/ToDoLists/AllLists");
        }
    }
}
