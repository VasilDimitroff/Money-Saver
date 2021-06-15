namespace MoneySaver.Web.ViewModels.ViewComponents
{
    using System;

    public class GetRecordsByDateComponentViewModel
    {

        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DefaultStartDate => DateTime.Today;

        public DateTime EndDate { get; set; }

        public DateTime DefaultEndDate => DateTime.UtcNow;

    }
}
