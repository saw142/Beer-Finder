using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Web.Models
{
    public class BrewerySearch
    {
        public string SearchText { get; set; }

        public IList<Brewery> BreweryResults { get; set; }

    }
}
