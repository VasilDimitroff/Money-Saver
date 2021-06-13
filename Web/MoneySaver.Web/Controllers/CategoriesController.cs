namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.Models.Categories;
    using MoneySaver.Web.Models.Records;
    using MoneySaver.Web.Models.Records.Enums;

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

        public IActionResult Add()
        {
            return View();
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
                    CreatedOn = r.CreatedOn.ToString("D", CultureInfo.InvariantCulture),
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
            };

            return this.View(model);
        }
    }
}
