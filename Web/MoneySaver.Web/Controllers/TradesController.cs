namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels.Trades;

    [Authorize]
    public class TradesController : Controller
    {
        private readonly ICompaniesService companiesService;

        public TradesController(ICompaniesService companiesService)
        {
            this.companiesService = companiesService;
        }

        public async Task<IActionResult> Buy()
        {
            var companies = await this.companiesService.GetAllCompaniesAsync();
            var model = new BuyStockInputModel
            {
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
        public async Task<IActionResult> Buy(BuyStockInputModel input)
        {
            var companies = await this.companiesService.GetAllCompaniesAsync();
            var model = new BuyStockInputModel
            {
                Companies = companies.Select(c => new CompanyViewModel
                {
                    Name = c.Name,
                    Ticker = c.Ticker,
                })
                .ToList(),
            };

            return this.View(model);
        }
    }
}
