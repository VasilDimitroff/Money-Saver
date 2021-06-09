namespace MoneySaver.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Models.Records;

    public interface IRecordsService
    {
        public Task<string> AddAsync(string userId, int categoryId, int walletId, string description, decimal amount, RecordType type);

        public Task<string> RemoveAsync(string recordId);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(int walletId, DateTime startDate, DateTime endDate);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByCategoryAsync(int walletId, int categoryId);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string keyword, int walletId);

        public Task<IEnumerable<RecordInfoDto>> GetRecordsByWalletAsync(int walletId);

        public Task<RecordInfoDto> GetRecordById(string recordId);
    }
}
