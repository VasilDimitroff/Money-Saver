namespace MoneySaver.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;

    using MoneySaver.Web.ViewModels.ViewComponents;

    public class SearchRecordsInCategoryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string searchTerm, int categoryId)
        {
            SearchRecordsByCategoryComponentViewModel viewModel = new SearchRecordsByCategoryComponentViewModel();
            viewModel.SearchTerm = searchTerm;
            viewModel.CategoryId = categoryId;

            return this.View(viewModel);
        }
    }
}
