namespace MoneySaver.Web.Models.ViewComponents
{
    using System;

    public class GetRecordsByDateComponentViewModel
    {
        public int WalletId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
