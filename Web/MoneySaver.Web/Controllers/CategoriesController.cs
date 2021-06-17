namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels.Categories;
    using MoneySaver.Web.ViewModels.Records;
    using MoneySaver.Web.ViewModels.Records.Enums;

    public class CategoriesController : Controller
    {
        private readonly ICategoriesService categoriesService;
        private readonly IWalletsService walletsService;

        public CategoriesController(ICategoriesService categoriesService, IWalletsService walletsService)
        {
            this.categoriesService = categoriesService;
            this.walletsService = walletsService;
        }

        public IActionResult All()
        {
            return View();
        }

        public async Task<IActionResult> Add(int id)
        {
            var model = new AddCategoryInputModel();

            var wallets = await this.categoriesService.GetAllWalletsWithNameAndIdAsync("first");

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
            await this.categoriesService.AddAsync(input.Name, input.WalletId, input.BadgeColor);
            return this.Redirect($"/Wallets/Details/{input.WalletId}");
        }

        public async Task<IActionResult> Edit(int id, int walletId)
        {
            var categoryInfo = await this.categoriesService.GetCategoryInfoForEditAsync(id);
            var wallets = await this.categoriesService.GetAllWalletsWithNameAndIdAsync("first");

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
            await this.categoriesService.EditAsync(input.CategoryId, input.CategoryName, input.WalletId, input.BadgeColor.ToString());

            return this.Redirect($"/Wallets/Details/{input.WalletId}");
        }

        public async Task<IActionResult> Delete(int id, int walletId)
        {
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
            await this.categoriesService.RemoveAsync(input.OldCategoryId, input.NewCategoryId);
            return this.Redirect($"/Wallets/Details/{input.WalletId}");
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await this.categoriesService.GetRecordsByCategoryAsync(id);
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
                CategoryId = category.CategoryId,
                Currency = category.Currency,
                WalletId = category.WalletId,
                WalletName = category.WalletName,
                BadgeColor = Enum.Parse<BadgeColor>(category.BadgeColor.ToString()),
            };

            return this.View(model);
        }
    }
}
