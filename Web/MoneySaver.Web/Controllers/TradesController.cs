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
    using MoneySaver.Web.ViewModels.Trades;
    using MoneySaver.Web.ViewModels.Trades.Enums;

    [Authorize]
    public class TradesController : Controller
    {
        private readonly ICompaniesService companiesService;
        private readonly ITradesService tradesService;
        private readonly ICurrenciesService currenciesService;
        private readonly UserManager<ApplicationUser> userManager;

        public TradesController(
            ICompaniesService companiesService,
            ITradesService tradesService,
            ICurrenciesService currenciesService,
            UserManager<ApplicationUser> userManager)
        {
            this.companiesService = companiesService;
            this.tradesService = tradesService;
            this.currenciesService = currenciesService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Add()
        {
            if (this.ModelState.IsValid)
            {
            }

            var companies = await this.companiesService.GetAllCompaniesAsync();
            var currencies = await this.currenciesService.GetAllAsync();
            var model = new AddTradeInputModel
            {
                Companies = companies.Select(c => new CompanyViewModel
                {
                    Name = c.Name,
                    Ticker = c.Ticker,
                })
                .ToList(),
                Currencies = currencies.Select(c => new CurrencyViewModel
                {
                    Code = c.Code,
                    CurrencyId = c.CurrencyId,
                    Name = c.Name,
                })
                .ToList(),
            };

            var selectedCompany = new CompanyViewModel();
            model.SelectedCompany = selectedCompany;

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTradeInputModel input)
        {
            if (this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            /*
            if (input.Type == TradeType.Buy)
            {
                await this.tradesService.CreateBuyTradeAsync(user.Id, input.SelectedCompany.Ticker, input.Quantity, input.Price, input.SelectedCurrencyId);
            }
            else
            {
                await this.tradesService.CreateSellTradeAsync(user.Id, input.SelectedCompany.Ticker, input.Quantity, input.Price, input.SelectedCurrencyId);
            }
            */

            return this.Redirect("/Trades/All");
        }
    }
}
