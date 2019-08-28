-- Switch to the system (aka master) database
USE master;
GO

-- Delete the CleBrews Database (IF EXISTS)
IF EXISTS(select * from sys.databases where name='CleBrews')
DROP DATABASE CleBrews;
GO

-- Create a new CleBrews Database
CREATE DATABASE CleBrews;
GO

-- Switch to the DemoDB Database
USE CleBrews
GO

BEGIN TRANSACTION;

CREATE TABLE users
(
	id			int			identity(1,1),
	username	varchar(50)	not null,
	email		varchar(50) not null,
	password	varchar(50)	not null,
	salt		varchar(50)	not null,
	role		varchar(50)	default('user'),

	constraint pk_users primary key (id)
);

CREATE TABLE Breweries
(	
	id					int				identity(1,1),
	name				varchar(50)		not null,
	address_1			varchar(255) 	null,
	address_2			varchar(255)	null,
	address_city		varchar(255)	null,
	address_zip			varchar(10)		null,
	happyhour_start		varchar(255)	null,
	happyhour_end		varchar(255)	null,
	happyhour_details	varchar(255)	null,
	established_date	varchar(4)		null,
	description			varchar(3000)   null,
	brewery				Bit			not null,
	bar					Bit			not null,

	constraint pk_breweries primary key (id)
);

CREATE TABLE Beers
(	
	id					int				identity(1,1),
	name				varchar(50)		not null,
	abv					FLOAT(1)		null,
	image_url			varchar(500)	null,
	description			varchar(3000)	null

	constraint pk_beers primary key (id)
);

CREATE TABLE BeerReview
(
	id					int				identity(1,1),
	beer_id				int				not null,
	user_id				int				not null,
	rating				int				null,
	review				varchar(500)	null,
	date_of_review		varchar(250)	null,

	constraint pk_beerreviews primary key (id),
	constraint fk_beers_br foreign key (beer_id) references beers(id),
	constraint fk_users_br foreign key (user_id) references users(id),
);


--CREATE TABLE BeerReviewsUsers
--(
--	id					int				identity(1,1),
--	beerreview_id		int				not null,
--	user_id				int				not null,

--	constraint pk_beerreviews_users primary key (id),
--	constraint fk_beers_bru foreign key (beerreview_id) references beers(id),
--	constraint fk_users_bru foreign key (user_id) references users(id),
--)

--CREATE TABLE BeerReviewsBeers
--(
--	id					int				identity(1,1),
--	beer_id				int				not null,
--	user_id				int				not null,

--	constraint pk_beerreviews_beers primary key (id),
--	constraint fk_beers_brb foreign key (beer_id) references beers(id),
--	constraint fk_users_brb foreign key (user_id) references users(id),
--)

CREATE TABLE BeerTypes
(
	id					int				identity(1,1),
	name				varchar(50)		not null,
	
	constraint pk_beertypes primary key (id)
)

CREATE TABLE Breweries_Beers
(
	id					int				identity(1,1),
	brewery_id				int				not null,
	beer_id				int				not null,

	constraint pk_breweries_beers primary key (id),
	constraint fk_breweries foreign key (brewery_id) references breweries(id),
	constraint fk_beers foreign key (beer_id) references Beers(id)
);

CREATE TABLE Beers_BeerTypes
(
	id					int				identity(1,1),
	beer_id				int				not null,
	beertype_id			int				not null,

	constraint pk_beers_beertypes primary key (id),
	constraint fk_beers_bt foreign key (beer_id) references beers(id),
	constraint fk_beertypes_bt foreign key (beertype_id) references beertypes(id),
)

COMMIT TRANSACTION;