

------

Server: .\sqlexpress
Server: 192.168.0.150\\SQLEXPRESS

Benutzername: sa
Passwort: www

Database: DbPerson

-------

CREATE TABLE [dbo].[Person](
    [PersonId] [int] IDENTITY(1,1) NOT NULL,
      NULL,
       CONSTRAY KEY CLUSTERED 
    (
        [PersonId] ASC
    )
) ON [PRIMARY]

------

INSERT INTO [dbo].[Person] (Vorname, Nachname, Email, GebDatum)
VALUES 
('Max', 'Mustermann', 'max.mustermann@example.com', '1992-03-15'),
('Erika', 'Musterfrau', 'erika.musterfrau@example.com', '1989-06-20'),
('Felix', 'Mustersohn', 'felix.mustersohn@example.com', '1995-09-12'),
('Anna', 'Musterfrau', 'anna.musterfrau@example.com', '1990-01-05'), 
('Lukas', 'Mustermann', 'lukas.mustermann@example.com', '1987-11-30');


------

SELECT * FROM Person

------

