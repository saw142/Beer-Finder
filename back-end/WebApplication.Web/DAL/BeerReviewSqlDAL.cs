using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Web.Models;

namespace WebApplication.Web.DAL
{
    public class BeerReviewSqlDAL : IBeerReviewDAL
    {
        private readonly string connectionString;

        public BeerReviewSqlDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public BeerReview GetReview(User user, Beer beer)
        {
            if (user == null || beer == null)
            {
                return new BeerReview();
            }
            var review = new BeerReview();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var query = @"SELECT TOP 1 beers.id AS beer_id, beers.name AS beer_name, beers.image_url AS image_url, beerreview.id AS review_id, beerreview.rating AS review_rating, beerreview.date_of_review AS review_date, beerreview.review AS review_text, users.id AS user_id, users.username AS user_username FROM users JOIN beerreview ON users.id = beerreview.user_id JOIN beers on beers.id = beerreview.beer_id WHERE users.id = @user_id AND beerreview.beer_id = @beer_id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user_id", user.Id);
                    cmd.Parameters.AddWithValue("@beer_id", beer.BeerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            review = MapRowToBeerReview(reader);
                        }
                    }
                }
                return review;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public void CreateReview(BeerReview beerReview)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO beerreview VALUES (@beer_id, @user_id, @rating, @review, @date);", conn);
                    cmd.Parameters.AddWithValue("@beer_id", beerReview.BeerReviewed.BeerId);
                    cmd.Parameters.AddWithValue("@user_id", beerReview.UserReviewing.Id);
                    cmd.Parameters.AddWithValue("@rating", beerReview.ReviewRating);
                    cmd.Parameters.AddWithValue("@review", beerReview.ReviewText);
                    cmd.Parameters.AddWithValue("@date", beerReview.DateOfReview.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                    cmd.ExecuteNonQuery();

                    return;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public IList<BeerReview> GetReviews(User user)
        {
            var reviews = new List<BeerReview>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var query = @"SELECT beers.id AS beer_id, beers.name AS beer_name, beers.image_url AS image_url, beerreview.id AS review_id, beerreview.rating AS review_rating, beerreview.date_of_review AS review_date, beerreview.review AS review_text, users.id AS user_id, users.username AS user_username FROM users JOIN beerreview ON users.id = beerreview.user_id JOIN beers on beers.id = beerreview.beer_id WHERE users.id = @user_id;";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user_id", user.Id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        reviews.Add(MapRowToBeerReview(reader));
                    }
                }

                return reviews;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public IList<BeerReview> GetReviews(Beer beer)
        {
            var reviews = new List<BeerReview>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var query = @"SELECT beers.id AS beer_id, beers.name AS beer_name, beers.image_url AS image_url, beerreview.id AS review_id, beerreview.rating AS review_rating, beerreview.date_of_review AS review_date, beerreview.review AS review_text, users.id AS user_id, users.username AS user_username FROM users JOIN beerreview ON users.id = beerreview.user_id JOIN beers on beers.id = beerreview.beer_id WHERE beers.id = @beer_id;";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@beer_id", beer.BeerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        reviews.Add(MapRowToBeerReview(reader));
                    }
                }

                return reviews;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public void CreateOrUpdateReview(BeerReview beerReview)
        {
            var review = GetReview(beerReview.UserReviewing, beerReview.BeerReviewed);

            if (review.Equals(new BeerReview()))
            {
                CreateReview(beerReview);
            }
            else
            {
                UpdateReview(beerReview);
            }

        }

        public void UpdateReview(BeerReview beerReview)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE beerreview SET rating = @rating, review = @review WHERE beer_id = @beer_id AND user_id = @user_id", conn);
                    cmd.Parameters.AddWithValue("@review_id", beerReview.BeerReviewId);
                    cmd.Parameters.AddWithValue("@beer_id", beerReview.BeerReviewed.BeerId);
                    cmd.Parameters.AddWithValue("@user_id", beerReview.UserReviewing.Id);
                    cmd.Parameters.AddWithValue("@rating", beerReview.ReviewRating);
                    cmd.Parameters.AddWithValue("@review", beerReview.ReviewText);

                    cmd.ExecuteNonQuery();

                    return;
                }
        }
            catch (SqlException ex)
            {
                throw ex;
            }
}

        public void DeleteReview(BeerReview beerReview)
        {
            throw new NotImplementedException();
        }

        private BeerReview MapRowToBeerReview(SqlDataReader reader)
        {
            //try
            //{
                var dt = Convert.ToString(reader["review_date"]);
                var reviewDate = DateTime.ParseExact(dt, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                var br = new BeerReview
                {
                    BeerReviewId = Convert.ToInt32(reader["review_id"]),
                    UserReviewing = MapRowToUser(reader),
                    BeerReviewed = MapRowToBeer(reader),
                    ReviewText = Convert.ToString(reader["review_text"]),
                    DateOfReview = reviewDate
                };
                br.SetRaiting(Convert.ToDouble(reader["review_rating"]));

                return br;
            //}
            //catch
            //{
                //return new BeerReview();
            //}
        }

        private Beer MapRowToBeer(SqlDataReader reader)
        {
            try
            {
                return new Beer()
                {
                    BeerId = Convert.ToInt32(reader["beer_id"]),
                    BeerName = Convert.ToString(reader["beer_name"]),
                    BeerUrl = Convert.ToString(reader["image_url"])
                };
            }
            catch
            {
                return new Beer();
            }
        }

        private User MapRowToUser(SqlDataReader reader)
        {
            try
            {
                return new User()
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
    }
}
