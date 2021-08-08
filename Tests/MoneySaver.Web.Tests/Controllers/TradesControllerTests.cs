namespace MoneySaver.Web.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MoneySaver.Data;
    using MoneySaver.Data.Common.Repositories;
    using MoneySaver.Data.Models;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Data.Repositories;
    using MoneySaver.Services.Data;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.Controllers;
    using MoneySaver.Web.ViewModels.Currencies;
    using MoneySaver.Web.ViewModels.Trades;
    using MoneySaver.Web.ViewModels.Wallets;
    using Moq;
    using Xunit;

    public class TradesControllerTests
    {
        private readonly ITradesService tradesService;
        private readonly ICompaniesService companiesService;
        private readonly FakeUserManager userManager;
        private DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder;
        private ApplicationDbContext db;
        private IDeletableEntityRepository<Company> repo;
        private TradesController controller;
        private AddTradeInputModel inputModel;
        private EditTradeInputModel editModel;

        public TradesControllerTests()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase("tradesDatabase");
            this.db = new ApplicationDbContext(this.optionsBuilder.Options);
            this.repo = new EfDeletableEntityRepository<Company>(this.db);
            this.companiesService = new CompaniesService(this.repo);
            this.tradesService = new TradesService(this.db, this.companiesService);
            this.userManager = new FakeUserManager();
            this.controller = new TradesController(this.companiesService, this.tradesService, this.userManager);
        }

        [Fact]
        public async Task AddTradeShouldReturnViewWithValidModel()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Add(2);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AddTradeInputModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.InvestmentWalletId);
            Assert.Equal(3, model.Companies.Count());
        }

        [Fact]
        public async Task AddTradeShouldCreateNewBuyTrade()
        {
            // Arrange
            this.FillDatabase();
            this.inputModel = new AddTradeInputModel
            {
                Price = 20,
                InvestmentWalletId = 2,
                Quantity = 5,
                Type = ViewModels.Trades.Enums.TradeType.Buy,
                SelectedCompany = new CompanyViewModel
                {
                    Id = "company3",
                    Name = "Amazon",
                    Ticker = "amzn",
                },
            };

            // Act
            var result = await this.controller.Add(this.inputModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            var newTrade = this.db.Trades.FirstOrDefault(x => !x.Id.StartsWith("trade"));
            Assert.Equal(2, newTrade.InvestmentWalletId);
            Assert.Equal(-20, newTrade.Price);
            Assert.Equal(5, newTrade.StockQuantity);
            Assert.Equal("company3", newTrade.CompanyId);
            Assert.Equal(4, this.db.Trades.Count());
            Assert.Equal(TradeType.Buy, newTrade.Type);
        }

        [Fact]
        public async Task AddTradeShouldCreateNewSellTrade()
        {
            // Arrange
            this.FillDatabase();
            this.inputModel = new AddTradeInputModel
            {
                Price = -50,
                InvestmentWalletId = 2,
                Quantity = 3,
                Type = ViewModels.Trades.Enums.TradeType.Sell,
                SelectedCompany = new CompanyViewModel
                {
                    Id = "company2",
                    Name = "Facebook",
                    Ticker = "FB",
                },
            };

            // Act
            var result = await this.controller.Add(this.inputModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            var newTrade = this.db.Trades.FirstOrDefault(x => !x.Id.StartsWith("trade"));
            Assert.Equal(2, newTrade.InvestmentWalletId);
            Assert.Equal(50, newTrade.Price);
            Assert.Equal(3, newTrade.StockQuantity);
            Assert.Equal("company2", newTrade.CompanyId);
            Assert.Equal(4, this.db.Trades.Count());
            Assert.Equal(TradeType.Sell, newTrade.Type);
        }

        [Fact]
        public async Task EditShouldReturnViewWithValidModel()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Edit("trade1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditTradeInputModel>(viewResult.ViewData.Model);
            Assert.Equal(ViewModels.Trades.Enums.TradeType.Buy, model.Type);
            Assert.Equal("company1", model.SelectedCompany.Id);
            Assert.Equal(5, model.Quantity);
            Assert.Equal(-100, model.Price);
            Assert.Equal(-500, model.Amount);
        }

        [Fact]
        public async Task EditShouldEditTrade()
        {
            // Arrange
            this.FillDatabase();
            this.editModel = new EditTradeInputModel
            {
                Id = "trade1",
                InvestmentWalletId = 1,
                Quantity = 11,
                Price = 120,
                Type = ViewModels.Trades.Enums.TradeType.Buy,
                SelectedCompany = new CompanyViewModel
                {
                    Id = "company3",
                    Name = "Amazon",
                    Ticker = "amzn",
                },
            };

            // Act
            var result = await this.controller.Edit(this.editModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            var editedTrade = this.db.Trades.FirstOrDefault(x => x.Id == "trade1");
            Assert.Equal(TradeType.Buy, editedTrade.Type);
            Assert.Equal("company3", editedTrade.CompanyId);
            Assert.Equal("AMZN", editedTrade.Company.Ticker);
            Assert.Equal("Amazon", editedTrade.Company.Name);
            Assert.Equal(11, editedTrade.StockQuantity);
            Assert.Equal(-120, editedTrade.Price);
            Assert.Equal(1, editedTrade.InvestmentWalletId);
            Assert.Equal(3, this.db.Trades.Count());
        }

        [Fact]
        public async Task DeleteShouldDeleteTrade()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Delete("trade1");

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            var deletedTrade = this.db.Trades.FirstOrDefault(x => x.Id == "trade1");
            Assert.Null(deletedTrade);
            Assert.Equal(2, this.db.Trades.Count());
        }

        private void FillDatabase()
        {
            this.CleanDatabase();
            this.AddUser();
            this.AddCurrencies();
            this.AddCompanies();
            this.AddInvestmentWallets();
            this.AddTrades();
        }

        private void CleanDatabase()
        {
            foreach (var trade in this.db.Trades)
            {
                this.db.Trades.Remove(trade);
                this.db.SaveChanges();
            }

            foreach (var wallet in this.db.InvestmentWallets)
            {
                this.db.InvestmentWallets.Remove(wallet);
                this.db.SaveChanges();
            }

            foreach (var company in this.db.Companies)
            {
                this.db.Companies.Remove(company);
                this.db.SaveChanges();
            }

            foreach (var currency in this.db.Currencies)
            {
                this.db.Currencies.Remove(currency);
                this.db.SaveChanges();
            }

            foreach (var user in this.db.Users)
            {
                this.db.Users.Remove(user);
                this.db.SaveChanges();
            }

            this.db.SaveChanges();
        }

        private ApplicationUser AddUser()
        {
            var user = new ApplicationUser()
            {
                Id = "userId",
                Email = "v.b.dimitrow@gmail.com",
                IsDeleted = false,
                PasswordHash = "UnhashedPass2021",
                UserName = "v.b.dimitrow@gmail.com",
                CreatedOn = DateTime.UtcNow,
                NormalizedUserName = "V.B.DIMITROW@GMAIL.COM",
                NormalizedEmail = "V.B.DIMITROW@GMAIL.COM",
                EmailConfirmed = false,
                SecurityStamp = "SecurityStamp",
                ConcurrencyStamp = "ConcurrencyStamp",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
            };

            this.db.Users.Add(user);
            this.db.SaveChanges();

            return user;
        }

        private IEnumerable<Currency> AddCurrencies()
        {
            var currencies = new List<Currency>();

            currencies.Add(new Currency
            {
                Id = 1,
                Code = "BGN",
                Name = "Bulgarian Lev",
            });

            currencies.Add(new Currency
            {
                Id = 2,
                Code = "USD",
                Name = "US Dollar",
            });

            currencies.Add(new Currency
            {
                Id = 3,
                Code = "EUR",
                Name = "Euro",
            });

            this.db.Currencies.AddRange(currencies);
            this.db.SaveChanges();

            return currencies;
        }

        private IEnumerable<Company> AddCompanies()
        {
            var companies = new List<Company>();

            companies.Add(new Company
            {
                Id = "company1",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false,
                Name = "Tesla",
                Ticker = "TSLA",
                Trades = new HashSet<Trade>(),
            });

            companies.Add(new Company
            {
                Id = "company2",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false,
                Name = "Facebook",
                Ticker = "FB",
                Trades = new HashSet<Trade>(),
            });

            companies.Add(new Company
            {
                Id = "company3",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false,
                Name = "Amazon",
                Ticker = "AMZN",
                Trades = new HashSet<Trade>(),
            });

            this.db.Companies.AddRange(companies);
            this.db.SaveChanges();

            return companies;
        }

        private void AddInvestmentWallets()
        {
            var wallets = new List<InvestmentWallet>();

            wallets.Add(new InvestmentWallet
            {
                Id = 1,
                CurrencyId = 1,
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = "userId",
                Name = "BGN Account",
                Trades = new HashSet<Trade>(),
            });

            wallets.Add(new InvestmentWallet
            {
                Id = 2,
                CurrencyId = 2,
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = "userId",
                Name = "USD Account",
                Trades = new HashSet<Trade>(),
            });

            wallets.Add(new InvestmentWallet
            {
                Id = 3,
                CurrencyId = 3,
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = "userId",
                Name = "EUR Account",
                Trades = new HashSet<Trade>(),
            });

            this.db.InvestmentWallets.AddRange(wallets);
            this.db.SaveChanges();
        }

        private IEnumerable<Trade> AddTrades()
        {
            var trades = new List<Trade>();

            trades.Add(new Trade
            {
                Id = "trade1",
                CompanyId = "company1",
                InvestmentWalletId = 1,
                Price = -100,
                StockQuantity = 5,
                CreatedOn = DateTime.UtcNow,
                Type = TradeType.Buy,
            });

            trades.Add(new Trade
            {
                Id = "trade2",
                CompanyId = "company2",
                InvestmentWalletId = 2,
                Price = -200,
                StockQuantity = 10,
                CreatedOn = DateTime.UtcNow,
                Type = TradeType.Buy,
            });

            trades.Add(new Trade
            {
                Id = "trade3",
                CompanyId = "company2",
                InvestmentWalletId = 2,
                Price = 50,
                StockQuantity = 2,
                CreatedOn = DateTime.UtcNow,
                Type = TradeType.Sell,
            });

            this.db.Trades.AddRange(trades);
            this.db.SaveChanges();

            return trades;
        }
    }
}
