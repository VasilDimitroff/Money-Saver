namespace MoneySaver.Web.ViewComponents
{
    using System;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class GetRecordsInCategoryByDateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DateTime startDate, DateTime endDate, int categoryId)
        {
            GetRecordsInCategoryByDateComponentViewModel viewModel = new GetRecordsInCategoryByDateComponentViewModel();
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.CategoryId = categoryId;
            viewModel.DefaultStartDate = DateTime.UtcNow;
            viewModel.DefaultEndDate = DateTime.UtcNow;

            return this.View(viewModel);
        }
    }
}
