namespace MoneySaver.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models;

    public interface IWalletsService
    {
        public Task<IEnumerable<WalletInfoDto>> GetWallets(string userId);

        public Task<IEnumerable<CategoryWalletInfoDto>> GetWalletCategories(int walletId);

        public Task<string> Add(string userId, string name, decimal initialMoney, string currencyName);

        public Task<string> Remove(int walletId);
    }
}
