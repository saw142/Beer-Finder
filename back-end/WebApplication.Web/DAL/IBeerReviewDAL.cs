using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Web.Models;

namespace WebApplication.Web.DAL
{
    public interface IBeerReviewDAL
    {
        BeerReview GetReview(User user, Beer beer);

        IList<BeerReview> GetReviews(User user);

        IList<BeerReview> GetReviews(Beer beer);

        void CreateReview(BeerReview beerReview);

        void UpdateReview(BeerReview beerReview);

        void CreateOrUpdateReview(BeerReview beerReview);

        void DeleteReview(BeerReview beerReview);
    }
}
