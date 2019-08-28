using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Web.Models;

namespace WebApplication.Web.DAL
{
    public interface IBeerDAL
    {
        /// <summary>
        /// Given a string, returns a list of beers
        /// </summary>
        /// <param name="search">Search string</param>
        /// <returns>a list of beers</returns>
        BeerSearch SearchBeers(string search);

        /// <summary>
        /// Gets all the beers
        /// </summary>
        /// <returns>a list of beers</returns>
        IList<Beer> GetBeers();

        /// <summary>
        /// Gets a single beer's details
        /// </summary>
        /// <param name="BeerId"></param>
        /// <returns>the beer</returns>
        Beer GetBeer(int BeerId);

        /// <summary>
        /// Adds new beer to the database
        /// </summary>
        /// <param name="beer">the beer</param>
        //void CreateBeer(Beer beer);

        /// <summary>
        /// Updates beer in the database
        /// </summary>
        /// <param name="beer">the beer</param>
        //void UpdateBeer(Beer beer);

        /// <summary>
        /// Deletes a beer from the database
        /// </summary>
        /// <param name="id">the beer id</param>
        //void DeleteBeer(int id);

    }
}
