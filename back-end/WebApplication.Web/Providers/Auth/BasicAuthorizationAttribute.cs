
///
/// A simple Basic Authorization Filter used if Auth is needed by a Front-End application.
/// Prepared By: Tech Elevator
/// 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Web.DAL;

namespace WebApplication.Web.Providers.Auth
{
    /// <summary>
    /// This class implements a "Basic Auth" check to see if the incoming request
    /// includes the "Authoriation" header. The user's credentials are validated
    /// and the User object added to the HttpContext upon successful authenticaiton.
    /// </summary>
    public class BasicAuthorizationAttribute : Attribute, IActionFilter
    {
        private string[] roles;

        public BasicAuthorizationAttribute(params string[] roles)
        {
            this.roles = roles;
        }

        /// <summary>
        /// Called after the action executes.
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        { }

        /// <summary>
        /// Called before the action executes.
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {

            // Check for the header           
            if (context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                // Extract the credentials
                var header = context.HttpContext.Request.Headers["Authorization"];
                var base64 = header.ToString().Split(' ')[1];
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                var credentials = decoded.Split(':');

                var userDal = context.HttpContext.RequestServices.GetService<IUserDAL>();
                var user = userDal.GetUser(credentials[0]);
                var hashProvider = new HashProvider();

                // User does not exist or failed to provide proper credentials
                if (user == null || !hashProvider.VerifyPasswordMatch(user.Password, credentials[1], user.Salt))
                {
                    context.Result = new StatusCodeResult(401);
                    return;
                }

                // User does not have proper role as provided by the filter.
                if (roles.Length > 0 && !roles.Contains(user.Role))
                {
                    context.Result = new StatusCodeResult(403);
                    return;
                }

                // User is successfully authenticated/authorized.
                context.HttpContext.Items.Add("User", user);
                return;                                
            }

            // Client did not provide the Authorization header
            context.Result = new StatusCodeResult(401);
            return;           
        }
    }
}
