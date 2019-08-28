using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Web.Models
{
    public class BeerSearch
    {
        public string SearchText { get; set; }

        public IList<Beer> BeerResults { get; set; }

        public IList<BeerType> BeerType { get; set; }
    }
}
