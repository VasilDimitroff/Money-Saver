using System;

namespace MoneySaver.Web.Models.Records
{
    public class EditRecordViewModel : AddRecordViewModel
    {
        public string Id { get; set; }

        public decimal OldAmount{ get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
