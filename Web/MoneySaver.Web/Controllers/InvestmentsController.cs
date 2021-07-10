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
    using MoneySaver.Web.ViewModels.Currencies;
    using MoneySaver.Web.ViewModels.Investments;
    using MoneySaver.Web.ViewModels.Trades;
    using MoneySaver.Web.ViewModels.Trades.Enums;

    [Authorize]
    public class InvestmentsController : Controller
    {
        private readonly ICurrenciesService currenciesService;
        private readonly IInvestmentsWalletsService investmentsWalletsService;
        private readonly UserManager<ApplicationUser> userManager;

        public InvestmentsController(
            ICurrenciesService currenciesService,
            IInvestmentsWalletsService investmentsWalletsService,
            UserManager<ApplicationUser> userManager)
        {
            this.currenciesService = currenciesService;
            this.investmentsWalletsService = investmentsWalletsService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> AddWallet()
        {
            if (this.ModelState.IsValid)
            {
            }

            var currenciesDto = await this.currenciesService.GetAllAsync();

            var model = new AddInvestmentWalletInputModel()
            {
                Currencies = currenciesDto.Select(c => new CurrencyViewModel
                {
                    Code = c.Code,
                    CurrencyId = c.CurrencyId,
                    Name = c.Name,
                })
                .ToList(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddWallet(AddInvestmentWalletInputModel input)
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.investmentsWalletsService
                .AddAsync(user.Id, input.Name, input.SelectedCurrencyId);

            return this.Redirect("/Investments/All");
        }

        public async Task<IActionResult> EditWallet(int id)
        {
            if (this.ModelState.IsValid)
            {
            }

            var currenciesDto = await this.currenciesService.GetAllAsync();
            var currentCurrency = await this.investmentsWalletsService.GetInvestmentCurrencyAsync(id);

            var model = new EditInvestmentWalletInputModel()
            {
                Id = id,
                Name = await this.investmentsWalletsService.GetInvestmentWalletNameAsync(id),
                SelectedCurrency = new CurrencyViewModel
                {
                    CurrencyId = currentCurrency.CurrencyId,
                    Code = currentCurrency.Code,
                    Name = currentCurrency.Name,
                },
                Currencies = currenciesDto.Select(c => new CurrencyViewModel
                {
                    Code = c.Code,
                    CurrencyId = c.CurrencyId,
                    Name = c.Name,
                })
                .ToList(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditWallet(EditInvestmentWalletInputModel input)
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.investmentsWalletsService.EditAsync(user.Id, input.Id, input.SelectedCurrency.CurrencyId, input.Name);

            return this.Redirect("/Investments/All");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.investmentsWalletsService.RemoveAsync(user.Id, id);

            return this.Redirect("/Investments/All");
        }

        public async Task<IActionResult> All()
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);
            var result = await this.investmentsWalletsService.GetAllAsync(user.Id);

            var model = new List<InvestmentWalletViewModel>();

            model = result.Select(iw => new InvestmentWalletViewModel
            {
                Id = iw.Id,
                CreatedOn = iw.CreatedOn,
                Name = iw.Name,
                Currency = new CurrencyViewModel
                {
                    Name = iw.Currency.Name,
                    Code = iw.Currency.Code,
                    CurrencyId = iw.Currency.CurrencyId,
                },
                TotalTradesCount = iw.TotalTradesCount,
                TotalBuyTradesAmount = iw.TotalBuyTradesAmount,
                TotalSellTradesAmount = iw.TotalSellTradesAmount,
            })
                .ToList();

            return this.View(model);
        }
    }
}
