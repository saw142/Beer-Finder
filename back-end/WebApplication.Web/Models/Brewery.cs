using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Web.Models
{
    public class Brewery
    {
        public int BreweryId { get; set; }

        public string BreweryName { get; set; }

        public string BreweryAddress1 { get; set; }

        public string BreweryAddress2 { get; set; }

        public string BreweryCity { get; set; }

        public string BreweryZip { get; set; }

        public string HappyHourStartTime { get; set; }

        public string HappyHourEndTime { get; set; }

        public string HappyHourDetails { get; set; }

        public string EstablishedDate { get; set; }

        public string Description { get; set; }

        public IList<Beer> BeersServed { get; set; }

        public bool IsBrewery { get; set; }

        public bool IsBar { get; set; }
    }
}
