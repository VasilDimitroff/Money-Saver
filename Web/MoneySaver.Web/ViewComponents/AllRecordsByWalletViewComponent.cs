namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class AllRecordsByWalletViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int walletId)
        {
            AllRecordsByWalletComponentViewModel viewModel = new AllRecordsByWalletComponentViewModel();
            viewModel.WalletId = walletId;
            return this.View(viewModel);
        }
    }
}
