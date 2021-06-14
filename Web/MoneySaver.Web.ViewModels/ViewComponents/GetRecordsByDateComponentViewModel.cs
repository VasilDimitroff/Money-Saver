namespace MoneySaver.Web.ViewModels.ViewComponents
{
    using System;

    public class GetRecordsByDateComponentViewModel
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DefaultStartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime DefaultEndDate { get; set; }

    }
}
