namespace MoneySaver.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using MoneySaver.Data.Common.Models;
    using MoneySaver.Data.Models.Enums;

    public class Trade : BaseModel<string>
    {

        [Required]
        [ForeignKey(nameof(Company))]
        public string CompanyTicker { get; set; }

        public virtual Company Company { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 10000000)]
        public int StockQuantity { get; set; }

        [Required]
        public TradeType Type { get; set; }

        [Required]
        public int InvestmentWalletId { get; set; }

        public virtual InvestmentWallet InvestmentWallet { get; set; }
    }
}