using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Web.Models
{
    public class BeerReview
    {
        public int BeerReviewId { get; set; }

        public Beer BeerReviewed { get; set; }

        public User UserReviewing { get; set; }

        //public BeerReview Review { get; set; }

        public double ReviewRating { get; private set; }

        public string ReviewText { get; set; }

        public DateTime DateOfReview { get; set; }

        public void SetRaiting (Double rating)
        {
            if (rating < 0)
            {
                rating = 0;
            }

            if (rating > 5)
            {
                rating = 5;
            }
            ReviewRating = rating;
        }

        public bool Equals(BeerReview beerReview)
        {
            return (this.BeerReviewed == beerReview.BeerReviewed)
                && (this.UserReviewing == beerReview.UserReviewing);
        }
    }
}