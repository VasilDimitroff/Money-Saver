namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.Models.ViewComponents;

    public class AddRecordViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int walletId)
        {
            AddRecordComponentViewModel viewModel = new AddRecordComponentViewModel();
            viewModel.WalletId = walletId;
            return this.View(viewModel);
        }
    }
}
