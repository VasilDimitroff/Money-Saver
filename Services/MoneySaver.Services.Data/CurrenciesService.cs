namespace MoneySaver.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using MoneySaver.Data;
    using MoneySaver.Services.Data.Contracts;
    using MoneySaver.Services.Data.Models.Currencies;

    public class CurrenciesService : ICurrenciesService
    {
        private readonly ApplicationDbContext db;

        public CurrenciesService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<CurrencyInfoDto>> GetAllAsync()
        {
            IEnumerable<CurrencyInfoDto> currencies = await this.db.Currencies
                .Select(x => new CurrencyInfoDto
                    {
                        Code = x.Code,
                        CurrencyId = x.Id,
                        Name = x.Name,
                    })
                .ToListAsync();

            return currencies;
        }
    }
}
