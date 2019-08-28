using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Web.Models;

namespace WebApplication.Web.DAL
{
    public interface IBreweryDAL
    {
        BrewerySearch SearchBreweries(string search);

        Brewery GetBrewery(int id);

        IList<Brewery> GetBreweries();


        //void CreateBrewery(Brewery brewery);

        //void UpdateBrewery(Brewery brewery);

        //void DeleteBrewery(int id);


    }
}
