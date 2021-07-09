// ReSharper disable VirtualMemberCallInConstructor
namespace MoneySaver.Data.Models
{
    using System;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Identity;
    using MoneySaver.Data.Common.Models;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.Lists = new HashSet<ToDoList>();
            this.Wallets = new HashSet<Wallet>();
            this.InvestmentWallets = new HashSet<InvestmentWallet>();
        }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<ToDoList> Lists { get; set; }

        public virtual ICollection<Wallet> Wallets { get; set; }

        public virtual ICollection<InvestmentWallet> InvestmentWallets { get; set; }
    }
}
