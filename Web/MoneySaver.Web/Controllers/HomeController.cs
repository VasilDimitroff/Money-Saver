namespace MoneySaver.Web.Controllers
{
    using System.Diagnostics;

    using MoneySaver.Web.ViewModels;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data.Contracts;

    public class HomeController : BaseController
    {
        private IRecordsService recordsService;

        public HomeController(IRecordsService recordsService)
        {
            this.recordsService = recordsService;
        }

        public IActionResult Index()
        {

            try
            {
               // this.recordsService.AddAsync(userId, description, amount, category, type, wallet);
            }
            catch (System.Exception ex)
            {
                BadRequest(ex.Message);
            }

            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        public IActionResult AddRecord()
        {
   
            return this.View();
        }
    }
}
