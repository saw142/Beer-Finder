using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Web.Models.Authentication
{
    /// <summary>
    /// Represents a new user model used for the authentication api.
    /// </summary>
    public class NewUserModel
    {
        /// <summary>
        /// The user's username
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The user's email
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// The user's password
        /// </summary>
        [Required]
        public string Password { get; set; }                
    }
}
