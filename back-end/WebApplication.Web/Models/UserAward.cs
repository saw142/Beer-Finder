using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Web.Models
{
    public class UserAward
    {
        public string AwardName { get; set; }

        public string BadgeImageUrl { get; set; }

        public DateTime AwardWonDate { get; set; }
    }
}
