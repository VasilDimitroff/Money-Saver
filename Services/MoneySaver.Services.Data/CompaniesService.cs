namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Common;
    using MoneySaver.Data;
    using MoneySaver.Data.Common.Repositories;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models.Companies;

    public class CompaniesService : ICompaniesService
    {
        private readonly IDeletableEntityRepository<Company> companyRepository;

        public CompaniesService(IDeletableEntityRepository<Company> companyRepository)
        {
            this.companyRepository = companyRepository;
        }

        public async Task<IEnumerable<GetCompanyDto>> GetAllCompaniesAsync()
        {
            var companies = await this.companyRepository.All()
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
            var company = await this.companyRepository.All().FirstOrDefaultAsync(c => c.Id == companyId);

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

            await this.companyRepository.AddAsync(company);
            await this.companyRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CompanyExtendedDto>> GetAllWithDeletedAsync()
        {
            var companies = await this.companyRepository.AllWithDeleted()
                 .Select(c => new CompanyExtendedDto
                 {
                     Id = c.Id,
                     Name = c.Name,
                     Ticker = c.Ticker,
                     CreatedOn = c.CreatedOn,
                     TradesCount = c.Trades.Count(),
                     IsDeleted = c.IsDeleted,
                 })
                 .ToListAsync();

            return companies;
        }

        public async Task<string> EditAsync(string id, string ticker, string companyName)
        {
            if (!await this.IsCompanyExistAsync(id))
            {
                throw new ArgumentException(GlobalConstants.InvalidCompanyId);
            }

            var companyWithId = await this.companyRepository
                .AllWithDeleted()
                .FirstOrDefaultAsync(c => c.Id == id);

            var companyWithThisTicker = await this.companyRepository.AllWithDeleted().FirstOrDefaultAsync(c => c.Ticker == ticker);

            if (companyWithThisTicker != null)
            {
                if (companyWithId.Id != companyWithThisTicker.Id)
                {
                    throw new ArgumentException(GlobalConstants.CompanyWithThisTickerAlreadyExists);
                }
            }

            companyWithId.Ticker = ticker.ToUpper();
            companyWithId.Name = companyName;

            this.companyRepository.Update(companyWithId);
            await this.companyRepository.SaveChangesAsync();

            return companyWithId.Name;
        }

        public async Task<string> DeleteAsync(string id)
        {
            if (!await this.IsCompanyExistAsync(id))
            {
                throw new ArgumentException(GlobalConstants.InvalidCompanyId);
            }

            var company = await this.companyRepository
                .All()
                .FirstOrDefaultAsync(c => c.Id == id);

            this.companyRepository.Delete(company);
            await this.companyRepository.SaveChangesAsync();

            return company.Name;
        }

        public async Task<string> UndeleteAsync(string id)
        {
            var company = await this.companyRepository
               .AllWithDeleted()
               .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                throw new ArgumentException(GlobalConstants.InvalidCompanyId);
            }

            if (!company.IsDeleted)
            {
                throw new ArgumentException(GlobalConstants.CompanyNotMarkedAsDeleted);
            }

            this.companyRepository.Undelete(company);
            await this.companyRepository.SaveChangesAsync();

            return company.Name;
        }

        public async Task<bool> IsCompanyAlreadyExistAsync(string ticker)
        {
            var company = await this.companyRepository
                .All()
                .FirstOrDefaultAsync(c => c.Ticker.ToUpper() == ticker.ToUpper());

            if (company != null)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> IsCompanyExistAsync(string id)
        {
            var company = await this.companyRepository
                .All()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company != null)
            {
                return true;
            }

            return false;
        }
    }
}
