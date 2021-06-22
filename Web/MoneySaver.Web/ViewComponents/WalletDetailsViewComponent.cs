namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class WalletDetailsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int walletId)
        {
            WalletDetailsComponentViewModel viewModel = new WalletDetailsComponentViewModel();
            viewModel.WalletId = walletId;
            return this.View(viewModel);
        }
    }
}
