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
    using MoneySaver.Services.Data.Models.InvestmentWallets;
    using MoneySaver.Services.Data.Models.Trades;

    public class TradesService : ITradesService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICompaniesService companiesService;

        public TradesService(
            ApplicationDbContext dbContext,
            ICompaniesService companiesService)
        {
            this.dbContext = dbContext;
            this.companiesService = companiesService;
        }

        public async Task CreateBuyTradeAsync(string userId, int investmentWalletId, string companyId, int quantity, decimal pricePerShare)
        {
            var investmentWallet = await this.dbContext.InvestmentWallets
                .FirstOrDefaultAsync(iw => iw.Id == investmentWalletId);

            if (investmentWallet == null)
            {
                throw new ArgumentException(GlobalConstants.InvestmentWalletNotExist);
            }

            if (!await this.CanUserEditInvestmentWallet(userId, investmentWalletId))
            {
                throw new ArgumentException(GlobalConstants.CannotEditInvestmentWallet);
            }

            var companyDto = await this.companiesService.GetCompanyByIdAsync(companyId);

            if (pricePerShare > 0)
            {
                pricePerShare *= -1;
            }

            var userTrade = new Trade()
            {
                Id = Guid.NewGuid().ToString(),
                InvestmentWalletId = investmentWalletId,
                CreatedOn = DateTime.UtcNow,
                CompanyId = companyDto.Id,
                Price = pricePerShare,
                StockQuantity = quantity,
                Type = TradeType.Buy,
            };

            await this.dbContext.Trades.AddAsync(userTrade);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task CreateSellTradeAsync(string userId, int investmentWalletId, string companyId, int quantity, decimal pricePerShare)
        {
            var investmentWallet = await this.dbContext.InvestmentWallets
                .FirstOrDefaultAsync(iw => iw.Id == investmentWalletId);

            if (investmentWallet == null)
            {
                throw new ArgumentException(GlobalConstants.InvestmentWalletNotExist);
            }

            if (!await this.CanUserEditInvestmentWallet(userId, investmentWalletId))
            {
                throw new ArgumentException(GlobalConstants.CannotEditInvestmentWallet);
            }

            var companyDto = await this.companiesService.GetCompanyByIdAsync(companyId);

            if (pricePerShare < 0)
            {
                pricePerShare *= -1;
            }

            int currentlyHoldings = this.GetCompanyStocksHoldingsCount(companyId, investmentWalletId);

            if (currentlyHoldings < quantity)
            {
                throw new ArgumentException(GlobalConstants.NotEnoughQuantity);
            }

            var userTrade = new Trade()
            {
                Id = Guid.NewGuid().ToString(),

                CreatedOn = DateTime.UtcNow,
                CompanyId = companyDto.Id,
                Price = pricePerShare,
                StockQuantity = quantity,
                Type = TradeType.Sell,
                InvestmentWalletId = investmentWalletId,
            };

            await this.dbContext.Trades.AddAsync(userTrade);
            await this.dbContext.SaveChangesAsync();
        }

        public int GetCompanyStocksHoldingsCount(string companyId, int investmentWalletId)
        {
            int totalBuyQuantity = this.dbContext.Trades
                .Where(ut => ut.InvestmentWalletId == investmentWalletId
                    && ut.CompanyId == companyId
                    && ut.Type == TradeType.Buy)
                .Sum(ut => ut.StockQuantity);

            int totalSellQuantity = this.dbContext.Trades
                .Where(ut => ut.InvestmentWalletId == investmentWalletId
                    && ut.CompanyId == companyId
                    && ut.Type == TradeType.Sell)
                .Sum(ut => ut.StockQuantity);

            int currentlyHoldingQuantity = totalBuyQuantity - totalSellQuantity;

            return currentlyHoldingQuantity;
        }

        public async Task<EditTradeDto> GetTradeInfoForEdit(string userId, string tradeId)
        {
            var trade = await this.dbContext.Trades
                .Include(t => t.Company)
                .Include(t => t.InvestmentWallet)
                .ThenInclude(iw => iw.Currency)
                .FirstOrDefaultAsync(t => t.Id == tradeId);

            if (trade == null)
            {
                throw new ArgumentException(GlobalConstants.TradeNotExist);
            }

            if (!await this.CanUserEditInvestmentWallet(userId, trade.InvestmentWalletId))
            {
                throw new ArgumentException(GlobalConstants.CannotEditInvestmentWallet);
            }

            var tradeDto = new EditTradeDto
            {
                Id = trade.Id,
                CreatedOn = trade.CreatedOn,
                InvestmentWallet = new InvestmentWalletIdNameAndCurrencyDto
                {
                    Id = trade.InvestmentWalletId,
                    Name = trade.InvestmentWallet.Name,
                    CurrencyCode = trade.InvestmentWallet.Currency.Code,
                },
                Price = trade.Price,
                Quantity = trade.StockQuantity,
                Type = trade.Type,
                SelectedCompany = new GetCompanyDto
                {
                    Name = trade.Company.Name,
                    Id = trade.CompanyId,
                    Ticker = trade.Company.Ticker,
                },
                AllInvestmentWallets = await this.GetInvestmentWalletsAsync(userId),
            };

            return tradeDto;
        }

        public async Task UpdateAsync(string userId, string tradeId, string companyId, int investmentWalletId, decimal price, int quantity, DateTime createdOn)
        {
            var trade = await this.dbContext.Trades
               .Include(t => t.Company)
               .Include(t => t.InvestmentWallet)
               .FirstOrDefaultAsync(t => t.Id == tradeId);

            if (trade == null)
            {
                throw new ArgumentException(GlobalConstants.TradeNotExist);
            }

            var company = await this.dbContext.Companies.FirstOrDefaultAsync(c => c.Ticker == companyId);

            if (company == null)
            {
                throw new ArgumentException(GlobalConstants.InvalidCompanyId);
            }

            var investmentWallet = await this.dbContext.InvestmentWallets
                .FirstOrDefaultAsync(iw => iw.Id == investmentWalletId);

            if (investmentWallet == null)
            {
                throw new ArgumentException(GlobalConstants.InvestmentWalletNotExist);
            }

            if (!await this.CanUserEditInvestmentWallet(userId, trade.InvestmentWalletId))
            {
                throw new ArgumentException(GlobalConstants.CannotEditInvestmentWallet);
            }

            trade.InvestmentWalletId = investmentWalletId;
            trade.Price = price;
            trade.StockQuantity = quantity;
            trade.CompanyId = companyId;
            trade.CreatedOn = createdOn;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(string userId, string tradeId)
        {
            var trade = await this.dbContext.Trades
               .FirstOrDefaultAsync(t => t.Id == tradeId);

            if (trade == null)
            {
                throw new ArgumentException(GlobalConstants.TradeNotExist);
            }

            if (!await this.CanUserEditInvestmentWallet(userId, trade.InvestmentWalletId))
            {
                throw new ArgumentException(GlobalConstants.CannotEditInvestmentWallet);
            }

            this.dbContext.Remove(trade);

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<int> GetInvestmentWalletIdByTradeIdAsync(string tradeId)
        {
            var trade = await this.dbContext.Trades.FirstOrDefaultAsync(t => t.Id == tradeId);

            if (trade == null)
            {
                throw new ArgumentException(GlobalConstants.TradeNotExist);
            }

            return trade.InvestmentWalletId;
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

        private async Task<IEnumerable<InvestmentWalletIdNameAndCurrencyDto>> GetInvestmentWalletsAsync(string userId)
        {
            var wallets = await this.dbContext.InvestmentWallets
                .Include(iw => iw.Currency)
                .Where(iw => iw.ApplicationUserId == userId)
                .Select(iw => new InvestmentWalletIdNameAndCurrencyDto
                {
                    Id = iw.Id,
                    CurrencyCode = iw.Currency.Code,
                    Name = iw.Name,
                })
                .ToListAsync();

            return wallets;
        }

        private async Task<bool> CanUserEditInvestmentWallet(string userId, int investmentWalletId)
        {
            var investmentWallet = await this.dbContext.InvestmentWallets
                .Where(w => w.Id == investmentWalletId && w.ApplicationUserId == userId)
                .FirstOrDefaultAsync();

            if (investmentWallet == null)
            {
                return false;
            }

            return true;
        }
    }
}
