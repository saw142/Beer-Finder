using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.Web.DAL;
using WebApplication.Web.Models;
using WebApplication.Web.Providers.Auth;

namespace WebApplication.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthProvider AuthProvider;
        public IBeerDAL BeerDAL;
        public IBreweryDAL BreweryDAL;
        public IBeerReviewDAL BeerReviewDAL;
        public IUserDAL UserDAL;

        public HomeController(IAuthProvider authProvider, IBeerDAL beerDAL, IBreweryDAL breweryDAL, IBeerReviewDAL beerReviewDAL, IUserDAL userDAL)
        {
            this.AuthProvider = authProvider;
            BeerDAL = beerDAL;
            BreweryDAL = breweryDAL;
            BeerReviewDAL = beerReviewDAL;
            UserDAL = userDAL;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var combinedSearch = new CombinedSearch();
            return View(combinedSearch);
        }

        [HttpGet]
        public IActionResult SearchBeersAndBreweries(string searchType, string searchText)
        {
            if (searchType == "Beer")
            {
                var beerSearchResults = BeerDAL.SearchBeers(searchText);
                return View("SearchBeers", beerSearchResults);
            }
            var brewerySearchResults = BreweryDAL.SearchBreweries(searchText);
            return View("SearchBreweries", brewerySearchResults);
        }

        // BREWERIES ----------------------------
        [HttpGet]
        public IActionResult SearchBreweries(string search)
        {
            return View("searchBreweries");
        }

        [HttpGet]
        public IActionResult BreweryDetail(int id)
        {
            Brewery BreweryDetail = new Brewery();
            var brewery = BreweryDAL.GetBrewery(id);
            return View(brewery);
        }

        // BEERS --------------------------------
        [HttpGet]
        public IActionResult SearchBeers(string search)
        {
            return View();
        }

        [HttpGet]
        public IActionResult BeerDetail(int id)
        {
            Beer BeerDetail = new Beer();
            var beer = BeerDAL.GetBeer(id);
            var user = AuthProvider.GetCurrentUser();
            var reviews = BeerReviewDAL.GetReviews(beer);
            var userReview = BeerReviewDAL.GetReview(user, beer);
            ViewBag.Stars = stars;

            if (userReview == null)
            {
                userReview = new BeerReview
                {
                    BeerReviewId = -1
                };
            }


            BeerViewModel beerView = new BeerViewModel
            {
                CurrentBeer = beer,
                CurrentBeerId = beer.BeerId,

                CurrentUser = user,
                CurrentUserId = ((user == null) ? -1 : user.Id),

                BeerReviews = reviews,

                UserReview = userReview,
                UserRating = userReview.ReviewRating,
                UserReviewText = userReview.ReviewText
            };

            return View(beerView);
        }

        // BEER REVIEW --------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveBeerReview(BeerViewModel response)
        {
            var beerReviewed = BeerDAL.GetBeer(response.CurrentBeerId);
            var userReviewing = AuthProvider.GetCurrentUser();
            var userReviewId = response.UserReviewId;
            var reviewText = response.UserReviewText;
            var reviewRating = response.UserRating;
            BeerReview beerReview = new BeerReview
            {
                BeerReviewed = beerReviewed,
                UserReviewing = userReviewing,
                DateOfReview = DateTime.Now,
                BeerReviewId = userReviewId,
                ReviewText = reviewText,
            };
            beerReview.SetRaiting(response.UserRating);

            if (!string.IsNullOrEmpty(reviewText) && reviewRating > 0 )
            {
                BeerReviewDAL.CreateOrUpdateReview(beerReview);
            }

            return RedirectToAction("BeerDetail", new {id = beerReviewed.BeerId });
        }


        private new Dictionary<int, string> stars = new Dictionary<int, string>
        {
            { 5, "Awesome - 5 stars" },
            { 4, "Pretty good - 4 stars" },
            { 3, "Meh - 3 stars" },
            { 2, "Kinda bad - 2 stars" },
            { 1, "Bad - 1 star" }
        };


        // OTHER --------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
