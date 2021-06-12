namespace MoneySaver.Web.Models.Wallets
{
    using MoneySaver.Web.Infrastructure.CustomValidations;

    public class WalletIdViewModel
    {
        [IsWalletExist]
        public int WalletId { get; set; }
    }
}
