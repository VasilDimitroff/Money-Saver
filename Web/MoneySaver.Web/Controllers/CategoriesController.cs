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

        public CategoriesController(ICategoriesService categoriesService)
        {
            this.categoriesService = categoriesService;
        }

        public IActionResult All()
        {
            return View();
        }

        public async Task<IActionResult> Add(int walletId)
        {
            var model = new AddCategoryInputModel();
            model.Wallets = new List<WalletNameAndIdViewModel>();

            var wallets = await this.categoriesService.GetAllWalletsWithNameAndIdAsync("first");

            model.WalletId = walletId;

            model.Wallets = wallets.Select(w => new WalletNameAndIdViewModel
            {
                WalletId = w.WalletId,
                WalletName = w.WalletName,
            })
                .ToList();

            return this.View(model);
        }

        public async Task<IActionResult> Post(AddCategoryInputModel input)
        {
            return this.View();
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
