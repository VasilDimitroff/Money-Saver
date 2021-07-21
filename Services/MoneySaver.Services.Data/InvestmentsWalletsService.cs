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

        public async Task<InvestmentWalletTradesDto> GetTradesAsync(string userId, int investmentWalletId, int page, int itemsPerPage = 5)
        {
            InvestmentWallet investmentWallet = await this.dbContext.InvestmentWallets
                .Include(iw => iw.Trades)
                .ThenInclude(t => t.Company)
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
                TotalBuyTradesAmount = investmentWallet.Trades.Where(t => t.Type == TradeType.Buy).Sum(t => t.Price * t.StockQuantity),
                TotalSellTradesAmount = investmentWallet.Trades.Where(t => t.Type == TradeType.Sell).Sum(t => t.Price * t.StockQuantity),
                TotalTradesCount = investmentWallet.Trades.Count(),
                Currency = new CurrencyInfoDto
                {
                    Name = investmentWallet.Currency.Name,
                    CurrencyId = investmentWallet.CurrencyId,
                    Code = investmentWallet.Currency.Code,
                },
                Trades = investmentWallet.Trades
                .OrderByDescending(x => x.CreatedOn)
                .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
                .Select(t => new TradeDto
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
                    TotalBuyTradesAmount = iw.Trades.Where(t => t.Type == TradeType.Buy).Sum(t => t.Price * t.StockQuantity),
                    TotalSellTradesAmount = iw.Trades.Where(t => t.Type == TradeType.Sell).Sum(t => t.Price * t.StockQuantity),
                    TotalTradesCount = iw.Trades.Count(),
                })
                .OrderBy(iw => iw.CreatedOn)
                .ToListAsync();

            return investmentWallets;
        }

        public int GetTradesCount(int investmentWalletId)
        {
            return this.dbContext.Trades.Count(x => x.InvestmentWalletId == investmentWalletId);
        }

        public async Task<IEnumerable<CompanyHoldingsDto>> GetHoldingsAsync(string userId, int investmentWalletId)
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

            var tradedCompanies = await this.dbContext.Companies
                .Where(c => c.Trades.Any(t => t.InvestmentWalletId == investmentWalletId))
                //.Where(c => c.Trades.Any(t => t.InvestmentWallet.ApplicationUserId == userId))
                .Include(c => c.Trades)
                .ThenInclude(t => t.InvestmentWallet)
                .ToListAsync();

            var companiesDto = new List<CompanyHoldingsDto>();

            foreach (var company in tradedCompanies)
            {
                int totalSellCompanyQuantity = company.Trades
                    .Where(t => t.InvestmentWalletId == investmentWalletId && t.Type == TradeType.Sell).Sum(t => t.StockQuantity);

                int totalBuyCompanyQuantity = company.Trades
                   .Where(t => t.InvestmentWalletId == investmentWalletId && t.Type == TradeType.Buy).Sum(t => t.StockQuantity);

                int currentlyCompanyHoldings = totalBuyCompanyQuantity - totalSellCompanyQuantity;

                int buyTradeCompanyCount = company.Trades.Where(t => t.InvestmentWalletId == investmentWalletId && t.Type == TradeType.Buy).Count();
                int sellTradeCompanyCount = company.Trades.Where(t => t.InvestmentWalletId == investmentWalletId && t.Type == TradeType.Sell).Count();

                if (currentlyCompanyHoldings > 0)
                {
                    var companyHoldingDto = new CompanyHoldingsDto
                    {
                        Name = company.Name,
                        Ticker = company.Ticker,
                        StocksHoldings = currentlyCompanyHoldings,
                        BuyTrades = buyTradeCompanyCount,
                        SellTrades = sellTradeCompanyCount,
                    };

                    companiesDto.Add(companyHoldingDto);
                }
            }

            return companiesDto;
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
