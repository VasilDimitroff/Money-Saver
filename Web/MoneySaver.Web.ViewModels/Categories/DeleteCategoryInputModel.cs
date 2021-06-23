namespace MoneySaver.Web.ViewModels.Categories
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Web.ViewModels.Records.Enums;

    public class DeleteCategoryInputModel
    {
        public DeleteCategoryInputModel()
        {
            this.Categories = new HashSet<DeleteCategoryNameAndIdViewModel>();
        }

        [Required(ErrorMessage = "The Id of Old Category is required")]
        public int OldCategoryId { get; set; }

        public string OldCategoryName { get; set; }

        public BadgeColor OldCategoryBadgeColor { get; set; }

        [Required(ErrorMessage = "The Id of New Category is required")]
        public int NewCategoryId { get; set; }

        [Required]
        public int WalletId { get; set; }

        public string WalletName { get; set; }

        public IEnumerable<DeleteCategoryNameAndIdViewModel> Categories { get; set; }
    }
}
