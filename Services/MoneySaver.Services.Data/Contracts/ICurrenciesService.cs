namespace MoneySaver.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MoneySaver.Services.Data.Models.Currencies;

    public interface ICurrenciesService
    {
        public Task<IEnumerable<CurrencyInfoDto>> GetAllAsync();

        public Task<bool> IsCurrencyExistAsync(int currencyId);
    }
}
