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
    using MoneySaver.Services.Data.Models.Categories;
    using MoneySaver.Services.Data.Models.Records;

    public class RecordsService : IRecordsService
    {
        private readonly ApplicationDbContext dbContext;

        public RecordsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(int categoryId, string description, decimal amount, string type, DateTime? createdOn)
        {
            Category category = await this.GetCategoryByIdAsync(categoryId);

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            RecordType recordType = this.ParseRecordType(type);

            if (recordType == 0)
            {
                throw new ArgumentException(GlobalConstants.InvalidRecordType);
            }

            Wallet wallet = await this.dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == category.WalletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            amount = Math.Abs(amount);
            string amountAsString = amount.ToString("f2");
            amount = decimal.Parse(amountAsString);

            if (recordType == RecordType.Expense)
            {
                amount = (-1) * amount;
            }

            Record record = new Record
            {
                Id = Guid.NewGuid().ToString(),
                Amount = amount,
                CategoryId = categoryId,
                CreatedOn = createdOn == null || createdOn == default ? DateTime.UtcNow : createdOn.Value,
                ModifiedOn = DateTime.UtcNow,
                Description = description,
                Type = recordType,
            };

            await this.EditWalletAmountAsync(wallet.Id, amount);

           // wallet.MoneyAmount += amount;
            await this.dbContext.Records.AddAsync(record);
            await this.dbContext.SaveChangesAsync();

            return GlobalConstants.RecordSuccessfullyAdded;
        }

        public async Task<string> RemoveAsync(string recordId)
        {
            Record record = await this.dbContext.Records.FirstOrDefaultAsync(r => r.Id == recordId);

            if (record == null)
            {
                throw new ArgumentException(GlobalConstants.RecordNotExist);
            }

            Wallet wallet = await this.dbContext.Wallets
                .FirstOrDefaultAsync(w => w.Categories.Any(c => c.Records.Any(r => r.Id == recordId)));

            int walletId = wallet.Id;
            decimal amount = record.Amount;

            if (record.Type == RecordType.Expense)
            {
                 amount = Math.Abs(amount);
            }
            else if (record.Type == RecordType.Income)
            {
                 amount *= -1;
            }

            this.dbContext.Records.Remove(record);

            await this.dbContext.SaveChangesAsync();

            await this.EditWalletAmountAsync(walletId, amount);

            return GlobalConstants.SuccessfullyRemovedRecord;
        }

        public async Task<EditRecordInfoDto> GetRecordWithAllCategories(string recordId, int walletId)
        {
            var allCategories = await this.dbContext.Categories
                .Where(c => c.WalletId == walletId)
                .ToListAsync();

            EditRecordInfoDto record = await this.dbContext.Records
                .Select(r => new EditRecordInfoDto
                {
                    Id = r.Id,
                    Amount = r.Amount,
                    CategoryId = r.CategoryId,
                    Description = r.Description,
                    ModifiedOn = r.ModifiedOn.HasValue ? r.ModifiedOn : null,
                    CreatedOn = r.CreatedOn,
                    Type = r.Type,
                    WalletName = r.Category.Wallet.Name,
                    Categories = allCategories.Select(c => new CategoryBasicInfoDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                    })
                    .ToList(),
                })
                .FirstOrDefaultAsync(r => r.Id == recordId);

            if (record == null)
            {
                throw new ArgumentException(GlobalConstants.RecordNotExist);
            }

            return record;
        }

        public async Task<string> UpdateRecord(string recordId, int categoryId, int walletId, string description, decimal oldAmount, decimal newAmount, string type, DateTime createdOn)
        {
            newAmount = Math.Abs(newAmount);
            string newAmountAsString = newAmount.ToString("f2");
            newAmount = decimal.Parse(newAmountAsString);

            Wallet wallet = await this.dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            Record record = await this.dbContext.Records.FirstOrDefaultAsync(r => r.Id == recordId);

            if (record == null)
            {
                throw new ArgumentException(GlobalConstants.RecordNotExist);
            }

            Category category = await this.GetCategoryByIdAsync(categoryId);

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            if (createdOn == default)
            {
                createdOn = DateTime.UtcNow;
            }

            RecordType recordInputType = this.ParseRecordType(type);

            if (recordInputType == 0)
            {
                throw new ArgumentException(GlobalConstants.InvalidRecordType);
            }

            oldAmount = Math.Abs(oldAmount);

            if (recordInputType == record.Type)
            {
                if (recordInputType == RecordType.Income)
                {
                    oldAmount *= -1;
                }
            }

            if (recordInputType != record.Type)
            {
                if (recordInputType == RecordType.Expense)
                {
                    oldAmount *= -1;
                }
            }

            await this.EditWalletAmountAsync(walletId, oldAmount);

            if (recordInputType == RecordType.Expense)
            {
                newAmount = -1 * newAmount;
            }

            record.Description = description;
            record.Amount = newAmount;
            record.CreatedOn = createdOn;
            record.ModifiedOn = DateTime.UtcNow;
            record.Type = recordInputType;
            record.CategoryId = categoryId;

            await this.EditWalletAmountAsync(wallet.Id, newAmount);
            this.dbContext.SaveChanges();

            return GlobalConstants.RecordSuccessfullyUpdated;
        }

        public async Task<string> UpdateRecordWORKFINE(string recordId, int categoryId, int walletId, string description, decimal oldAmount, decimal newAmount, string type, DateTime modifiedOn)
        {
            newAmount = Math.Abs(newAmount);
            string newAmountAsString = newAmount.ToString("f2");
            newAmount = decimal.Parse(newAmountAsString);

            Wallet wallet = await this.dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            Record record = await this.dbContext.Records.FirstOrDefaultAsync(r => r.Id == recordId);

            if (record == null)
            {
                throw new ArgumentException(GlobalConstants.RecordNotExist);
            }

            Category category = await this.GetCategoryByIdAsync(categoryId);

            if (category == null)
            {
                throw new ArgumentException(GlobalConstants.UnexistingCategory);
            }

            RecordType recordInputType = this.ParseRecordType(type);

            if (recordInputType == 0)
            {
                throw new ArgumentException(GlobalConstants.InvalidRecordType);
            }

            string currentRecordId = record.Id;
            DateTime currentRecordCreatedDate = record.CreatedOn;

            oldAmount = Math.Abs(oldAmount);

            if (recordInputType == record.Type)
            {
                if (recordInputType == RecordType.Income)
                {
                    oldAmount *= -1;
                }
            }

            await this.RemoveAsync(recordId);

            if (recordInputType == RecordType.Expense)
            {
                newAmount = -1 * newAmount;
            }

            Record newRecord = new Record
            {
                Id = currentRecordId,
                CreatedOn = currentRecordCreatedDate,
                ModifiedOn = modifiedOn,
                Type = recordInputType,
                Description = description,
                CategoryId = categoryId,
                Amount = newAmount,
            };

            await this.dbContext.Records.AddAsync(newRecord);
            await this.EditWalletAmountAsync(wallet.Id, newAmount);
            this.dbContext.SaveChanges();

            return GlobalConstants.RecordSuccessfullyUpdated;
        }

        public async Task EditWalletAmountAsync(int walletId, decimal amount)
        {
            var wallet = await this.dbContext.Wallets
                .FirstOrDefaultAsync(x => x.Id == walletId);

            if (wallet == null)
            {
                throw new ArgumentException(GlobalConstants.WalletNotExist);
            }

            wallet.MoneyAmount += amount;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsUserOwnRecordAsync(string userId, string recordId)
        {
            var record = await this.dbContext.Records
                .Where(r => r.Id == recordId && r.Category.Wallet.ApplicationUserId == userId)
                .FirstOrDefaultAsync();

            if (record == null)
            {
                return false;
            }

            return true;
        }

        private RecordType ParseRecordType(string type)
        {
            bool isTypeParsed = Enum.TryParse<RecordType>(type, out RecordType recordType);

            if (!isTypeParsed)
            {
                return 0;
            }

            return recordType;
        }

        private async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            var category = await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

            return category;
        }
    }
}
