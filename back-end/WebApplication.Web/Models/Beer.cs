using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Web.Models
{
    public class Beer
    {
        public int BeerId { get; set; }

        public string BeerName { get; set; }

        public double BeerAbv { get; set; }

        public string BeerUrl { get; set; }

        public string BeerDescription { get; set; }

        public IList<BeerType> BeerTypes { get; set; }

        public IList<Brewery> BreweriesServed { get; set; }

        /// <summary>
        /// The user's reviews of beers
        /// </summary>
        public IList<BeerReview> BeerReviews { get; set; }
    }
}
