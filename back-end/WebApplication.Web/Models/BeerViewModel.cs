using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Web.Models
{
    public class BeerViewModel
    {
        //public User CurrentUser { get; set; }
        public Beer CurrentBeer { get; set; }
        public int CurrentBeerId { get; set; }

        public User CurrentUser { get; set; }
        public int CurrentUserId { get; set; }

        public IList<BeerReview> BeerReviews { get; set; }

        public BeerReview UserReview { get; set; }

        public int UserReviewId { get; set; }

        [Required]
        [Display(Name = "Your Rating")]
        public double UserRating { get; set; }

        [Required]
        [Display(Name = "Your Review")]
        public string UserReviewText { get; set; }

        public double AverageRating
        {
            get
            {
                return AverageBeerReview();
            }
        }

        private double AverageBeerReview()
        {
            if (BeerReviews != null && BeerReviews.Count != 0)
            {
                double[] ratings = BeerReviews.Select(review => review.ReviewRating).ToArray();
                return AverageDouble(ratings);
            }
            return -1;
        }



        private double AverageDouble(double[] nums)
        {
            int count = 0;
            double sum = 0;
            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i] >= 1)
                {
                    sum += nums[i];
                    count++;
                }
            }

            return sum/count;
        }

        //public UserBeerReviewViewModel UserBeerReviewModel { get; set; }
    }
}
