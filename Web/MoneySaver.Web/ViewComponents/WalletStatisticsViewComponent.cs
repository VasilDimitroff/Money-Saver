namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.Models.ViewComponents;

    public class WalletStatisticsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int walletId)
        {
            WalletStatisticsComponentViewModel viewModel = new WalletStatisticsComponentViewModel();
            viewModel.WalletId = walletId;
            return this.View(viewModel);
        }
    }
}
