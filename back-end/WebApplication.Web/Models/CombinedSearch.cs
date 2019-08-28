using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Web.DAL;

namespace WebApplication.Web.Models
{
    public class CombinedSearch
    {
        public BrewerySearch BrewerySearch;
        public BeerSearch BeerSearch;

        public IEnumerable<SelectListItem> SearchType = new List<SelectListItem>()
        {
            new SelectListItem() {Text="Beer",  Value="Beer"},
            new SelectListItem() {Text="Brewery or Bar", Value="Brewery"},
        };

        public string SearchText;

 

    }
}
