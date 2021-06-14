namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class WalletStatisticsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int id)
        {
            WalletStatisticsComponentViewModel viewModel = new WalletStatisticsComponentViewModel();
            viewModel.Id = id;
            return this.View(viewModel);
        }
    }
}
