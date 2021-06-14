namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class EditRecordViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string recordId, int walletId)
        {
            EditRecordComponentViewModel viewModel = new EditRecordComponentViewModel();
            viewModel.Id = recordId;
            viewModel.WalletId = walletId;
            return this.View(viewModel);
        }
    }
}
