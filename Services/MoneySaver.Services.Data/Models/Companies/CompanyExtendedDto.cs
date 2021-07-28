namespace MoneySaver.Services.Data.Models.Companies
{
    using System;

    public class CompanyExtendedDto : GetCompanyDto
    {
        public int TradesCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
