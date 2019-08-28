using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Web.DAL;

namespace WebApplication.Web.Models
{
    public class User
    {
        /// <summary>
        /// The user's id.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The user's username.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        /// <summary>
        /// The user's username.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        /// <summary>
        /// The user's password.
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// The user's salt.
        /// </summary>
        [Required]
        public string Salt { get; set; }

        /// <summary>
        /// The user's role.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// The user's reviews of beers
        /// </summary>
        public IList<BeerReview> BeerReviews { get; set; }

        /// <summary>
        /// The user's checkins
        /// </summary>
        public IList<BreweryCheckin> LocationCheckins { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<UserAward> AwardsWon
        {
            get
            {
                return GetUserAwards();
            }
        }

        /// <summary>
        /// TODO: ummm... figure out how to do this
        /// </summary>
        /// <returns></returns>
        private IList<UserAward> GetUserAwards()
        {
            IList<UserAward> awards = new List<UserAward>();

            return awards;
        }
    }
}
