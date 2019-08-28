using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Web.Models;

namespace WebApplication.Web.DAL
{
    public interface IUserDAL
    {
        /// <summary>
        /// Retrieves a user from the system by username.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetUser(string username);

        User GetUser(int id);

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"></param>
        void CreateUser(User user);

        /// <summary>
        /// Checks to se if a user already exists in the database with the same username or email
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        bool UserExists(string username, string email);

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user"></param>
        void UpdateUser(User user);

        /// <summary>
        /// Deletes a user from the system.
        /// </summary>
        /// <param name="user"></param>
        void DeleteUser(User user);
    }
}
