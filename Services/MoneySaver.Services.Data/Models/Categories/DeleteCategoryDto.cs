namespace MoneySaver.Services.Data.Models.Categories
{
    using System.Collections.Generic;

    using MoneySaver.Data.Models.Enums;

    public class DeleteCategoryDto
    {
        public DeleteCategoryDto()
        {
            this.Categories = new HashSet<DeleteCategoryNameAndIdDto>();
        }

        public int OldCategoryId { get; set; }

        public string OldCategoryName { get; set; }

        public BadgeColor OldCategoryBadgeColor { get; set; }

        public int NewCategoryId { get; set; }

        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public IEnumerable<DeleteCategoryNameAndIdDto> Categories { get; set; }
    }
}
