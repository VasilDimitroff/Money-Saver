namespace MoneySaver.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Web.ViewModels.Administration.Dashboard;

    [Area("Administration")]
    public class DashboardController : AdministrationController
    {

        public IActionResult Index()
        {
           return this.View();
        }
    }
}
