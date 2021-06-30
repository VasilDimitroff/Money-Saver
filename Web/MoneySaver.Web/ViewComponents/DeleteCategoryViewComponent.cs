namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class DeleteCategoryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int categoryId, int walletId)
        {
            DeleteCategoryComponentViewModel viewModel = new DeleteCategoryComponentViewModel();
            viewModel.CategoryId = categoryId;
            viewModel.WalletId = walletId;

            return this.View(viewModel);
        }
    }
}
