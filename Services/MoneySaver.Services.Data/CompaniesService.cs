namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;
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
                    Id = c.Id,
                })
                .ToListAsync();

            return companies;
        }

        public async Task<GetCompanyDto> GetCompanyByIdAsync(string companyId)
        {
            var company = await this.dbContext.Companies.FindAsync(companyId);

            if (string.IsNullOrWhiteSpace(companyId) || company == null)
            {
                throw new ArgumentException(GlobalConstants.InvalidCompanyId);
            }

            var companyDto = new GetCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Ticker = company.Ticker,
            };

            return companyDto;
        }

        public async Task AddAsync(string ticker, string companyName)
        {
            ticker = ticker.ToUpper();

            var company = new Company
            {
                Id = Guid.NewGuid().ToString(),
                Ticker = ticker,
                Name = companyName,
                CreatedOn = DateTime.UtcNow,
            };

            await this.dbContext.Companies.AddAsync(company);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsCompanyAlreadyExistAsync(string ticker)
        {
            var company = await this.dbContext.Companies.FirstOrDefaultAsync(c => c.Ticker.ToUpper() == ticker.ToUpper());

            if (company != null)
            {
                return true;
            }

            return false;
        }
    }
}
