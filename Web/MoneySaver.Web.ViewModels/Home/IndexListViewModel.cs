namespace MoneySaver.Web.ViewModels.Home
{
    using System.ComponentModel.DataAnnotations;

    public class IndexListViewModel
    {
        [Required]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
