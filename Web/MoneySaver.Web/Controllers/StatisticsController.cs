namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.Models.Categories;
    using MoneySaver.Web.Models.Statistics;

    public class StatisticsController : Controller
    {
        private readonly IWalletsService walletsService;

        public StatisticsController(IWalletsService walletsService)
        {
            this.walletsService = walletsService;
        }

        [HttpGet("/Statistics")]
        public async Task<IActionResult> Index(int walletId)
        {
            var dbResult = await this.walletsService.GetWalletWithCategoriesAsync(walletId);

            StatisticsWalletViewModel model = new StatisticsWalletViewModel()
            {
                WalletId = dbResult.WalletId,
                ModifiedOn = dbResult.ModifiedOn,
                TotalAmount = dbResult.TotalAmount,
                WalletName = dbResult.WalletName,
                Categories = dbResult.Categories.Select(c => new CategoryStatisticsViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    TotalRecordsCount = c.RecordsCount,
                    TotalSpent = c.TotalSpent,
                })
                .ToList(),
            };

            return this.View(model);
        }
    }
}
