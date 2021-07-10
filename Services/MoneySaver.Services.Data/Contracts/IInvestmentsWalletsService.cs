namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models.InvestmentWallets;

    public interface IInvestmentsWalletsService
    {
        public Task AddAsync(string userId, string name, int currencyId);

        public Task EditAsync(string userId, int investmentWalletId);

        public Task RemoveAsync(string userId, int investmentWalletId);

        public Task<IEnumerable<InvestmentWalletDto>> GetAllAsync(string userId);
    }
}
