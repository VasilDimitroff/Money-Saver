﻿namespace MoneySaver.Web.ViewModels.Categories
{
    using System.ComponentModel.DataAnnotations;

    using MoneySaver.Web.ViewModels.Records.Enums;

    public class DeleteCategoryNameAndIdViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public BadgeColor BadgeColor { get; set; }
    }
}
