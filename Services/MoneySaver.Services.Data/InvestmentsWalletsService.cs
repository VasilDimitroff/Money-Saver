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
    using MoneySaver.Services.Data.Models.Companies;
    using MoneySaver.Services.Data.Models.Currencies;
    using MoneySaver.Services.Data.Models.InvestmentWallets;
    using MoneySaver.Services.Data.Models.Trades;

    public class InvestmentsWalletsService : IInvestmentsWalletsService
    {
        private readonly ApplicationDbContext dbContext;

        public InvestmentsWalletsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(string userId, string name, int currencyId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(GlobalConstants.EmptyInvestmentsWalletName);
            }

            var currency = await this.dbContext.Currencies.FirstOrDefaultAsync(c => c.Id == currencyId);

            if (currency == null)
            {
                throw new ArgumentException(GlobalConstants.CurrencyNotExist);
            }

            var investmentWallet = new InvestmentWallet
            {
                ApplicationUserId = userId,
                Name = name,
                CurrencyId = currencyId,
                CreatedOn = DateTime.UtcNow,
            };

            await this.dbContext.InvestmentWallets.AddAsync(investmentWallet);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task EditAsync(string userId, int investmentWalletId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(string userId, int investmentWalletId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InvestmentWalletDto>> GetAllAsync(string userId)
        {
            var investmentWallets = await this.dbContext.InvestmentWallets
                .Select(iw => new InvestmentWalletDto
                {
                    Id = iw.Id,
                    Name = iw.Name,
                    CreatedOn = iw.CreatedOn,
                    Currency = new CurrencyInfoDto
                    {
                        CurrencyId = iw.CurrencyId,
                        Code = iw.Currency.Code,
                        Name = iw.Currency.Name,
                    },
                    TotalBuyTradesAmount = iw.Trades.Where(t => t.Type == TradeType.Buy).Sum(t => t.Price),
                    TotalSellTradesAmount = iw.Trades.Where(t => t.Type == TradeType.Sell).Sum(t => t.Price),
                    TotalTradesCount = iw.Trades.Count(),
                })
                .OrderBy(iw => iw.CreatedOn)
                .ToListAsync();

            return investmentWallets;
        }

        private async Task<bool> IsUserOwnInvestmentWalletAsync(string userId, int investmentWalletId)
        {
            var investmentWallet = await this.dbContext.InvestmentWallets
                .Where(l => l.Id == investmentWalletId && l.ApplicationUserId == userId)
                .FirstOrDefaultAsync();

            if (investmentWallet == null)
            {
                return false;
            }

            return true;
        }
    }
}
