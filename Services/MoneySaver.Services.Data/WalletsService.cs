namespace MoneySaver.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using MoneySaver.Data;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Contracts;

    public class WalletsService : IWalletsService
    {
        private readonly ApplicationDbContext dbContext;

        public WalletsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Wallet> GetWalletByNameAsync(string userId, string wallet)
        {
            Wallet targetWallet = await this.dbContext.Wallets
              .FirstOrDefaultAsync(w => w.Name.ToLower() == wallet.ToLower() && w.ApplicationUser.Id == userId);

            return targetWallet;
        }
    }
}
