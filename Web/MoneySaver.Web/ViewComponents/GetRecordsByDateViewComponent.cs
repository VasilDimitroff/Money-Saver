namespace MoneySaver.Web.ViewComponents
{
    using System;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class GetRecordsByDateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DateTime startDate, DateTime endDate, int id)
        {
            GetRecordsByDateComponentViewModel viewModel = new GetRecordsByDateComponentViewModel();
            viewModel.DefaultStartDate = DateTime.UtcNow;
            viewModel.DefaultEndDate = DateTime.UtcNow;
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;

            return this.View(viewModel);
        }
    }
}
