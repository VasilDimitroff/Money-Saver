namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models.Companies;

    public class CompaniesService : ICompaniesService
    {
        private readonly ApplicationDbContext dbContext;

        public CompaniesService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<GetCompanyDto>> GetAllCompaniesAsync()
        {
            var companies = await this.dbContext.Companies
                .Select(c => new GetCompanyDto
                {
                    Name = c.Name,
                    Ticker = c.Ticker,
                })
                .ToListAsync();

            return companies;
        }

        public async Task<GetCompanyDto> GetCompanyByTickerAsync(string ticker)
        {
            var company = await this.dbContext.Companies.FindAsync(ticker);

            if (string.IsNullOrWhiteSpace(ticker) || company == null)
            {
                throw new ArgumentException(GlobalConstants.InvalidCompanyTicker);
            }

            var companyDto = new GetCompanyDto()
            {
                Name = company.Name,
                Ticker = company.Ticker,
            };

            return companyDto;
        }
    }
}
