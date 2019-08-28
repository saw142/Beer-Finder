using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Web.Models
{
    public class BreweryDetail
    {
        public Brewery Brewery { get; set; }

        public IList<Brewery> Breweries { get; set; }

    }
}
