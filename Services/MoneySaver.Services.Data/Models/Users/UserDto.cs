﻿namespace MoneySaver.Services.Data.Models.Users
{
    using System;
    using System.Collections.Generic;

    using MoneySaver.Services.Data.Models.Roles;

    public class UserDto
    {
        public UserDto()
        {
            this.Roles = new List<RoleDto>();
        }

        public string Id { get; set; }

        public string Username { get; set; }

        public DateTime CreatedOn { get; set; }

        public ICollection<RoleDto> Roles { get; set; }
    }
}
