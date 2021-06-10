namespace MoneySaver.Web.Models.Records
{
    using System.Collections.Generic;

    using MoneySaver.Web.Models.Categories;
    using MoneySaver.Web.Models.Records.Enums;

    public class AddRecordViewModel
    {
        public int CategoryId { get; set; }

        public IEnumerable<CategoryNameIdViewModel> Categories { get; set; }

        public RecordTypeViewModel Type { get; set; }
    }
}
