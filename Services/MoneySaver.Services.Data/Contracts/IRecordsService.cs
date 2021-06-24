namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models.Categories;
    using MoneySaver.Services.Data.Models.Records;

    public interface IRecordsService
    {
        public Task<string> AddAsync(int categoryId, string description, decimal amount, string type, DateTime? createdOn);

        public Task<string> RemoveAsync(string recordId);

        public Task<IEnumerable<RecordInfoDto>> GetPagedRecordsByWalletIdAsync(int walletId);

        public Task<EditRecordInfoDto> GetRecordWithAllCategories(string recordId, int walletId);

        public Task<string> UpdateRecord(string recordId, int categoryId, int walletId, string description, decimal oldAmount, decimal newAmount, string type, DateTime createdOn);

        public Task EditWalletAmountAsync(int walletId, decimal amount);

        public Task<bool> IsUserOwnRecordAsync(string userId, string recordId);
    }
}
