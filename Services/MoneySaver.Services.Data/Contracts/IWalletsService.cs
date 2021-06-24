namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models.Categories;
    using MoneySaver.Services.Data.Models.Records;
    using MoneySaver.Services.Data.Models.Wallets;

    public interface IWalletsService
    {
        public Task<WalletCategoriesDto> GetWalletWithCategoriesAsync(int walletId);

        public Task<string> AddAsync(string userId, string name, decimal initialMoney, int currencyId);

        public Task<string> RemoveAsync(int walletId);

        public Task EditAsync(string userId, int walletId, string name, decimal amount, int currencyId);

        public Task<IEnumerable<RecordInfoDto>> GetAllRecordsAsync(int page, int walletId, int itemsPerPage);

        public Task<EditWalletDto> GetWalletInfoForEditAsync(string userId, int walletId);

        public Task<IEnumerable<AllWalletsDto>> GetAllWalletsAsync(string userId);

        public Task<string> GetWalletNameAsync(int walletId);

        public Task<int> GetWalletIdByRecordIdAsync(string recordId);

        public Task<int> GetWalletIdByCategoryIdAsync(int categoryId);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string keyword, int walletId, int page, int itemsPerPage);

        public Task<IEnumerable<CategoryBasicInfoDto>> GetWalletCategoriesAsync(int walletId);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(DateTime startDate, DateTime endDate, int walletId, int page, int itemsPerPage);

        public Task<WalletDetailsDto> GetWalletDetailsAsync(string userId, int walletId);

        public Task<bool> IsUserOwnWalletAsync(string userId, int walletId);

        public int GetCount(int walletId);

        public int GetSearchRecordsCount(string searchTerm, int walletId);

        public int GetDateSortedRecordsCount(DateTime startDate, DateTime endDate, int walletId);
    }
}
