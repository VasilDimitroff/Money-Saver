namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Common;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels.Categories;
    using MoneySaver.Web.ViewModels.Records;
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class CategoriesController : Controller
    {
        private readonly ICategoriesService categoriesService;
        private readonly IWalletsService walletsService;
        private readonly UserManager<ApplicationUser> userManager;

        public CategoriesController(ICategoriesService categoriesService, IWalletsService walletsService, UserManager<ApplicationUser> userManager)
        {
            this.categoriesService = categoriesService;
            this.walletsService = walletsService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Add(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            var model = new AddCategoryInputModel();

            var wallets = await this.categoriesService.GetAllWalletsWithNameAndIdAsync(user.Id);

            model.WalletId = id;
            model.WalletName = await this.walletsService.GetWalletNameAsync(id);
            model.Wallets = wallets.Select(w => new AddCategoryWalletsListViewModel
            {
                WalletId = w.WalletId,
                WalletName = w.WalletName,
            })
                .ToList();

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCategoryInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, input.WalletId))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            await this.categoriesService.AddAsync(input.Name, input.WalletId, input.BadgeColor);
            return this.Redirect($"/Wallets/Categories/{input.WalletId}");
        }

        public async Task<IActionResult> Edit(int id, int walletId)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, walletId))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForViewOrEditCategory);
            }

            var categoryInfo = await this.categoriesService.GetCategoryInfoForEditAsync(id);
            var wallets = await this.categoriesService.GetAllWalletsWithNameAndIdAsync(user.Id);

            var model = new EditCategoryInputModel();
            model.CategoryId = id;
            model.CategoryName = categoryInfo.CategoryName;
            model.BadgeColor = Enum.Parse<BadgeColor>(categoryInfo.BadgeColor);
            model.WalletId = walletId;
            model.Wallets = wallets.Select(w => new EditCategoryWalletsListViewModel
            {
                WalletId = w.WalletId,
                WalletName = w.WalletName,
            })
                .ToList();

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCategoryInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, input.WalletId))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, input.CategoryId))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForViewOrEditCategory);
            }

            await this.categoriesService.EditAsync(input.CategoryId, input.CategoryName, input.WalletId, input.BadgeColor.ToString());

            return this.Redirect($"/Wallets/Categories/{input.WalletId}");
        }

        public async Task<IActionResult> Delete(int id, int walletId)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, walletId))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForViewOrEditCategory);
            }

            var model = new DeleteCategoryInputModel();

            var modelInfo = await this.categoriesService.GetCategoryInfoForDeleteAsync(id, walletId);

            model.WalletId = modelInfo.WalletId;
            model.WalletName = modelInfo.WalletName;
            model.OldCategoryId = id;
            model.OldCategoryName = modelInfo.OldCategoryName;
            model.OldCategoryBadgeColor = Enum.Parse<BadgeColor>(modelInfo.OldCategoryBadgeColor.ToString());
            model.Categories = modelInfo.Categories
                .Select(c => new DeleteCategoryNameAndIdViewModel
                {
                    BadgeColor = Enum.Parse<BadgeColor>(c.BadgeColor.ToString()),
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                })
                .ToList();

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteCategoryInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.walletsService.IsUserOwnWalletAsync(user.Id, input.WalletId))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForEditWallet);
            }

            if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, input.OldCategoryId))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForViewOrEditCategory);
            }

            if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, input.NewCategoryId) && input.NewCategoryId != -1)
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForViewOrEditCategory);
            }

            await this.categoriesService.RemoveAsync(input.OldCategoryId, input.NewCategoryId);
            return this.Redirect($"/Wallets/Categories/{input.WalletId}");
        }

        public async Task<IActionResult> Details(int id, int page = 1)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForViewOrEditCategory);
            }

            const int ItemsPerPage = 12;

            var category = await this.categoriesService.GetRecordsByCategoryAsync(id, page, ItemsPerPage);
            CategoryRecordsViewModel model = new CategoryRecordsViewModel()
            {
                Records = category.Records
                .Select(r => new RecordsByCategoryViewModel
                {
                    Amount = r.Amount,
                    CreatedOn = r.CreatedOn.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture),
                    Description = r.Description,
                    Id = r.Id,
                    Type = Enum.Parse<RecordTypeInputModel>(r.Type.ToString()),
                })
                .ToList(),
                Category = category.Category,
                CategoryId = id,
                Currency = category.Currency,
                WalletId = category.WalletId,
                WalletName = category.WalletName,
                BadgeColor = Enum.Parse<BadgeColor>(category.BadgeColor.ToString()),
            };

            model.ItemsPerPage = ItemsPerPage;
            model.PageNumber = page;
            model.RecordsCount = this.categoriesService.GetRecordsCount(id);

            return this.View(model);
        }

        public async Task<IActionResult> Search(int id, string searchTerm, int page = 1)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!await this.categoriesService.IsUserOwnCategoryAsync(user.Id, id))
            {
                throw new ArgumentException(GlobalConstants.NoPermissionForViewOrEditCategory);
            }

            if (searchTerm == null)
            {
                searchTerm = string.Empty;
            }

            CategoryRecordsViewModel model = new CategoryRecordsViewModel();

            const int ItemsPerPage = 12;

            model.ItemsPerPage = ItemsPerPage;
            model.PageNumber = page;

            /*
            // !!!
            var category = await this.categoriesService.GetRecordsByCategoryAsync(searchTerm, id, page, ItemsPerPage);

            model.Category = category.Category;
            model.CategoryId = category.CategoryId;
            model.Currency = category.Currency;
            model.WalletId = category.WalletId;
            model.WalletName = category.WalletName;
            model.BadgeColor = Enum.Parse<BadgeColor>(category.BadgeColor.ToString());

            //!!
            model.RecordsCount = this.walletsService.GetSearchRecordsCount(searchTerm, id);
            model.SearchTerm = searchTerm;

            model.Records = category.Records
                .Select(r => new RecordsByCategoryViewModel
                {
                    Amount = r.Amount,
                    CreatedOn = r.CreatedOn.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture),
                    Description = r.Description,
                    Id = r.Id,
                    Type = Enum.Parse<RecordTypeInputModel>(r.Type.ToString()),
                })
                .ToList();
             */

            return this.View(model);
        }
    }
}
