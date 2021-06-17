namespace MoneySaver.Web.ViewModels.Categories
{
    using System.Collections.Generic;

    using MoneySaver.Web.ViewModels.Records.Enums;

    public class DeleteCategoryInputModel
    {
        public DeleteCategoryInputModel()
        {
            this.Categories = new HashSet<DeleteCategoryNameAndIdViewModel>();
        }

        public int OldCategoryId { get; set; }

        public string OldCategoryName { get; set; }

        public BadgeColor OldCategoryBadgeColor { get; set; }

        public int NewCategoryId { get; set; }

        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public IEnumerable<DeleteCategoryNameAndIdViewModel> Categories { get; set; }
    }
}
