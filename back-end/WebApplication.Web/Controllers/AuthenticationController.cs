using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Web.DAL;
using WebApplication.Web.Models;
using WebApplication.Web.Models.Account;
using WebApplication.Web.Models.Authentication;
using WebApplication.Web.Providers.Auth;

namespace WebApplication.Web.Controllers
{
    /// <summary>
    /// This api endpoint can be used to manage user data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IUserDAL userDAL;

        /// <summary>
        /// Creates a new authentication controller.
        /// </summary>
        /// <param name="authProvider"></param>
        /// <param name="userDAL"></param>
        public AuthenticationController(IUserDAL userDAL)
        {
            this.userDAL = userDAL;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">A model representing the new user.</param>
        /// <returns></returns>
        [HttpPost("register")]
        public ActionResult<dynamic> Register(NewUserModel model)
        {
            // Validate Request
            if (String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { Message = "Username or password missing." });
            }
            //if (userDAL.GetUser(model.Username) != null)
            if (userDAL.UserExists(model.Username, model.Email))
            {
                return BadRequest(new { Message = "Username or email already exists" });
            }

            // Create the user
            var hashProvider = new HashProvider();
            var hashedPassword = hashProvider.HashPassword(model.Password);
            var user = new User
            {
                Password = hashedPassword.Password,
                Salt = hashedPassword.Salt,
                Username = model.Username,
                Role = "User"
            };
            userDAL.CreateUser(user);

            return Ok();
        }

        /// <summary>
        /// Gets the currently logged-in user object.
        /// </summary>
        /// <returns></returns>
        [BasicAuthorization]
        [HttpGet("user", Name = "GetUser")]
        public ActionResult<dynamic> GetUser()
        {
            var user = (User)HttpContext.Items["User"];
            return Ok(new
            {
                user.Username,
                user.Role
            });
        }


    }
}