using MoneySaver.Data.Models.Enums;
using System;

namespace MoneySaver.Data.Models
{
    public class UserTrade
    {
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public int StockId { get; set; }

        public virtual Stock Stock { get; set; }

        public int Quantity { get; set; }

        public TradeType Type { get; set; }

        public DateTime TradeDate { get; set; }

    }
}