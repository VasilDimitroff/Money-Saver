namespace MoneySaver.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using MoneySaver.Data.Common.Models;
    using MoneySaver.Data.Models.Enums;

    public class UserTrade : BaseModel<string>
    {
        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [ForeignKey(nameof(Company))]
        public string CompanyTicker { get; set; }

        public virtual Company Company { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        public virtual Currency Currency { get; set; }

        [Required]
        public TradeType Type { get; set; }
    }
}