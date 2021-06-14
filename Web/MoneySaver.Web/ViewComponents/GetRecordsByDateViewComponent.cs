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
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.Id = id;
            viewModel.DefaultStartDate = DateTime.UtcNow;
            viewModel.DefaultEndDate = DateTime.UtcNow;

            return this.View(viewModel);
        }
    }
}
