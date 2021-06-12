using Microsoft.AspNetCore.Mvc;
using MoneySaver.Services.Data.Contracts;
using MoneySaver.Web.Models.Categories;
using MoneySaver.Web.Models.Records;
using MoneySaver.Web.Models.Records.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Web.Controllers
{
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
                    CreatedOn = r.CreatedOn,
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
