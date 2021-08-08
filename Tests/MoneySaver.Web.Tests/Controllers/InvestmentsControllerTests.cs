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
    using MoneySaver.Data.Models;
    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Web.Controllers;
    using MoneySaver.Web.ViewModels.Currencies;
    using MoneySaver.Web.ViewModels.Investments;
    using Moq;
    using Xunit;

    public class InvestmentsControllerTests
    {
        private readonly ICurrenciesService currenciesService;
        private readonly IInvestmentsWalletsService investmentsWalletsService;
        private readonly FakeUserManager userManager;
        private DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder;
        private ApplicationDbContext db;
        private InvestmentsController controller;
        private AddInvestmentWalletInputModel inputModel;
        private EditInvestmentWalletInputModel editModel;

        public InvestmentsControllerTests()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase("investmentWalletsDatabase");
            this.db = new ApplicationDbContext(this.optionsBuilder.Options);
            this.currenciesService = new CurrenciesService(this.db);
            this.investmentsWalletsService = new InvestmentsWalletsService(this.db);
            this.userManager = new FakeUserManager();
            this.controller = new InvestmentsController(this.currenciesService, this.investmentsWalletsService, this.userManager);
        }

        [Fact]
        public async Task AddWalletGetShouldReturnViewWithValidModel()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.AddWallet();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AddInvestmentWalletInputModel>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Currencies.Count());
        }

        [Fact]
        public async Task AddWalletPostShouldCreateWalletSuccessfully()
        {
            // Arrange
            this.FillDatabase();
            this.inputModel = new AddInvestmentWalletInputModel
            {
                Name = "Test Investment Wallet",
                SelectedCurrencyId = 2,
            };

            // Act
            var result = await this.controller.AddWallet(this.inputModel);

            // Assert
            var viewResult = Assert.IsType<RedirectResult>(result);

            var createdWallet = this.db.InvestmentWallets.FirstOrDefault(x => x.Name == "Test Investment Wallet");

            Assert.Equal("Test Investment Wallet", createdWallet.Name);
            Assert.Equal(2, createdWallet.CurrencyId);
            Assert.Equal(4, this.db.InvestmentWallets.Count());
        }

        [Fact]
        public async Task EditWalletGetShouldReturnViewWithValidModel()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.EditWallet(2);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditInvestmentWalletInputModel>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Currencies.Count());
            Assert.Equal(2, model.SelectedCurrency.CurrencyId);
            Assert.Equal("USD Account", model.Name);
        }

        [Fact]
        public async Task EditWalletGetShouldEditWalletSucessfully()
        {
            // Arrange
            this.FillDatabase();
            this.editModel = new EditInvestmentWalletInputModel
            {
                Id = 2,
                Name = "Edited Wallet Name",
                SelectedCurrency = new CurrencyViewModel
                {
                    CurrencyId = 3,
                    Name = "Euro",
                    Code = "EUR",
                },
            };

            // Act
            var result = await this.controller.EditWallet(this.editModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            var editedWallet = this.db.InvestmentWallets.FirstOrDefault(x => x.Id == 2);
            Assert.Equal("Edited Wallet Name", editedWallet.Name);
            Assert.Equal(3, editedWallet.CurrencyId);
        }

        [Fact]
        public async Task DeleteWalletGetShouldDeleteWalletSucessfully()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.DeleteWallet(2);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            var deletedWallet = this.db.InvestmentWallets.FirstOrDefault(x => x.Id == 2);
            Assert.Null(deletedWallet);
            Assert.Equal(2, this.db.InvestmentWallets.Count());
        }

        [Fact]
        public async Task AllInvestmentsShouldReturn3InvesmentWalletsInView()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.AllInvestments();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<InvestmentWalletViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Count());
        }

        [Fact]
        public async Task TradesShouldReturn2TradesInView()
        {
            // Arrange
            this.FillDatabase();

            // Act
            var result = await this.controller.Trades(2);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<InvestmentWalletTradesViewModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Trades.Count());
            Assert.True(model.Trades.Any(t => t.Type == ViewModels.Trades.Enums.TradeType.Buy));
            Assert.True(model.Trades.Any(t => t.Type == ViewModels.Trades.Enums.TradeType.Sell));
            Assert.True(model.Trades.Any(t => t.Price == 50));
            Assert.True(model.Trades.Any(t => t.StockQuantity == 2));
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
