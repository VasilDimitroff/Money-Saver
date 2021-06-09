namespace MoneySaver.Services.Data.Models
{
    using System.Collections.Generic;

    public class WalletInfoDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal MoneyAmount { get; set; }

        public string Currency { get; set; }

        public IEnumerable<CategoryWalletInfoDto> Categories { get; set; }

    }
}
