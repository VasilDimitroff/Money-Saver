namespace MoneySaver.Web.ViewModels.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MoneySaver.Common;
    using MoneySaver.Web.ViewModels.Roles;

    public class UserViewModel
    {
        public UserViewModel()
        {
            this.Roles = new List<RoleViewModel>();
        }

        public string Id { get; set; }

        public string Username { get; set; }

        public DateTime CreatedOn { get; set; }

        public ICollection<RoleViewModel> Roles { get; set; }

        public bool IsAdmin => this.Roles.Any(r => r.Name.ToLower() == GlobalConstants.AdministratorRoleName.ToLower());

        public bool IsDeleted { get; set; }
    }
}
