namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class SearchRecordViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string searchTerm, int walletId)
        {
            SearchRecordComponentViewModel viewModel = new SearchRecordComponentViewModel();
            viewModel.SearchTerm = searchTerm;
            viewModel.Id = walletId;

            return this.View(viewModel);
        }
    }
}
