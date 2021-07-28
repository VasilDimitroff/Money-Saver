namespace MoneySaver.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Common;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.ViewModels.Companies;

    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly ICompaniesService companiesService;

        public CompaniesController(ICompaniesService companiesService)
        {
            this.companiesService = companiesService;
        }

        public IActionResult Add(int investmentWalletId = 0)
        {
            this.ViewBag.InvestmentWalletId = investmentWalletId;
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCompanyInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewBag.InvestmentWalletId = input.InvestmentWalletId;
                this.TempData["InvalidAddModel"] = "Some of fields you entered are invalid!";
                return this.View(input);
            }

            if (await this.companiesService.IsCompanyAlreadyExistAsync(input.Ticker))
            {
                this.TempData["CompanyExist"] = $"Company with ticker {input.Ticker} already exists!";
                this.ViewBag.InvestmentWalletId = input.InvestmentWalletId;
                return this.View(input);
            }

            await this.companiesService.AddAsync(input.Ticker, input.CompanyName);

            if (input.InvestmentWalletId == 0)
            {
                if (this.User.IsInRole(GlobalConstants.AdministratorRoleName))
                {
                    this.TempData["SuccessfullAddedCompany"] =
                        $"Successfilly added company with name {input.CompanyName} and ticker {input.Ticker}!";

                    List<CompanyViewModel> companies = await this.GetAllCompaniesWithDeletedAsync();

                    return this.View("Index", companies);
                }

                return this.Redirect($"/Investments/AllInvestments");
            }

            return this.Redirect($"/Trades/Add?investmentWalletId={input.InvestmentWalletId}");
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<CompanyViewModel> companies = await this.GetAllCompaniesWithDeletedAsync();

                return this.View(companies);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var companyDto = await this.companiesService.GetCompanyByIdAsync(id);
                var company = new EditCompanyInputModel
                {
                    Id = companyDto.Id,
                    Name = companyDto.Name,
                    Ticker = companyDto.Ticker,
                };

                return this.View(company);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            } 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Edit(EditCompanyInputModel company)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    this.TempData["InvalidEditModel"] = $"Some of fields you entered are invalid!";
                    return this.View(company);
                }

                string editedCompanyName = await this.companiesService.EditAsync(company.Id, company.Ticker, company.Name);
                
                this.TempData["SuccessfullUpdatedCompany"] = $"Successfully updated company {editedCompanyName}!";

                List<CompanyViewModel> companies = await this.GetAllCompaniesWithDeletedAsync();

                return this.View("Index", companies);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var companyDto = await this.companiesService.GetCompanyByIdAsync(id);
                var companyModel = new CompanyViewModel()
                {
                    Id = companyDto.Id,
                    Name = companyDto.Name,
                    Ticker = companyDto.Ticker,
                };

                return this.View(companyModel);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var companyName = await this.companiesService.DeleteAsync(id);

                this.TempData["SuccessfullDeletedCompany"] = $"Successfully deleted company {companyName}!";

                List<CompanyViewModel> companies = await this.GetAllCompaniesWithDeletedAsync();

                return this.View("Index", companies);
            }
            catch (Exception ex)
            {
                return this.Redirect($"/Home/Error?message={ex.Message}");
            }    
        }

        private async Task<List<CompanyViewModel>> GetAllCompaniesWithDeletedAsync()
        {
            var companiesDto = await this.companiesService.GetAllWithDeletedAsync();
            var companies = new List<CompanyViewModel>();

            companies = companiesDto.Select(c => new CompanyViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Ticker = c.Ticker,
                CreatedOn = c.CreatedOn,
                TradesCount = c.TradesCount,
                IsDeleted = c.IsDeleted,
            })
                .ToList();
            return companies;
        }
    }
}
