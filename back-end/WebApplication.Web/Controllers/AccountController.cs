using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Web.DAL;
using WebApplication.Web.Models.Account;
using WebApplication.Web.Providers.Auth;

namespace WebApplication.Web.Controllers
{    
    public class AccountController : Controller
    {
        private readonly IAuthProvider AuthProvider;
        private readonly IUserDAL UserDAL;
        public IBeerDAL BeerDAL;
        public IBreweryDAL BreweryDAL;
        public IBeerReviewDAL BeerReviewDAL;

        public AccountController(IAuthProvider authProvider, IUserDAL userDAL, IBeerDAL beerDAL, IBreweryDAL breweryDAL, IBeerReviewDAL beerReviewDAL)
        {
            this.AuthProvider = authProvider;
            this.UserDAL = userDAL;
            this.BeerDAL = beerDAL;
            this.BreweryDAL = breweryDAL;
            this.BeerReviewDAL = beerReviewDAL;
        }

        [AuthorizationFilter] // actions can be filtered to only those that are logged in
        //[AuthorizationFilter("Admin", "Author", "Manager")]  <-- or filtered to only those that have a certain role
        [HttpGet]
        public IActionResult Index()
        {
            var user = AuthProvider.GetCurrentUser();
            user.BeerReviews = BeerReviewDAL.GetReviews(AuthProvider.GetCurrentUser());
            ViewBag.Stars = stars;
            return View(user);
        }


        private new Dictionary<int, string> stars = new Dictionary<int, string>
        {
            { 5, "Awesome - 5 stars" },
            { 4, "Pretty good - 4 stars" },
            { 3, "Meh - 3 stars" },
            { 2, "Kinda bad - 2 stars" },
            { 1, "Bad - 1 star" }
        };


        [HttpGet]
        public IActionResult Login()
        {            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            // Ensure the fields were filled out
            if (ModelState.IsValid)
            {
                // Check that they provided correct credentials
                bool validLogin = AuthProvider.SignIn(loginViewModel.Username, loginViewModel.Password);
                if (validLogin)
                {
                    // Redirect the user where you want them to go after successful login
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(loginViewModel);
        }
        
        [HttpGet]
        public IActionResult LogOff()
        {
            // Clear user from session
            AuthProvider.LogOff();

            // Redirect the user where you want them to go after logoff
            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel registerViewModel = new RegisterViewModel(UserDAL);
            return View(registerViewModel);
        }

        [HttpPost]
        public JsonResult UsernameExists(string username)
        {
            var userExists = AuthProvider.UserExistsCheck(username, String.Empty);

            return Json(userExists);
        }

        [HttpPost]
        public JsonResult EmailExists(string email)
        {
            var userExists = AuthProvider.UserExistsCheck(String.Empty, email);

            return Json(userExists);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel registerViewModel)
        {
            registerViewModel.UserDAL = UserDAL;
            if (ModelState.IsValid)
            {
                // Register them as a new user (and set default role)
                // When a user registeres they need to be given a role. If you don't need anything special
                // just give them "User".
                AuthProvider.Register(registerViewModel.Username, registerViewModel.Email, registerViewModel.Password, role: "User"); 

                // Redirect the user where you want them to go after registering
                return RedirectToAction("Index", "Home");
            }

            return View(registerViewModel);
        }
    }
}