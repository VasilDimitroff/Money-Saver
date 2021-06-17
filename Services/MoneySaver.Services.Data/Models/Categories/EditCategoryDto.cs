using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.Services.Data.Models.Categories
{
    public class EditCategoryDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string BadgeColor { get; set; }

        public int WalletId { get; set; }

        public string WalletName { get; set; }
    }
}
