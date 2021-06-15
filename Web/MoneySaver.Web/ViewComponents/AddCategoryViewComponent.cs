using Microsoft.AspNetCore.Mvc;
using MoneySaver.Web.ViewModels.ViewComponents;

namespace MoneySaver.Web.ViewComponents
{
    public class AddCategoryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int walletId)
        {
            AddCategoryComponentViewModel viewModel = new AddCategoryComponentViewModel();
            viewModel.WalletId = walletId;
            return this.View(viewModel);
        }
    }
}
