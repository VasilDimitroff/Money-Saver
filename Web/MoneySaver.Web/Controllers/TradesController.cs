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
    public class TradesController : Controller
    {
        private readonly ICompaniesService companiesService;
        private readonly ITradesService tradesService;
        private readonly UserManager<ApplicationUser> userManager;

        public TradesController(
            ICompaniesService companiesService,
            ITradesService tradesService,
            UserManager<ApplicationUser> userManager)
        {
            this.companiesService = companiesService;
            this.userManager = userManager;
            this.tradesService = tradesService;
        }

        public async Task<IActionResult> Add(int investmentWalletId)
        {
            if (!this.ModelState.IsValid)
            {
            }

            var companies = await this.companiesService.GetAllCompaniesAsync();
            var model = new AddTradeInputModel
            {
                InvestmentWalletId = investmentWalletId,
                Companies = companies.Select(c => new CompanyViewModel
                {
                    Name = c.Name,
                    Ticker = c.Ticker,
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
            if (!this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var selectedCompany = await this.companiesService
                .GetCompanyByTickerAsync(input.SelectedCompany.Ticker);

            input.SelectedCompany.Name = selectedCompany.Name;

            if (input.Type == TradeType.Buy)
            {
                await this.tradesService.CreateBuyTradeAsync(user.Id, input.InvestmentWalletId, input.SelectedCompany.Ticker, input.Quantity, input.Price);
            }
            else
            {
                await this.tradesService.CreateSellTradeAsync(user.Id, input.InvestmentWalletId, input.SelectedCompany.Ticker, input.Quantity, input.Price);
            }

            return this.Redirect("/Investments/All");
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (!this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var companies = await this.companiesService.GetAllCompaniesAsync();
            var result = await this.tradesService.GetTradeInfoForEdit(user.Id, id);

            var model = new EditTradeInputModel
            {
                Id = result.Id,
                CreatedOn = result.CreatedOn,
                Quantity = result.Quantity,
                Type = Enum.Parse<TradeType>(result.Type.ToString()),
                Price = result.Price,
                InvestmentWallet = new InvestmentWalletIdNameAndCurrencyViewModel
                {
                    Id = result.InvestmentWallet.Id,
                    CurrencyCode = result.InvestmentWallet.CurrencyCode,
                    Name = result.InvestmentWallet.Name,
                },
                AllInvestmentWallets = result.AllInvestmentWallets.Select(iw => new InvestmentWalletIdNameAndCurrencyViewModel
                {
                    Id = iw.Id,
                    CurrencyCode = iw.CurrencyCode,
                    Name = iw.Name,
                }),
                SelectedCompany = new CompanyViewModel
                {
                    Name = result.SelectedCompany.Name,
                    Ticker = result.SelectedCompany.Ticker,
                },
                Companies = companies.Select(c => new CompanyViewModel
                {
                    Name = c.Name,
                    Ticker = c.Ticker,
                })
                .ToList(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTradeInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
            }

            var user = await this.userManager.GetUserAsync(this.User);

            await this.tradesService
                .Update(
                user.Id,
                input.Id,
                input.SelectedCompany.Ticker,
                input.InvestmentWalletId,
                input.Price,
                input.Quantity,
                input.CreatedOn);

            return this.Redirect($"/Investments/Trades/{input.InvestmentWalletId}");
        }
    }
}
