using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Web.Models;

namespace WebApplication.Web.DAL
{
    public class BeerSqlDAL : IBeerDAL
    {

        private string connectionString;
        public BeerSqlDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public BeerSearch SearchBeers(string search)
        {
            BeerSearch beerSearch = new BeerSearch();
            Dictionary<int, Beer> beers = new Dictionary<int, Beer>();

            if (string.IsNullOrEmpty(search))
            {
                beerSearch.BeerResults = GetBeers();
                beerSearch.SearchText = search;
                return beerSearch;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    //var query = "SELECT beers.id AS beer_id, beers.name AS beer_name, abv, image_url, BeerTypes.name AS beertype_name, BeerTypes.id AS beertype_id, breweries.id AS brewery_id, breweries.name as brewery_name, breweries.description AS brewery_description FROM beers JOIN beers_beertypes ON beers.id = beers_beertypes.beer_id JOIN beertypes ON beertypes.id = Beers_BeerTypes.beertype_id JOIN breweries_beers ON brewery_id = breweries_beers.brewery_id JOIN breweries ON breweries.id = breweries_beers.brewery_id WHERE beers.name LIKE '%' + @search + '%' OR beers.description LIKE '%' + @search + '%' OR beertypes.name LIKE '%' + @search + '%'";

                        var query = "SELECT beers.id AS beer_id, beers.name AS beer_name, beers.description AS beer_description, abv, image_url, BeerTypes.name AS beertype_name, BeerTypes.id AS beertype_id FROM beers JOIN beers_beertypes ON beers.id = beers_beertypes.beer_id JOIN beertypes ON beertypes.id = Beers_BeerTypes.beertype_id WHERE beers.name LIKE '%' + @search + '%' OR beers.description LIKE '%' + @search + '%' OR beertypes.name LIKE '%' + @search + '%'";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", search);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var beerId = Convert.ToInt32(reader["beer_id"]);
                        if (!beers.ContainsKey(beerId))
                        {
                            beers.Add(beerId, RowToBeer(reader, false));
                        }
                        else
                        {
                            beers[beerId].BeerTypes.Add(RowToBeerType(reader));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }

            beerSearch.BeerResults = beers.Values.ToList();
            beerSearch.SearchText = search;
            return beerSearch;
        }

        public IList<Beer> GetBeers()
        {
            Dictionary<int, Beer> beers = new Dictionary<int, Beer>();

            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var query = "SELECT beers.id AS beer_id, beers.name AS beer_name, beers.description AS beer_description, abv, image_url, BeerTypes.name AS beertype_name, BeerTypes.id AS beertype_id FROM beers JOIN beers_beertypes ON beers.id = beers_beertypes.beer_id JOIN beertypes ON beertypes.id = Beers_BeerTypes.beertype_id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var beerId = Convert.ToInt32(reader["beer_id"]);
                        if (!beers.ContainsKey(beerId))
                        {
                            beers.Add(beerId, RowToBeer(reader, false));
                        }
                        else
                        {
                            beers[beerId].BeerTypes.Add(RowToBeerType(reader));
                        }

                    }
                }
            }
            catch(SqlException ex)
            {
                throw ex;
            }

            return beers.Values.ToList();
        }


        public Beer GetBeer(int beerId)
        {
            Beer beer = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var query = "SELECT beers.id AS beer_id, beers.name AS beer_name, beers.description AS beer_description, abv, image_url, BeerTypes.name AS beertype_name, BeerTypes.id AS beertype_id, breweries.name AS brewery_name, breweries.id AS brewery_id, breweries.description AS brewery_description, beerreview.id AS review_id, beerreview.rating AS review_rating, beerreview.date_of_review AS rating_date, beerreview.review AS review_review, users.id AS user_id, users.username AS user_username FROM beers JOIN beers_beertypes ON beers.id = beers_beertypes.beer_id JOIN beertypes ON beertypes.id = Beers_BeerTypes.beertype_id  JOIN breweries_beers ON breweries_beers.beer_id = beers.id  JOIN breweries ON breweries_beers.brewery_id = breweries.id LEFT JOIN beerreview on beers.id = beerreview.beer_id LEFT JOIN users ON users.id = beerreview.user_id WHERE beers.id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", beerId);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (beer == null)
                        {
                            beer = RowToBeer(reader, true);
                        }
                        else
                        {
                            // see if the beer type already exists
                            var currentBeerType = RowToBeerType(reader);
                            if (!beer.BeerTypes.Any(item => item.BeerTypeId == currentBeerType.BeerTypeId))
                            {
                                beer.BeerTypes.Add(currentBeerType);
                            }

                            // see if the brewery already exists
                            var currentBrewery = RowToBrewery(reader);
                            if (!beer.BreweriesServed.Any(item => item.BreweryId == currentBrewery.BreweryId))
                            {
                                beer.BreweriesServed.Add(currentBrewery);
                            }

                            // see if the beer review already exists
                            var currentBeerReview = RowToBeerReview(reader);
                            if (!beer.BeerReviews.Any(item => item.BeerReviewId == currentBeerReview.BeerReviewId))
                            {
                                beer.BeerReviews.Add(currentBeerReview);
                            }
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }

            return beer;
        }

        private Beer RowToBeer(SqlDataReader reader, bool includeBreweriesAndReviews)
        {
            var beer = new Beer()
            {
                BeerId = Convert.ToInt32(reader["beer_id"]),
                BeerName = Convert.ToString(reader["beer_name"]),
                BeerAbv = Convert.ToDouble(reader["abv"]),
                BeerUrl = Convert.ToString(reader["image_url"]),
                BeerDescription = Convert.ToString(reader["beer_description"])
            };
            if (RowToBeerType(reader) != null)
            {
                beer.BeerTypes = new List<BeerType>() { RowToBeerType(reader) };
            }
            if (includeBreweriesAndReviews && RowToBrewery(reader) != null)
            {
                beer.BreweriesServed= new List<Brewery>() { RowToBrewery(reader) };
            }
            if (includeBreweriesAndReviews && RowToBeerReview(reader) != null)
            {
                beer.BeerReviews = new List<BeerReview> { RowToBeerReview(reader) };
            }
            return beer;
        }

        private BeerType RowToBeerType(SqlDataReader reader)
        {
            return new BeerType
            {
                BeerTypeId = Convert.ToInt32(reader["beertype_id"]),
                TypeName = Convert.ToString(reader["beertype_name"])
            };
        }

        private Brewery RowToBrewery(SqlDataReader reader)
        {
            try
            {
                return new Brewery
                {
                    BreweryId = Convert.ToInt32(reader["brewery_id"]),
                    BreweryName = Convert.ToString(reader["brewery_name"]),
                    Description = Convert.ToString(reader["brewery_description"])
                };
            }
            catch
            {
                return new Brewery();
            }
        }

        private BeerReview RowToBeerReview(SqlDataReader reader)
        {
            try
            {
                var br = new BeerReview
                {
                    BeerReviewId = Convert.ToInt32(reader["review_id"]),
                    ReviewText = Convert.ToString(reader["review_text"]),
                    DateOfReview = Convert.ToDateTime("review_date"),
                    UserReviewing = RowToUser(reader)
                };
                br.SetRaiting(Convert.ToDouble(reader["review_rating"]));
                return br;
            }
            catch
            {
                return new BeerReview();
            }
        }

        private User RowToUser(SqlDataReader reader)
        {
            try
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["user_id"]),
                    Username = Convert.ToString(reader["user_username"])
                };
            }
            catch
            {
                return new User();
            }
        }

        //public void CreateBeer(Beer beer)
        //{
        //    throw new NotImplementedException();
        //}

        //public void UpdateBeer(Beer beer)
        //{
        //    throw new NotImplementedException();
        //}

        //public void DeleteBeer(int id)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            var query = "DELETE FROM Beers_BeerTypes WHERE beer_id = @id;"
        //                + "DELETE FROM Breweries_Beers WHERE beer_id = @id;"
        //                + "DELETE FROM Beers WHERE id = @id;";
        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            cmd.Parameters.AddWithValue("@id", id);

        //            cmd.ExecuteNonQuery();
        //            return;
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
