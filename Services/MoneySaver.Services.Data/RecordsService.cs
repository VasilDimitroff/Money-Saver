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

    public class RecordsService : IRecordsService
    {
        private readonly ApplicationDbContext dbContext;

        public RecordsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string description, decimal amount, int categoryId, string type, int walletId)
        {
            Wallet targetWallet = await this.GetWalletByIdAsync(walletId);

            RecordType recordType;
            bool isTypeParsed = Enum.TryParse<RecordType>(type, out recordType);

            Category targetCategory = await this.dbContext.Categories
                .FirstOrDefaultAsync(category => category.Id == categoryId);

            if (targetCategory == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfCategory);
            }

            if (targetWallet == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfWallet);
            }

            if (!isTypeParsed)
            {
                throw new ArgumentException(GlobalConstants.InvalidRecordType);
            }

            Record record = new Record()
            {
                Description = description,
                Amount = amount,
                Category = targetCategory,
                Type = recordType,
                Wallet = targetWallet,
            };

            await this.dbContext.Records.AddAsync(record);
            await this.dbContext.SaveChangesAsync();
            return string.Format(GlobalConstants.SuccessfullyAddedRecord, description, recordType, amount, targetCategory);
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByCategoryAsync(string categoryId)
        {
            Wallet targetWallet = await this.GetWalletByNameAsync(userId, wallet);

            Category targetCategory = await this.dbContext.Categories
                .FirstOrDefaultAsync(categ => categ.Name.ToLower() == category.ToLower()
                && categ.Records.Any(r => r.Wallet == targetWallet)
                && categ.Records.Any(r => r.Wallet.ApplicationUserId == userId));

            if (targetWallet == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfWallet);
            }

            if (targetCategory == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfCategory);
            }

            var records = await this.dbContext.Records
                .Where(record => record.Category.Name.ToLower() == category.ToLower())
                .Select(record => new RecordInfoDto
                {
                    Category = record.Category.Name,
                    Amount = record.Amount,
                    Description = record.Description,
                    Type = record.Type.ToString(),
                    Wallet = record.Wallet.Name,
                })
                .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByDateRangeAsync(string userId, DateTime? startDate, DateTime endDate, string wallet)
        {
            Wallet targetWallet = await this.GetWalletByNameAsync(userId, wallet);

            if (targetWallet == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfWallet);
            }

            if (startDate == null)
            {
               startDate = endDate.AddDays(-7);
            }

            var records = await this.dbContext.Records
                .Where(record => record.CreatedOn >= startDate
                    && record.CreatedOn <= endDate
                    && record.Wallet.ApplicationUserId == userId
                    && record.Wallet == targetWallet)
                .Select(record => new RecordInfoDto
                {
                    Category = record.Category.Name,
                    Amount = record.Amount,
                    Description = record.Description,
                    Type = record.Type.ToString(),
                    Wallet = record.Wallet.Name,
                })
                .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<RecordInfoDto>> GetRecordsByKeywordAsync(string userId, string keyword, string wallet)
        {
            Wallet targetWallet = await this.GetWalletByNameAsync(userId, wallet);

            if (targetWallet == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfWallet);
            }

            if (keyword == null)
            {
                var records = await this.dbContext.Records
                    .Where(record => record.Wallet.ApplicationUserId == userId && record.Wallet == targetWallet)
                    .Select(record => new RecordInfoDto
                    {
                        Category = record.Category.Name,
                        Amount = record.Amount,
                        Description = record.Description,
                        Type = record.Type.ToString(),
                        Wallet = record.Wallet.Name,
                    })
                    .ToListAsync();

                return records;
            }

            var filteredRecords = await this.dbContext.Records
                    .Where(x => x.Description
                        .ToLower()
                        .Contains(keyword.ToLower())
                            && x.Wallet == targetWallet
                            && x.Wallet.ApplicationUserId == userId)
                    .Select(record => new RecordInfoDto
                    {
                        Category = record.Category.Name,
                        Amount = record.Amount,
                        Description = record.Description,
                        Type = record.Type.ToString(),
                        Wallet = record.Wallet.Name,
                    })
                    .ToListAsync();

            return filteredRecords;
        }

        public async Task<string> RemoveAsync(string userId, int id, string wallet)
        {
            Wallet targetWallet = await this.GetWalletByNameAsync(userId, wallet);

            if (targetWallet == null)
            {
                throw new ArgumentNullException(GlobalConstants.NullValueOfWallet);
            }

            Record targetRecord = await this.dbContext.Records
                .FirstOrDefaultAsync(record => record.Id == id
                    && record.Wallet == targetWallet
                    && record.Wallet.ApplicationUserId == userId);

            if (targetRecord == null)
            {
                throw new ArgumentException(GlobalConstants.InvalidRecordId);
            }

            string successfullMessage = GlobalConstants.SuccessfullyRemovedRecord;

            return successfullMessage;
        }

        private async Task<Wallet> GetWalletByIdAsync(int walletId)
        {
            Wallet targetWallet = await this.dbContext.Wallets
              .FirstOrDefaultAsync(w => w.Id == walletId);

            return targetWallet;
        }
    }
}
