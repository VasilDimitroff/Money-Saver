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

        public RecordsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(int categoryId, string description, decimal amount, string type)
        {
            var category = await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

            bool isTypeParsed = Enum.TryParse<RecordType>(type, out RecordType recordType);

            if (!isTypeParsed)
            {
                throw new ArgumentException(GlobalConstants.InvalidRecordType);
            }

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            Wallet wallet = await this.dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == category.WalletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            if (recordType == RecordType.Expense)
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
                Type = recordType,
            };

            wallet.MoneyAmount += amount;

            await this.dbContext.Records.AddAsync(record);
            await this.dbContext.SaveChangesAsync();

            return GlobalConstants.RecordSuccessfullyAdded;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByCategoryAsync(int categoryId)
        {
            var category = await this.dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            var records = await this.dbContext.Records
                 .Where(r => r.CategoryId == categoryId)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CategoryId = r.CategoryId,
                     CreatedOn = r.CreatedOn,
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = r.Category.Wallet.Name,
                     Currency = r.Category.Wallet.Currency.Code,
                 })
                 .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {

            var records = await this.dbContext.Records
                 .Where(r => r.CreatedOn >= startDate && r.CreatedOn <= endDate && r.Category.Wallet.ApplicationUserId == userId)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CategoryId = r.CategoryId,
                     CreatedOn = r.CreatedOn,
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = r.Category.Wallet.Name,
                     Currency = r.Category.Wallet.Currency.Code,
                 })
                 .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string keyword, string userId)
        {
            var records = await this.dbContext.Records
                 .Where(r => r.Description.Contains(keyword) && r.Category.Wallet.ApplicationUserId == userId)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CategoryId = r.CategoryId,
                     CreatedOn = r.CreatedOn,
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = r.Category.Wallet.Name,
                     Currency = r.Category.Wallet.Currency.Code,
                 })
                 .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByWalletAsync(int walletId)
        {
            var wallet = await this.dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            var records = await this.dbContext.Records
                 .Where(r => r.Category.WalletId == walletId)
                 .Select(r => new RecordInfoDto
                 {
                     Id = r.Id,
                     Amount = (r.Type == RecordType.Income) ? r.Amount : Math.Abs(r.Amount) * (-1),
                     Category = r.Category.Name,
                     CategoryId = r.CategoryId,
                     CreatedOn = r.CreatedOn,
                     Description = r.Description,
                     Type = r.Type.ToString(),
                     Wallet = wallet.Name,
                     Currency = r.Category.Wallet.Currency.Code,
                 })
                 .OrderByDescending(x => x.CreatedOn)
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
                    CategoryId = r.CategoryId,
                    CreatedOn = r.CreatedOn,
                    Description = r.Description,
                    Type = r.Type.ToString(),
                    Wallet = r.Category.Wallet.Name,
                    Currency = r.Category.Wallet.Currency.Code,
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
