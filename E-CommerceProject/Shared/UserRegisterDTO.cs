﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public record UserRegisterDTO
    {
        [Required(ErrorMessage = "DisplayName Is Required")]
        public string DisplayName { get; init; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress]
        public string Email { get; init; }

        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; init; }

        [Required(ErrorMessage = "UserName Is Required")]
        public string UserName { get; init; }
        public string? PhoneNumber { get; init; }
    }
}
