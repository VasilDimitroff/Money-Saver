namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models.Companies;

    public interface ICompaniesService
    {
        public Task<IEnumerable<GetCompanyDto>> GetAllCompaniesAsync();

        public Task<GetCompanyDto> GetCompanyByTickerAsync(string ticker);
    }
}