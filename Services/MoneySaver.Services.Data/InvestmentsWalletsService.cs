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
            var currency = await this.dbContext.Currencies.FirstOrDefaultAsync(c => c.Id == currencyId);

            if (currency == null)
            {
                throw new ArgumentException(GlobalConstants.CurrencyNotExist);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Unnamed Wallet";
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

        public async Task<InvestmentWalletTradesDto> GetTradesAsync(string userId, int investmentWalletId)
        {
            var investmentWallet = await this.dbContext.InvestmentWallets
                .Include(iw => iw.Trades)
                .Include(iw => iw.Currency)
                .FirstOrDefaultAsync(iw => iw.Id == investmentWalletId);

            if (investmentWallet == null)
            {
                throw new ArgumentException(GlobalConstants.InvestmentWalletNotExist);
            }

            if (!await this.IsUserOwnInvestmentWalletAsync(userId, investmentWalletId))
            {
                throw new ArgumentException(GlobalConstants.NotPermissionsToEditInvestmentWallet);
            }

            var walletTrades = new InvestmentWalletTradesDto
            {
                Id = investmentWallet.Id,
                Name = investmentWallet.Name,
                CreatedOn = investmentWallet.CreatedOn,
                TotalBuyTradesAmount = investmentWallet.Trades.Where(t => t.Type == TradeType.Buy).Sum(t => t.Price),
                TotalSellTradesAmount = investmentWallet.Trades.Where(t => t.Type == TradeType.Sell).Sum(t => t.Price),
                TotalTradesCount = investmentWallet.Trades.Count(),
                Currency = new CurrencyInfoDto
                {
                    Name = investmentWallet.Currency.Name,
                    CurrencyId = investmentWallet.CurrencyId,
                    Code = investmentWallet.Currency.Code,
                },
                Trades = investmentWallet.Trades.Select(t => new TradeDto
                {
                    Id = t.Id,
                    CreatedOn = t.CreatedOn,
                    Price = t.Price,
                    Type = t.Type,
                    StockQuantity = t.StockQuantity,
                    InvestmentWalletId = investmentWallet.Id,
                    Company = new GetCompanyDto
                    {
                        Name = t.Company.Name,
                        Ticker = t.CompanyTicker,
                    },
                })
                .ToList(),
            };

            return walletTrades;
        }

        public async Task<string> GetInvestmentWalletNameAsync(int investmentWalletId)
        {
            var investmentWallet = await this.dbContext.InvestmentWallets
               .FirstOrDefaultAsync(iw => iw.Id == investmentWalletId);

            if (investmentWallet == null)
            {
                throw new ArgumentException(GlobalConstants.InvestmentWalletNotExist);
            }

            return investmentWallet.Name;
        }

        public async Task<CurrencyInfoDto> GetInvestmentCurrencyAsync(int investmentWalletId)
        {
            var investmentWallet = await this.dbContext.InvestmentWallets
                .Include(iw => iw.Currency)
                .FirstOrDefaultAsync(iw => iw.Id == investmentWalletId);

            if (investmentWallet == null)
            {
                throw new ArgumentException(GlobalConstants.InvestmentWalletNotExist);
            }

            var currency = new CurrencyInfoDto()
            {
                CurrencyId = investmentWallet.CurrencyId,
                Code = investmentWallet.Currency.Code,
                Name = investmentWallet.Currency.Name,
            };

            return currency;
        }

        public async Task EditAsync(string userId, int investmentWalletId, int currencyId, string name)
        {
            var investmentWallet = await this.dbContext.InvestmentWallets
                .Include(iw => iw.Trades)
                .FirstOrDefaultAsync(iw => iw.Id == investmentWalletId);

            if (investmentWallet == null)
            {
                throw new ArgumentException(GlobalConstants.InvestmentWalletNotExist);
            }

            var currency = await this.dbContext.Currencies
                .FirstOrDefaultAsync(c => c.Id == currencyId);

            if (currency == null)
            {
                throw new ArgumentException(GlobalConstants.CurrencyNotExist);
            }

            if (!await this.IsUserOwnInvestmentWalletAsync(userId, investmentWalletId))
            {
                throw new ArgumentException(GlobalConstants.NotPermissionsToEditInvestmentWallet);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Unnamed Wallet";
            }

            investmentWallet.Name = name;
            investmentWallet.CurrencyId = currencyId;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(string userId, int investmentWalletId)
        {
            var investmentWallet = await this.dbContext.InvestmentWallets
                 .Include(iw => iw.Trades)
                 .FirstOrDefaultAsync(iw => iw.Id == investmentWalletId);

            if (investmentWallet == null)
            {
                throw new ArgumentException(GlobalConstants.InvestmentWalletNotExist);
            }

            if (!await this.IsUserOwnInvestmentWalletAsync(userId, investmentWalletId))
            {
                throw new ArgumentException(GlobalConstants.NotPermissionsToEditInvestmentWallet);
            }

            var tradesForDelete = new List<Trade>();

            foreach (var trade in investmentWallet.Trades)
            {
                tradesForDelete.Add(trade);
            }

            this.dbContext.RemoveRange(tradesForDelete);

            this.dbContext.InvestmentWallets.Remove(investmentWallet);

            this.dbContext.SaveChanges();
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
