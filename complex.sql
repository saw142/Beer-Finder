DECLARE @user_id INT = 1;
SELECT beers.id AS beer_id, beers.name AS beer_name, beers.image_url AS image_url, beerreview.id AS review_id, beerreview.rating AS review_rating, beerreview.date_of_review AS review_date, beerreview.review AS review_text, users.id AS user_id, users.username AS user_username FROM users JOIN beerreview ON users.id = beerreview.user_id JOIN beers on beers.id = beerreview.beer_id WHERE users.id = @user_id;

SELECT * from beerreview