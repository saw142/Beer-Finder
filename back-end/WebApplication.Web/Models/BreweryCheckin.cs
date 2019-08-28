using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Web.Models
{
    public class BreweryCheckin
    {
        public Brewery Brewery { get; set; }

        public User UserCheckingIn { get; set; }
        
        public DateTime DateOfCheckin { get; set; }
    }
}
