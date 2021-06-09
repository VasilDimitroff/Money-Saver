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
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models;

    public class WalletsService : IWalletsService
    {
        private readonly ApplicationDbContext dbContext;

        public WalletsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> AddAsync(string userId, string name, decimal initialMoney, string currencyName)
        {
            Wallet wallet = await this.GetWalletAsync(userId, name);

            if (wallet != null)
            {
                throw new ArgumentException(GlobalConstants.WalletAlreadyExist);
            }

            Currency currency = await this.GetCurrencyAsync(currencyName);

            if (currency == null)
            {
                throw new ArgumentException(GlobalConstants.InvalidCurrency);
            }

            Wallet newWallet = new Wallet
            {
                ApplicationUserId = userId,
                Name = name,
                MoneyAmount = initialMoney,
                Currency = currency,
            };

            await this.dbContext.AddAsync(newWallet);

            await this.dbContext.SaveChangesAsync();

            return string.Format(GlobalConstants.WalletSuccessfullyAdded, newWallet.Name);
        }

        public async Task<IEnumerable<CategoryWalletInfoDto>> GetWalletCategoriesAsync(int walletId)
        {
            var categories = await this.dbContext.WalletsCategories
                .Where(wc => wc.WalletId == walletId)
                .Select(wc => new CategoryWalletInfoDto
                {
                    CategoryName = wc.Category.Name,
                    WalletName = wc.Wallet.Name,
                })
                .ToListAsync();

            return categories;
        }

        public async Task<IEnumerable<WalletInfoDto>> GetWalletsAsync(string userId)
        {
            var wallets = await this.dbContext.Wallets
                .Select(w => new WalletInfoDto
                {
                    Id = w.Id,
                    Currency = w.Currency.Code,
                    MoneyAmount = w.MoneyAmount,
                    Name = w.Name,
                    Categories = w.Categories.Select(c => new CategoryWalletInfoDto
                    {
                        CategoryName = c.Category.Name,
                        WalletName = c.Wallet.Name,
                    }),
                })
                .ToListAsync();

            return wallets;
        }

        //TODO: CHECK IF IT WORKS CORRECTLY!!!!
        public async Task<string> RemoveAsync(int walletId)
        {
            var walletsCategoriesForDelete = await this.dbContext.WalletsCategories
                .Where(wc => wc.WalletId == walletId)
                .ToListAsync();

            foreach (var wc in walletsCategoriesForDelete)
            {
                Category categoryForDelete = await this.dbContext.Categories
                    .FirstOrDefaultAsync(x => x.Id == wc.CategoryId);

                this.dbContext.Categories.Remove(categoryForDelete);

                var budgetsForDelete = await this.dbContext.Budgets.Where(b => b.WalletId == walletId).ToListAsync();

                foreach (var budget in budgetsForDelete)
                {
                    this.dbContext.Budgets.Remove(budget);
                }

                this.dbContext.WalletsCategories.Remove(wc);
            }

            var walletForDelete = await this.dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == walletId);

            this.dbContext.Remove(walletForDelete);

            await this.dbContext.SaveChangesAsync();

            return string.Format(GlobalConstants.WalletSuccessfullyRemoved, walletForDelete.Name);
        }

        public async Task<WalletInfoDto> GetWalletByIdAsync(int walletId)
        {
            var wallet = await this.dbContext.Wallets
                .Where(w => w.Id == walletId)
                .Select(w => new WalletInfoDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Currency = w.Currency.Code,
                    MoneyAmount = w.MoneyAmount,
                    Categories = w.Categories.Select(c => new CategoryWalletInfoDto
                    {
                        CategoryName = c.Category.Name,
                        WalletName = c.Wallet.Name,
                    }),
                })
                .FirstOrDefaultAsync();

            return wallet;
        }

        private async Task<Wallet> GetWalletAsync(string userId, string walletName)
        {
            Wallet wallet = await this.dbContext.Wallets
                .FirstOrDefaultAsync(w => w.Name == walletName && w.ApplicationUserId == userId);

            return wallet;
        }

        private async Task<Currency> GetCurrencyAsync(string currencyName)
        {
           Currency currency = await this.dbContext.Currencies.FirstOrDefaultAsync(x => x.Name == currencyName);

           return currency;
        }
    }
}
