using System;

using MoneySaver.Data.Models.Enums;

namespace MoneySaver.Data.Models
{
    public class UserTrade
    {
        public string Id { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public int StockId { get; set; }

        public virtual Stock Stock { get; set; }

        public int Quantity { get; set; }

        public TradeType Type { get; set; }

        public DateTime TradeDate { get; set; }

    }
}