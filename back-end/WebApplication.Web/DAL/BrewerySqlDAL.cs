using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Web.Models;

namespace WebApplication.Web.DAL
{
    public class BrewerySqlDAL : IBreweryDAL
    {
        private readonly string connectionString;

        public BrewerySqlDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public BrewerySearch SearchBreweries(string search)
        {
            var breweries = new List<Brewery>();
            BrewerySearch brewerySearch = new BrewerySearch();

            if (string.IsNullOrEmpty(search))
            {
                brewerySearch.BreweryResults = GetBreweries();
                brewerySearch.SearchText = search;
                return brewerySearch;
            }
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    //var query = $"SELECT breweries.id AS brewery_id, breweries.description AS brewery_description, breweries.address_1, breweries.address_2, breweries.address_city, breweries.bar, breweries.brewery, breweries.established_date, breweries.happyhour_start, breweries.happyhour_end, breweries.happyhour_details, breweries.name AS brewery_name, beers.id AS beer_id, breweries.address_zip, beers.name AS beer_name, abv, image_url, BeerTypes.name AS beertype_name, BeerTypes.id AS beertype_id FROM breweries JOIN breweries_beers ON breweries.id = breweries_beers.brewery_id JOIN beers ON breweries_beers.beer_id = beers.id JOIN beers_beertypes ON beers.id = beers_beertypes.beer_id JOIN beertypes ON beertypes.id = Beers_BeerTypes.beertype_id WHERE breweries.name LIKE '%' + @search + '%' OR breweries.address_city LIKE '%' + @search + '%'";

                    var query = $"SELECT breweries.id AS brewery_id, breweries.description AS brewery_description, breweries.address_1, breweries.address_2, breweries.address_city, breweries.bar, breweries.brewery, breweries.established_date, breweries.happyhour_start, breweries.address_zip, breweries.happyhour_end, breweries.happyhour_details, breweries.name AS brewery_name FROM breweries WHERE breweries.name LIKE '%' + @search + '%' OR breweries.address_city LIKE '%' + @search + '%'";

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", search);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        breweries.Add(MapRowToBrewery(reader, false));
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }

            brewerySearch.BreweryResults = breweries;
            brewerySearch.SearchText = search;
            return brewerySearch;
        }

        public IList<Brewery> GetBreweries()
        {
            var breweries = new List<Brewery>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT breweries.id AS brewery_id, breweries.description AS brewery_description, breweries.address_1, breweries.address_2, breweries.address_city, breweries.bar, breweries.brewery, breweries.established_date, breweries.happyhour_start, breweries.address_zip, breweries.happyhour_end, breweries.happyhour_details, breweries.name AS brewery_name  FROM breweries;", conn);

                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        breweries.Add(MapRowToBrewery(reader, false));
                    }

                }

                return breweries;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public Brewery GetBrewery(int id)
        {
            Brewery brewery = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var query = $"SELECT breweries.id AS brewery_id, breweries.description AS brewery_description, breweries.address_1, breweries.address_2, breweries.address_city, breweries.bar, breweries.brewery, breweries.established_date, breweries.happyhour_start, breweries.happyhour_end, breweries.happyhour_details, breweries.name AS brewery_name, beers.id AS beer_id, breweries.address_zip, beers.name AS beer_name, abv, image_url, BeerTypes.name AS beertype_name, BeerTypes.id AS beertype_id FROM breweries JOIN breweries_beers ON breweries.id = breweries_beers.brewery_id JOIN beers ON breweries_beers.beer_id = beers.id JOIN beers_beertypes ON beers.id = beers_beertypes.beer_id JOIN beertypes ON beertypes.id = Beers_BeerTypes.beertype_id WHERE brewery_id = @id";
                    //SqlCommand cmd = new SqlCommand("SELECT breweries.id AS brewery_id, breweries.description AS brewery_description, breweries.address_1, breweries.address_2, breweries.address_city, breweries.bar, breweries.brewery, breweries.established_date, breweries.happyhour_start, breweries.address_zip, breweries.happyhour_end, breweries.happyhour_details, breweries.name AS brewery_name  FROM breweries WHERE id = @id", conn);
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (brewery == null)
                        {
                            brewery = MapRowToBrewery(reader, true);
                        }
                        else
                        {
                            // see if the beer already exists
                            var currentBeer = RowToBeer(reader);
                            if (!brewery.BeersServed.Any(item => item.BeerId== currentBeer.BeerId))
                            {
                                brewery.BeersServed.Add(currentBeer);
                            }
                            else
                            {
                                // see if the beer type already exists
                                var currentBeerType = RowToBeerType(reader);
                                if (!currentBeer.BeerTypes.Any(item => item.BeerTypeId == currentBeerType.BeerTypeId))
                                {
                                    currentBeer.BeerTypes.Add(currentBeerType);
                                }
                            }
                        }
                    }
                }

                return brewery;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        private Brewery MapRowToBrewery(SqlDataReader reader, bool includeBeerRows)
        {
            var brewery = new Brewery()
            {
                BreweryId = Convert.ToInt32(reader["brewery_id"]),
                BreweryName = Convert.ToString(reader["brewery_name"]),
                BreweryAddress1 = Convert.ToString(reader["address_1"]),
                BreweryAddress2 = Convert.ToString(reader["address_2"]),
                BreweryCity = Convert.ToString(reader["address_city"]),
                BreweryZip = Convert.ToString(reader["address_zip"]),
                HappyHourStartTime = Convert.ToString(reader["happyhour_start"]),
                HappyHourEndTime = Convert.ToString(reader["happyhour_end"]),
                HappyHourDetails = Convert.ToString(reader["happyhour_details"]),
                EstablishedDate = Convert.ToString(reader["established_date"]),
                Description = Convert.ToString(reader["brewery_description"]),
                IsBrewery = Convert.ToBoolean(reader["brewery"]),
                IsBar = Convert.ToBoolean(reader["bar"]),
            };

            if (includeBeerRows && RowToBeer(reader) != null)
            {
                brewery.BeersServed = new List<Beer>() { RowToBeer(reader) };
            }
            return brewery;
        }

        private Beer RowToBeer(SqlDataReader reader)
        {
            if (reader["beer_id"] != null)
            {
                var beer = new Beer()
                {
                    BeerId = Convert.ToInt32(reader["beer_id"]),
                    BeerName = Convert.ToString(reader["beer_name"]),
                    BeerUrl = Convert.ToString(reader["image_url"]),
                };

                if (RowToBeerType(reader) != null)
                {
                    beer.BeerTypes = new List<BeerType>() { RowToBeerType(reader) };
                }
                return beer;
            }
            return null;
        }

        private BeerType RowToBeerType(SqlDataReader reader)
        {
            if (reader["beertype_id"] != null)
            {
                return new BeerType
                {
                    BeerTypeId = Convert.ToInt32(reader["beertype_id"]),
                    TypeName = Convert.ToString(reader["beertype_name"])
                };
            }
            return null;
        }

        //public void UpdateBrewery(Brewery brewery)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            var query = "UPDATE Breweries SET  name = @name, address_1 = @address1, address_2 = @address2, address_city = @city, address_zip = @zip, brewery = @brewery, bar = @bar, happyhour_start = @happyhour_start, happyhour_end = @happyhour_end, happyhour_details = @happyhour_details WHERE id = @id;";
        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            cmd.Parameters.AddWithValue("@id", brewery.BreweryId);
        //            cmd.Parameters.AddWithValue("@name", brewery.BreweryName);
        //            cmd.Parameters.AddWithValue("@address_1", brewery.BreweryAddress1);
        //            cmd.Parameters.AddWithValue("@address_2", brewery.BreweryAddress2);
        //            cmd.Parameters.AddWithValue("@address_city", brewery.BreweryCity);
        //            cmd.Parameters.AddWithValue("@address_zip", brewery.BreweryZip);
        //            cmd.Parameters.AddWithValue("@brewery", brewery.IsBrewery);
        //            cmd.Parameters.AddWithValue("@bar", brewery.IsBar);
        //            cmd.Parameters.AddWithValue("@happyhour_start", brewery.HappyHourStartTime);
        //            cmd.Parameters.AddWithValue("@happyhour_end", brewery.HappyHourEndTime);
        //            cmd.Parameters.AddWithValue("@happyhour_details", brewery.HappyHourDetails);
        //            cmd.ExecuteNonQuery();
        //            return;
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        throw ex;
        //    }
        //}


        //public void CreateBrewery(Brewery brewery)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            var query = "INSERT into Breweries (name, address_1, address_2, address_city, address_zip, brewery, bar, happyhour_start, happyhour_end, happyhour_details) VALUES (@name, @address1, @address2, @city, @zip, @brewery, @bar, @happyhour_start, @happyhour_end, @happyhour_details);";
        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            cmd.Parameters.AddWithValue("@name", brewery.BreweryName);
        //            cmd.Parameters.AddWithValue("@address_1", brewery.BreweryAddress1);
        //            cmd.Parameters.AddWithValue("@address_2", brewery.BreweryAddress2);
        //            cmd.Parameters.AddWithValue("@address_city", brewery.BreweryCity);
        //            cmd.Parameters.AddWithValue("@address_zip", brewery.BreweryZip);
        //            cmd.Parameters.AddWithValue("@brewery", brewery.IsBrewery);
        //            cmd.Parameters.AddWithValue("@bar", brewery.IsBar);
        //            cmd.Parameters.AddWithValue("@happyhour_start", brewery.HappyHourStartTime);
        //            cmd.Parameters.AddWithValue("@happyhour_end", brewery.HappyHourEndTime);
        //            cmd.Parameters.AddWithValue("@happyhour_details", brewery.HappyHourDetails);
        //            cmd.ExecuteNonQuery();
        //            return;
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public void DeleteBrewery(int id)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            var query = "DELETE FROM Breweries_Beers WHERE brewery_id = @id;"
        //            + "DELETE FROM breweries WHERE id = @id;";
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
