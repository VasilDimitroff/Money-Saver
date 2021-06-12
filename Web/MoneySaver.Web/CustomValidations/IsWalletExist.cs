namespace MoneySaver.Web.Infrastructure.CustomValidations
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using MoneySaver.Data;
    using MoneySaver.Web.Models.Wallets;

    public class IsWalletExist : ValidationAttribute
    {
        public IsWalletExist()
        {
        }

        public string GetErrorMessage() => $"Wallet with this Id doesn't exist!";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (WalletIdViewModel)validationContext.ObjectInstance;
            var dbContext = (ApplicationDbContext)validationContext
                         .GetService(typeof(ApplicationDbContext));

            var wallet = dbContext.Wallets.FirstOrDefault(x => x.Id == model.WalletId);

            if (wallet == null)
            {
                return new ValidationResult(this.GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }
}
