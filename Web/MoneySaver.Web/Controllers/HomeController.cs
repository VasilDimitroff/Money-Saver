namespace MoneySaver.Web.Controllers
{
    using System.Diagnostics;

    using MoneySaver.Web.ViewModels;

    using Microsoft.AspNetCore.Mvc;
    using MoneySaver.Services.Data;
    using MoneySaver.Web.Models;

    public class HomeController : BaseController
    {
        private IRecordsService recordsService;

        public HomeController(IRecordsService recordsService)
        {
            this.recordsService = recordsService;
        }

        public IActionResult Index()
        {
            string description = "test description";
            string category = "test category";
            decimal amount = 1000M;
            string type = "Income";
            string wallet = "Prihodi";

            RecordViewModel record = new RecordViewModel
            {
                Description = description,
                Category = category,
                Amount = amount,
                Type = type,
                Wallet = wallet
            };

            try
            {
                this.recordsService.AddAsync(description, amount, category, type, wallet);
            }
            catch (System.Exception ex)
            {
                BadRequest(ex.Message);
            }

            return this.View(record);
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
