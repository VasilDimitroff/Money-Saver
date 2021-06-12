namespace MoneySaver.Services.Data.Models.Wallets
{
    using System;
    using System.Collections.Generic;

    public class WalletCategoriesDto
    {
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public string Currency { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Incomes { get; set; }

        public decimal Outcomes { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public IEnumerable<CategoryWalletInfoDto> Categories { get; set; }
    }
}
