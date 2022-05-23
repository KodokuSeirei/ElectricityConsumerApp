CREATE TABLE [fs_region] (
  id int PRIMARY KEY IDENTITY NOT NULL,
  id_country int NOT NULL,
  id_okrug int NOT NULL,
  name varchar(250) NOT NULL,
);
