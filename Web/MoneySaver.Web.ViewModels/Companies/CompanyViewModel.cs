namespace MoneySaver.Web.ViewModels.Companies
{
    using System;

    public class CompanyViewModel
    {
        public string Id { get; set; }

        public string Ticker { get; set; }

        public string Name { get; set; }

        public int TradesCount { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
