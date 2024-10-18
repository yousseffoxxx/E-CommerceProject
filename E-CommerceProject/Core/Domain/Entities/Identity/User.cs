﻿using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.UserEntities
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; }
        public Address Address { get; set; } // Ref Nav Prop
    }
}
