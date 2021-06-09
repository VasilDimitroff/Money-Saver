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
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models;
    using MoneySaver.Services.Data.Models.Records;

    public class RecordsService : IRecordsService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWalletsService walletsService;

        public RecordsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string userId, int categoryId, int walletId, string description, decimal amount, RecordType type)
        {
            var category = await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            var wallet = await this.dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            if (type == RecordType.Expense)
            {
                amount = (-1) * amount;
            }

            Record record = new Record
            {
                Id = Guid.NewGuid().ToString(),
                Amount = amount,
                CategoryId = categoryId,
                CreatedOn = DateTime.UtcNow,
                Description = description,
                Type = type,
                WalletId = walletId,
            };

            wallet.MoneyAmount += amount;

            await this.dbContext.Records.AddAsync(record);
            await this.dbContext.SaveChangesAsync();

            return GlobalConstants.RecordSuccessfullyAdded;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByCategoryAsync(int walletId, int categoryId)
        {
            var category = await this.dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.WalletId == walletId);

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            var records = await this.dbContext.Records
                 .Where(r => r.CategoryId == categoryId && r.WalletId == walletId)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CreatedOn = r.CreatedOn.ToString(),
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = r.Wallet.Name,
                 })
                 .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(int walletId, DateTime startDate, DateTime endDate)
        {
            var wallet = await this.dbContext.Categories.FirstOrDefaultAsync(w => w.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            var records = await this.dbContext.Records
                 .Where(r => r.CreatedOn >= startDate && r.CreatedOn <= endDate && r.WalletId == walletId)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CreatedOn = r.CreatedOn.ToString(),
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = r.Wallet.Name,
                 })
                 .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string keyword, int walletId)
        {
            var wallet = await this.dbContext.Categories.FirstOrDefaultAsync(w => w.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            var records = await this.dbContext.Records
                 .Where(r => r.Description.Contains(keyword) && r.WalletId == walletId)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CreatedOn = r.CreatedOn.ToString(),
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = r.Wallet.Name,
                 })
                 .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByWalletAsync(int walletId)
        {
            var wallet = await this.dbContext.Categories.FirstOrDefaultAsync(w => w.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            var records = await this.dbContext.Records
                 .Where(r => r.WalletId == walletId)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CreatedOn = r.CreatedOn.ToString(),
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = r.Wallet.Name,
                 })
                 .ToListAsync();

            return records;
        }

        public async Task<string> RemoveAsync(string recordId)
        {
            Record record = await this.dbContext.Records.FirstOrDefaultAsync(r => r.Id == recordId);

            if (record == null)
            {
                throw new ArgumentException(GlobalConstants.RecordNotExist);
            }

            this.dbContext.Records.Remove(record);

            await this.dbContext.SaveChangesAsync();

            return GlobalConstants.SuccessfullyRemovedRecord;
        }

        public async Task<RecordInfoDto> GetRecordById(string recordId)
        {
            RecordInfoDto record = await this.dbContext.Records
                .Select(r => new RecordInfoDto
                {
                    Id = r.Id,
                    Amount = r.Amount,
                    Category = r.Category.Name,
                    CreatedOn = r.CreatedOn.ToString(),
                    Description = r.Description,
                    Type = r.Type.ToString(),
                    Wallet = r.Wallet.Name,
                })

                .FirstOrDefaultAsync(r => r.Id == recordId);

            if (record == null)
            {
                throw new ArgumentException(GlobalConstants.RecordNotExist);
            }

            return record;
        }
    }
}
