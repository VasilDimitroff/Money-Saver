namespace MoneySaver.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models;

    public interface IWalletsService
    {
        public Task<IEnumerable<WalletInfoDto>> GetWallets(string userId);
    }
}
