using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage ="The {0} must be at least {2} characters long!", MinimumLength = 3)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirm password do not match!")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "User Role")]
        public string RoleName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Contact number")]
        public string Telephone { get; set; }
    }
}
