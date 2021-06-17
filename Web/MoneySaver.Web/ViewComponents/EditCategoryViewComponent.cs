namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class EditCategoryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int categoryId, int walletId)
        {
            EditCategoryComponentViewModel viewModel = new EditCategoryComponentViewModel();
            viewModel.CategoryId = categoryId;
            viewModel.WalletId = walletId;

            return this.View(viewModel);
        }
    }
}
