--Create Tables
create table if not exists short_urls
(
	urlid bigserial PRIMARY KEY NOT NULL,
	long_url varchar,
	shortcode varchar,
	createdate datetime default now(),
	expirydate datetime default createdate
);

create table if not exists url_access
(	
	accesid bigserial PRIMARY KEY NOT NULL,
	urlid bigint,
	accessdate datetime
);

--Create Indexes
CREATE UNIQUE INDEX urlid_idx ON short_urls (urlid);
CREATE UNIQUE INDEX accesid_idx ON url_access (accesid);