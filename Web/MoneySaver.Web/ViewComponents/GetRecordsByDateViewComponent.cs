namespace MoneySaver.Web.ViewComponents
{
    using System;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.ViewComponents;

    public class GetRecordsByDateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DateTime startDate, DateTime endDate, int id)
        {
            DateTime now = DateTime.UtcNow;
            DateTime nowAt12AM = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            DateTime nowAt12PM = new DateTime(now.Year, now.Month, now.Day, 22, 59, 59);

            GetRecordsByDateComponentViewModel viewModel = new GetRecordsByDateComponentViewModel();
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.Id = id;
            viewModel.DefaultStartDate = nowAt12AM;
            viewModel.DefaultEndDate = nowAt12PM;

            return this.View(viewModel);
        }
    }
}
