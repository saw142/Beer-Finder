using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Web.DAL;

namespace WebApplication.Web.Models.Account
{
    public class RegisterViewModel : IValidatableObject
    {
        public IUserDAL UserDAL { get; set; }

        public RegisterViewModel(IUserDAL userDAL)
        {
            UserDAL = userDAL;
        }

        public RegisterViewModel()
        {
            // this is the WRONG way to do this... but due to timing, it's staying.
            UserDAL = new UserSqlDAL(@"Data Source=.\SQLEXPRESS;Initial Catalog=CleBrews;Integrated Security=True");
        }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (UserDAL.UserExists(Username, Email))
            {
                yield return new ValidationResult("User already exists. Please enter a new username/email.");
            }
        }
    }
}
