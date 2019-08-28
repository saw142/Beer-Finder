using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Web.Models
{
    public class BeerDetail
    {
        public Beer Beer { get; set; }

        public IList<Beer> Beers { get; set; }

    }
}
