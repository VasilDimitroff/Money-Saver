﻿namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MoneySaver.Services.Data.Models.Categories;
    using MoneySaver.Services.Data.Models.Records;
    using MoneySaver.Services.Data.Models.Wallets;

    public interface IWalletsService
    {
        public Task<IEnumerable<WalletInfoDto>> GetWalletsAsync(string userId);

        public Task<WalletCategoriesDto> GetWalletWithCategoriesAsync(int walletId);

        public Task<string> AddAsync(string userId, string name, decimal initialMoney, string currencyName);

        public Task<string> RemoveAsync(int walletId);

        public Task<string> GetWalletNameAsync(int walletId);

        public Task<int> GetWalletIdByRecordIdAsync(string recordId);

        public Task<IEnumerable<CategoryBasicInfoDto>> GetWalletCategoriesAsync(int walletId);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(DateTime startDate, DateTime endDate, int walletId);
    }
}
