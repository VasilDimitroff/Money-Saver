namespace MoneySaver.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MoneySaver.Data.Models;
    using MoneySaver.Services.Data.Models;

    public interface IWalletsService
    {
        public Task<IEnumerable<WalletInfoDto>> GetWalletsAsync(string userId);

        public Task<IEnumerable<CategoryWalletInfoDto>> GetWalletCategoriesAsync(int walletId);

        public Task<string> AddAsync(string userId, string name, decimal initialMoney, string currencyName);

        public Task<string> RemoveAsync(int walletId);

        public Task<WalletInfoDto> GetWalletByIdAsync(int walletId);

        public Task<string> GetWalletNameAsync(int walletId);
    }
}
