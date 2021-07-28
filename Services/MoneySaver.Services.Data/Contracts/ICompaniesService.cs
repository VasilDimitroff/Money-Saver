namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Models.Companies;

    public interface ICompaniesService
    {
        public Task<IEnumerable<GetCompanyDto>> GetAllCompaniesAsync();

        public Task<GetCompanyDto> GetCompanyByIdAsync(string id);

        public Task AddAsync(string ticker, string companyName);

        public Task<IEnumerable<CompanyExtendedDto>> GetAllWithDeletedAsync();

        public Task<string> EditAsync(string id, string ticker, string companyName);

        public Task<string> DeleteAsync(string id);

        public Task<string> UndeleteAsync(string id);

        public Task<bool> IsCompanyAlreadyExistAsync(string ticker);
    }
}
