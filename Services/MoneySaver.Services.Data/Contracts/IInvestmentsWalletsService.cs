﻿namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models.Currencies;
    using MoneySaver.Services.Data.Models.InvestmentWallets;

    public interface IInvestmentsWalletsService
    {
        public Task AddAsync(string userId, string name, int currencyId);

        public Task EditAsync(string userId, int investmentWalletId, int currencyId, string name);

        public Task RemoveAsync(string userId, int investmentWalletId);

        public Task<IEnumerable<InvestmentWalletDto>> GetAllAsync(string userId);

        public Task<InvestmentWalletTradesDto> GetTradesAsync(string userId, int investmentWalletId, int page, int itemsPerPage = 12);

        public Task<string> GetInvestmentWalletNameAsync(int investmentWalletId);

        public Task<CurrencyInfoDto> GetInvestmentCurrencyAsync(int investmentWalletId);

        public int GetTradesCount(int investmentWalletId);

        public Task<IEnumerable<CompanyHoldingsDto>> GetHoldingsAsync(string userId, int investmentWalletId);
    }
}
