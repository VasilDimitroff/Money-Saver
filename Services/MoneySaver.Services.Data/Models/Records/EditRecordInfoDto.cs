namespace MoneySaver.Services.Data.Models.Records
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Data.Models.Enums;
    using MoneySaver.Services.Data.Models.Categories;

    public class EditRecordInfoDto
    {
        public string Id { get; set; }

        public int CategoryId { get; set; }

        public string Description { get; set; }

        public string WalletName { get; set; }

        public decimal Amount { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public IEnumerable<CategoryBasicInfoDto> Categories { get; set; }

        public RecordType Type { get; set; }
    }
}
