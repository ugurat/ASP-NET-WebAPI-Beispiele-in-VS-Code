
ASP.NET-WebAPI-EF-Docker-VSCODE-DE-readme

# ASP.NET WebAPI-Erstellung mit Entity Framework, Interface und Docker

In diesem Leitfaden erkläre ich Schritt für Schritt, wie Sie mit Visual Studio Code und .NET 6.0 eine einfache ASP.NET WebAPI erstellen und diese API auf Docker veröffentlichen können. Folgen Sie den Schritten, um das Projekt abzuschließen.


## Datenbankstruktur - MS SQL Server


### MS SQL Server-Datenbanktabelle erstellen

Verwenden Sie das folgende Skript, um eine Datenbanktabelle in MS SQL Server zu erstellen. Dieses Skript erstellt die Person-Tabelle:

MS SQL Server-Verbindungsinformationen

```` bash

Server: .\sqlexpress
Server: 192.168.0.150\\SQLEXPRESS

Benutzername: sa
Passwort: www

Database: DbPerson

````


```` bash
Server: 192.168.0.100\SQLEXPRESS
WIN-SJQ692QAABC\SQLEXPRESS
Benutzername: sa
Passwort: www
````

Achtung:

Um eine Verbindung zum SQL Server über die IP-Adresse herzustellen, müssen zunächst unter Konfiguration die SQL Server-Netzwerkeinstellungen `Shared Memory`, `Named Pipes` und `TCP/IP` aktiviert werden. Der `SQL Server Browser`-Dienst muss laufen und der `UDP 1434`-Port muss geöffnet sein.



Datenbank erstellen.

```` sql

CREATE DATABASE DbPerson
GO

USE DbPerson
GO

````

Person-Tabelle erstellen.

```` sql
-- Skript zum Erstellen der Tabelle
CREATE TABLE [dbo].[Person] (
    [PersonId] INT IDENTITY(1,1) NOT NULL,
    [Vorname] NVARCHAR(50) NOT NULL,        -- Vorname
    [Nachname] NVARCHAR(50) NOT NULL,       -- Nachname
    [Email] NVARCHAR(100) NOT NULL,         -- E-Mail
    [GebDatum] DATE NOT NULL,               -- Geburtsdatum
    CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
    (
        [PersonId] ASC
    )
) ON [PRIMARY];
````

Dieses Skript erstellt die Person-Tabelle mit den Feldern PersonId, Vorname, Nachname, Email und GebDatum. PersonId ist als automatisch inkrementierende Identitätsspalte definiert.

### Beispielhafte Dateneingabe in die Person-Tabelle

In die erstellte Person-Tabelle in MS SQL Server können wir zu Testzwecken 5 Beispielaufzeichnungen einfügen. Diese Daten gehören zu realistischen, fiktiven Personen, die wir beim Testen oder während der Entwicklung unserer API verwenden können. Fügen Sie diese Daten mit der folgenden INSERT-Abfrage in die Tabelle ein:

```` sql
-- Beispiel Daten
INSERT INTO [dbo].[Person] (Vorname, Nachname, Email, GebDatum)
VALUES 
('Max', 'Mustermann', 'max.mustermann@example.com', '1990-05-14'),
('Erika', 'Musterfrau', 'erika.musterfrau@example.com', '1985-03-23'),
('Felix', 'Mustersohn', 'felix.mustersohn@example.com', '2000-08-09'),
('Hans', 'Müller', 'hans.mueller@example.com', '1982-11-19'),
('Anna', 'Schmidt', 'anna.schmidt@example.com', '1993-07-01');
````

Diese INSERT-Abfrage fügt fünf neue Personen in die Person-Tabelle ein.
Die Felder Vorname, Nachname, Email und GebDatum enthalten jeweils den Vornamen, Nachnamen, die E-Mail-Adresse und das Geburtsdatum jeder Person.


### Abfrage der Beispiel-Daten aus der Person-Tabelle

```` sql
SELECT * FROM Person
```` 

Ergebnis:

```` sql
1	Max	Mustermann	max.mustermann@example.com	1990-05-14
2	Erika	Musterfrau	erika.musterfrau@example.com	1985-03-23
3	Felix	Mustersohn	felix.mustersohn@example.com	2000-08-09
4	Hans	Müller	hans.mueller@example.com	1982-11-19
5	Anna	Schmidt	anna.schmidt@example.com	1993-07-01
````

Diese Daten werden während der Entwicklung und des Testens der API verwendet, um zu verstehen und zu testen, wie CRUD-Operationen (Create, Read, Update, Delete) mit den Daten in der Datenbank funktionieren.

Auf diese Weise können wir realistische Testszenarien erstellen und die Funktionalität der entwickelten API während der Datenbankverbindungen und Datenoperationen überprüfen.


## Ordnerstruktur
Ihre Projektordnerstruktur sollte wie folgt aussehen:

```
Proje/
    ├── PersonApi/
        ├── Controllers/
        │       └── PersonController.cs
        ├── Data/
        │       ├── PersonDbContext.cs
        │       └── Person.cs
        ├── appsettings.json
        ├── Dockerfile
        └── PersonApi.csproj
```

- appsettings.json: Datei, die Konfigurationseinstellungen wie die Verbindungszeichenfolge zur Datenbank enthält.

- Controllers/: Ordner, der die API-Controller enthält (PersonController.cs befindet sich hier).

- Data/: Ordner, der den Datenbankkontext (PersonDbContext.cs) und die mit den Datenbanktabellen verbundenen Modelle (Person.cs) enthält.

- Dockerfile: Konfigurationsdatei, um die API in einem Docker-Container auszuführen.

- PersonApi.csproj: Projektkonfigurationsdatei.


## 1. Neues Projekt erstellen
Öffnen Sie das Terminal, um ein neues Projekt zu erstellen und .NET 6.0 als Ziel festzulegen:

```bash
dotnet new webapi -o PersonApi --framework net6.0
```

Wechseln Sie in das Projektverzeichnis:

```bash
cd PersonApi
```

Öffnen Sie das Projekt in Visual Studio Code:

```bash
code .
```

## 2. Port-Einstellungen vornehmen
Öffnen Sie die Datei `launchSettings.json` und nehmen Sie die folgenden Einstellungen vor, damit das Projekt auf Port 5001 läuft:

```json
"profiles": {
  "PersonApi": {
    "commandName": "Project",
    "dotnetRunMessages": true,
    "launchBrowser": true,
    "launchUrl": "swagger",
    "applicationUrl": "https://localhost:6001;http://localhost:5001",
    "environmentVariables": {
      "ASPNETCORE_ENVIRONMENT": "Development"
    }
  }
}
```

Diese Einstellung sorgt dafür, dass das Projekt auf Port 5001 läuft und die Swagger-Oberfläche im Entwicklungsmodus automatisch geöffnet wird.

## 3. Installation des mit .NET 6.0 kompatiblen Microsoft.AspNetCore.OpenApi-Pakets
Installieren Sie das Swashbuckle.AspNetCore-Paket für Swagger/OpenAPI-Unterstützung, das mit .NET 6.0 kompatibel ist:

```bash
dotnet add package Swashbuckle.AspNetCore --version 6.2.3
```

Hinweis: Falls eine andere Version des Microsoft.AspNetCore.OpenApi-Pakets installiert wurde oder das aktuelle Paket auf eine inkompatible Version aktualisiert wird, können Sie das Paket mit folgendem Befehl entfernen:

```bash
dotnet remove package Microsoft.AspNetCore.OpenApi
```

----

## 4. Entity Framework-Installation und DB-First-Einstellungen

Wir werden die erforderlichen Schritte befolgen, um die Datenbank mit Entity Framework im Projekt zu verwalten.


### Installation des Entity Framework-Pakets

Installieren Sie das Entity Framework Core-Paket über NuGet:

```` bash 

dotnet new tool-manifest
dotnet tool install dotnet-ef --version 6.0.35

dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.35
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6.0.35
dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.35

dotnet add package Microsoft.Extensions.Configuration.Json --version 6.0.0

````

Diese Pakete bieten die erforderlichen Tools für Migrationen und Konfigurationen im Projekt mit Entity Framework Core, das mit MS SQL Server kompatibel ist. Durch das Hinzufügen dieser Pakete können Sie eine vollständige EF Core-Installation im Projekt durchführen.

### Aktualisieren der Datei `appsettings.json` zur Verwendung von Umgebungsvariablen

Konfigurieren Sie die Datei `appsettings.json`, um die Verbindungszeichenfolge nicht statisch anzugeben, sondern auf Umgebungsvariablen basierend. .NET Core-Anwendungen können Umgebungsvariablen lesen und diese durch die Werte in der Datei `appsettings.json` ersetzen. Zum Beispiel:

```` json
{

  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\sqlexpress;Database=DbPerson;User Id=sa;Password=www;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

````

Hinweis: `Trust Server Certificate=True` sorgt dafür, dass der Client-Computer dem vom Server bereitgestellten Zertifikat vertraut. Diese Methode kann jedoch Sicherheitsrisiken mit sich bringen. Wenn Sie es in einer Produktionsumgebung verwenden, ist es sicherer, ein echtes Zertifikat zu verwenden.


### Später entfernen!!!
Mit dieser Einstellung können wir eine Verbindung zur Datenbank namens „DbPerson“ herstellen.
Beispiel: „Server=.\\sqlexpress;Database=DbPerson;Benutzer-ID=sa;Passwort=www;“ `

```` bash
Server=192.168.0.150\\SQLEXPRESS;Database=DbPerson;User Id=sa;Password=www;
````

Das funktioniert: 
```` bash
"DefaultConnection": "Server=192.168.0.150\\SQLEXPRESS;Database=DbPerson;User Id=sa;Password=www;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;"
````

----

### Erstellung von Model und DbContext-Klasse mit DB First

Führen Sie den folgenden Befehl aus, um basierend auf der in der Datenbank vorhandenen Person-Tabelle automatisch das Model und die DbContext-Klasse zu erstellen:


Zugriff auf die Datenbank über appsettings.json:

```` bash
dotnet ef dbcontext scaffold Name=DefaultConnection Microsoft.EntityFrameworkCore.SqlServer -o Data -c PersonDbContext --force
````


Der `scaffold`-Befehl erstellt die Modelklassen und die `DbContext`-Klasse aus der vorhandenen Datenbank.

 - -o Data: Sorgt dafür, dass die Models und der DbContext im Ordner Data gespeichert werden.
 - -c PersonDbContext: Der Name der DbContext-Klasse wird auf PersonDbContext festgelegt.
 - --force: Aktualisiert vorhandene Dateien, indem sie neu erstellt werden.


Wenn dieser Befehl ausgeführt wird, werden das auf der Person-Tabelle basierende Person-Model und die PersonDbContext-Klasse automatisch im Ordner Data erstellt. Diese Strukturen werden entsprechend der Tabellenstruktur in der Datenbank erstellt und können für CRUD-Operationen verwendet werden.


#### Weitere Optionen

Update (falls später erforderlich):

````
dotnet tool update dotnet-ef --local --version 6.0.35
````

Zugriff auf die Datenbank über eine feste Verbindungszeichenfolge:

````
dotnet ef dbcontext scaffold "Server=.\\sqlexpress;Database=PersonDb;User Id=sa;Password=www;" Microsoft.EntityFrameworkCore.SqlServer -o Data -c PersonDbContext --force
````



### program.cs

```` csharp
// EINTRAGEN 
using Microsoft.EntityFrameworkCore;
using PersonApi.Data; // Fügen Sie den Namespace hinzu, in dem sich die DbContext-Klasse befindet.

...

// EINTRAGEN - Die Verbindungszeichenfolge wird aus der Datei appsettings.json entnommen.
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

// EINTRAGEN - Die aktualisierte Verbindungszeichenfolge wird dem DbContext hinzugefügt
builder.Services.AddDbContext<PersonDbContext>(options =>
    options.UseSqlServer(connStr)
);

// Add services to the container.
builder.Services.AddControllers();

...


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// EINTRAGEN - Haupt-URL auf die Swagger-Oberfläche umleiten
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }
    await next();
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

````

- `builder.Services.AddDbContext<PersonDbContext>(...)` : Fügt die PersonDbContext-Klasse als Dienst hinzu und verwendet die Verbindungszeichenfolge DefaultConnection aus der Datei appsettings.json für die Datenbankverbindung.

- `options.UseSqlServer(...)` : Stellt sicher, dass Entity Framework Core SQL Server verwendet.


```bash
dotnet run
```

Testen Sie die API, indem Sie zu `http://localhost:5001/swagger/index.html` navigieren. Wenn Sie jetzt `http://localhost:5001/` aufrufen, werden Sie automatisch auf die Swagger-Seite weitergeleitet.


Falls **Keine Startseite** vorhanden, URL manuell öffnen:

```` bash
http://localhost:5001/swagger/index.html
````

----


## 7. CORS-Konfiguration durch Bearbeiten der Datei `Program.cs`

`Program.cs` dosyasını aşağıdaki gibi güncelleyin. 
Mit dieser Änderung wird die Haupt-URL `http://localhost:5001/` automatisch auf die Swagger-Seite umgeleitet, und die Zugriffserlaubnisse werden durch die CORS-Konfiguration verwaltet:

```csharp

...

var builder = WebApplication.CreateBuilder(args);

// EINTRAGEN - CORS-Konfiguration hinzufügen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
                // erlaubte Domains:
                .AllowAnyOrigin() // Erlaubt alle Quellen (Domains). // Dies bedeutet, dass auf die API von jeder Domain aus zugegriffen werden kann.
                //.WithOrigins("https://www.ornek1.com", "https://www.ornek2.com") // Erlaubt nur die angegebenen Domains

                // erlaubte Methoden:
                .AllowAnyMethod() // Erlaubt alle HTTP-Methoden (z. B. GET, POST, PUT, DELETE). // Dies bedeutet, dass alle HTTP-Anfragen an die API akzeptiert werden.
                //.WithMethods("GET", "POST") // Erlaubt nur die Methoden GET und POST

                // erlaubte Header:
                .AllowAnyHeader() // Erlaubt alle HTTP-Anfrage-Header. // Diese Einstellung ermöglicht es, jeder Anfrage an die API beliebige Header hinzuzufügen.
                //.WithHeaders("Content-Type", "Authorization"); // Erlaubt nur die angegebenen Header
                ;        
                
    });
});

...


var app = builder.Build();


// EINTRAGEN - CORS aktivieren
app.UseCors("AllowAll"); // Erlaubt alle Domains, Methoden und Header
//app.UseCors("AllowSpecificMethods"); // Wenn GET, POST, ... angegeben sind.
//app.UseCors("AllowSpecificOrigins"); // Erlaubt nur bestimmte Domains
//app.UseCors("AllowSpecificMethodsAndHeaders"); // Erlaubt bestimmte Domains, bestimmte Header und bestimmte HTTP-Methoden

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Haupt-URL auf die Swagger-Oberfläche umleiten
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }
    await next();
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```


### WithHeaders - häufig verwendete Header 

Die folgenden häufig verwendeten Header können je nach Bedarf zugelassen werden:

```` csharp
.WithHeaders("Accept", "Accept-Language", "Cache-Control", "User-Agent", "Origin", "X-Requested-With", "Authorization", "Content-Type", "Cookie", "Referer");
````
Erläuterungen:

- `Accept`: Gibt den Inhaltstyp an, den der Client akzeptieren kann.
- `Accept-Language`: Gibt die bevorzugte Spracheinstellung des Clients an.
- `Cache-Control`: Gibt die Caching-Richtlinie an.
- `User-Agent`: Gibt Informationen über den anfragenden Client an.
- `Origin`: Gibt die Domain an, von der die Anfrage stammt.
- `X-Requested-With`: Definiert AJAX-Anfragen.
- `Authorization`: Authentifizierungs-Header.
- `Content-Type`: Gibt den Typ des gesendeten Inhalts an.
- `Cookie`: Enthält Cookie-Informationen.
- `Referer`: Gibt an, von welcher Seite die Anfrage stammt.


## 8. Projekt kompilieren und testen
Führen Sie das Projekt im Terminal aus:

```bash
dotnet run
```

Testen Sie die API, indem Sie die Adresse `http://localhost:5001/swagger/index.html` mit Swagger aufrufen. Wenn Sie jetzt die Adresse `http://localhost:5001/` aufrufen, werden Sie automatisch auf die Swagger-Seite umgeleitet.


----



## 5. Person API 

### Interface-Erstellung

`/Services/IPersonService.cs` Datei erstellen:

```` csharp
using PersonApi.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonApi.Services
{
    public interface IPersonService
    {
        Task<List<Person>> GetAllPersonsAsync();
        Task<Person?> GetPersonByIdAsync(int id);
        Task AddPersonAsync(Person person);
        Task UpdatePersonAsync(Person person);
        Task DeletePersonAsync(int id);
    }
}
````


### Service-Erstellung

`/Services/PersonService.cs` dosyasını oluşturun:

```` csharp
using PersonApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonApi.Services
{
    public class PersonService : IPersonService
    {
        private readonly PersonDbContext _context;

        public PersonService(PersonDbContext context)
        {
            _context = context;
        }

        public async Task<List<Person>> GetAllPersonsAsync()
        {
            return await _context.People.ToListAsync();
        }

        public async Task<Person?> GetPersonByIdAsync(int id)
        {
            return await _context.People.FindAsync(id);
        }

        public async Task AddPersonAsync(Person person)
        {
            _context.People.Add(person);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePersonAsync(Person person)
        {
            _context.People.Update(person);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePersonAsync(int id)
        {
            var person = await _context.People.FindAsync(id);
            if (person != null)
            {
                _context.People.Remove(person);
                await _context.SaveChangesAsync();
            }
        }
    }
}
````

Die Klasse `PersonService` implementiert das Interface `IPersonService` und definiert CRUD-Operationen. Diese Klasse greift über `PersonDbContext` auf die Datenbank zu und bietet CRUD-Funktionalität für `Person`-Objekte. Die Datenbankoperationen werden asynchron ausgeführt, um die Leistung zu verbessern.


### Controller-Erstellung

`Controllers/PersonController.cs` dosyasını oluşturun:

```` csharp
using Microsoft.AspNetCore.Mvc;
using PersonApi.Data;
using PersonApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Alle Personen abrufen.
        /// </summary>
        /// <returns>Liste von Personen</returns>
        [HttpGet]
        public async Task<ActionResult<List<Person>>> Get()
        {
            var persons = await _personService.GetAllPersonsAsync();
            return Ok(persons);
        }

        /// <summary>
        /// Eine spezifische Person anhand der ID abrufen.
        /// </summary>
        /// <returns>Person-Objekt</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetById(int id)
        {
            var person = await _personService.GetPersonByIdAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        /// <summary>
        /// Neue Person hinzufügen.
        /// </summary>
        /// <param name="person">Person-Daten</param>
        /// <returns>Resultat der Operation</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Person person)
        {
            if (person == null)
            {
                return BadRequest("Personendaten sind ungültig.");
            }

            await _personService.AddPersonAsync(person);
            return CreatedAtAction(nameof(GetById), new { id = person.PersonId }, person);
        }

        /// <summary>
        /// Vorhandene Person aktualisieren.
        /// </summary>
        /// <param name="id">ID der Person</param>
        /// <param name="person">Aktualisierte Person-Daten</param>
        /// <returns>Resultat der Operation</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Person person)
        {
            if (person == null || person.PersonId != id)
            {
                return BadRequest();
            }

            var existingPerson = await _personService.GetPersonByIdAsync(id);
            if (existingPerson == null)
            {
                return NotFound();
            }

            await _personService.UpdatePersonAsync(person);
            return NoContent();
        }

        /// <summary>
        /// Person löschen.
        /// </summary>
        /// <param name="id">ID der zu löschenden Person</param>
        /// <returns>Resultat der Operation</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingPerson = await _personService.GetPersonByIdAsync(id);
            if (existingPerson == null)
            {
                return NotFound();
            }

            await _personService.DeletePersonAsync(id);
            return NoContent();
        }
    }
}
````

Die Klasse `PersonController` ist der Controller, der die eingehenden HTTP-Anfragen an die API verarbeitet. Die CRUD-Operationen (Create, Read, Update, Delete) werden über `IPersonService` durchgeführt. Jede Aktion (z. B. Hinzufügen einer neuen Person oder Löschen einer Person) wird an den entsprechenden API-Endpunkt weitergeleitet und eine geeignete Antwort an den Client zurückgegeben.

 - `GET` : Ruft eine Liste aller Personen oder eine spezifische Person aufgrund ihrer ID ab.
 - `POST` : Fügt eine neue Person in die Datenbank ein.
 - `PUT`: Aktualisiert die Informationen einer bestehenden Person.
 - `DELETE`: Entfernt eine Person aus der Datenbank.

Dieser Controller verwendet die standardmäßigen HTTP-Methoden für CRUD-Operationen: GET, POST, PUT und DELETE. Die Methoden sind mit umfangreichen Kommentaren ausgestattet, um deren Funktion zu erklären.


### `Program.cs` Datei bearbeiten

Aktualisieren Sie die Datei `Program.cs` wie folgt. 

```` csharp
using Microsoft.EntityFrameworkCore;
using PersonApi.Data; // Fügen Sie den Namespace hinzu, in dem sich die DbContext-Klasse befindet.
using PersonApi.Services; // <-- EINTRAGEN 

...

// Die Verbindungszeichenfolge wird aus der Datei appsettings.json entnommen.
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");


// Die aktualisierte Verbindungszeichenfolge wird dem DbContext hinzugefügt
builder.Services.AddDbContext<PersonDbContext>(options =>
    options.UseSqlServer(connStr)
);

// EINTRAGEN - Service ohne Interface
// builder.Services.AddScoped<PersonService>(); // Register PersonService

// EINTRAGEN - Service mit Interface
builder.Services.AddScoped<IPersonService, PersonService>(); // Register IPersonService und PersonService


// Add services to the container.
builder.Services.AddControllers();

...


var app = builder.Build();


// EINTRAGEN - CORS'u etkinleştir
app.UseCors("AllowAll"); // Tüm domainler, yöntemler ve başlıklara izin ver
//app.UseCors("AllowSpecificMethods"); // Eger GET, POST, ... verilmis ise.
//app.UseCors("AllowSpecificOrigins"); // Sadece belirli domainlere izin ver
//app.UseCors("AllowSpecificMethodsAndHeaders"); // Belirli domainler, belirli başlıklar ve belirli HTTP yöntemlerine izin ver

...

````

 - `builder.Services.AddScoped<PersonService>();` : Diese Zeile registriert die Klasse PersonService direkt als Service und ermöglicht die Verwendung ohne Interface. Bei dieser Methode wird die Klasse PersonService direkt injiziert, wenn Dependency Injection verwendet wird. Diese Methode kann gewählt werden, wenn kein Interface erforderlich ist.

 - `builder.Services.AddScoped<IPersonService, PersonService>();` : Diese Zeile registriert das Interface IPersonService zusammen mit der Klasse PersonService. Dadurch wird bei der Dependency Injection eine Instanz der Klasse PersonService bereitgestellt, wenn der Typ IPersonService verwendet wird. Diese Methode bietet Flexibilität und erleichtert das Management von Abhängigkeiten.



```bash
dotnet run
```

Damit wird die Person API vollständig funktionsfähig. Sie können nun die CRUD-Funktionalitäten der API testen.


### NICHT BENÖTIGTE DATEIEN LÖSCHEN

Sie können die automatisch erstellten Dateien während der Projekterstellung löschen. 

`/WeatherForecast.cs` wird gelöscht.

`/Controllers/WeatherForecastController.cs` wird gelöscht.

----



## 9. Docker-Datei erstellen

### `Docker`
Erstellen Sie im Projektstammverzeichnis eine Datei mit dem Namen `Dockerfile` und fügen Sie den folgenden Inhalt hinzu:

```bash
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Definieren von Umgebungsvariablen
#ENV DB_CONNECTION="Server=.\\sqlexpress;Database=DbPerson;User Id=sa;Password=www;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;"
ENV CONNECTIONSTRING="Server=192.168.0.150\\sqlexpress;Database=DbPerson;User Id=sa;Password=www;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;"
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./*.csproj ./
RUN dotnet restore "./PersonApi.csproj"

# Alle Quellcodes kopieren und das Projekt bauen
COPY . ./
RUN dotnet build "./PersonApi.csproj" -c Release -o /app/build

# Bereitstellen für die Veröffentlichung
FROM build AS publish
RUN dotnet publish "./PersonApi.csproj" -c Release -o /app/publish

# Erstellen des endgültigen Images und Festlegen des Arbeitsverzeichnisses
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PersonApi.dll"]
```


### `Program.cs`-Datei bearbeiten

Aktualisieren Sie die Datei `Program.cs` wie folgt. 

Das Ersetzen der `CONNECTIONSTRING`-Informationen aus der Dockerfile-Datei oder den Laufzeitargumenten durch die in der Datei `appsettings.json` angegebenen `ConnectionStrings`-Informationen erfolgt in `Program.cs`.


```` csharp

...

// Die Verbindungszeichenfolge wird aus der Datei appsettings.json entnommen.
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");


// EINTRAGEN - Logging nur in der Entwicklungsumgebung
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("------");
    Console.WriteLine($"ConnStr appsettings: {connStr}"); // appsettings.json'dan gelen
    Console.WriteLine("------");
}

// EINTRAGEN - Verbindungszeichenfolge wird aus Umgebungsvariablen gelesen, falls vorhanden, andernfalls aus appsettings.json verwendet.
connStr = Environment.GetEnvironmentVariable("CONNECTIONSTRING") ?? connStr;

// EINTRAGEN - Sadece gelistirme ortaminda loglama
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine($"ConnStr Dockerfile: {connStr}"); // Ortam değişkenlerininden gelen
    Console.WriteLine("------");
}


// Aktualisierte Verbindungszeichenfolge wird dem DbContext hinzugefügt
builder.Services.AddDbContext<PersonDbContext>(options =>
    options.UseSqlServer(connStr)
);

...

````

Starten Sie die Anwendung neu. 

```bash
dotnet run
```

URL zum Testen  [http://localhost:5001/](http://localhost:5001/)

Auf diese Weise wird die Anwendung die Datenbankverbindung zunächst über `appsettings.json` beziehen und dann, falls in der `Dockerfile` oder beim Starten des Docker-Containers eine `CONNECTIONSTRING`-Information angegeben wurde, diese übernehmen. Dadurch erhält die Anwendung eine flexible Verbindungszeichenfolgenstruktur (ConnectionString). 

----


## 10. Docker-Image erstellen und ausführen

Terminalde Docker image oluşturmak için:

```bash
# Docker-Image erstellen
docker build -t personapi .
```

Starten Sie den Docker-Container und setzen Sie die Arbeitsumgebung auf "Development":


```bash
# Docker-Container starten und die Umgebung auf Development setzen
docker run -d -p 5001:80 --name personapi_container personapi
```

URL zum Testen  [http://localhost:5001/](http://localhost:5001/) .


Wenn Sie den Docker-Container  mit einer anderen CONNECTIONSTRING-Information starten möchten:

```bash
docker run -d -p 5001:80 -e CONNECTIONSTRING="Server=192.168.0.100\\sqlexpress;Database=DbPerson;User Id=sa;Password=sa.2025$;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;" personapi
```

oder indem die Umgebung auf `Development` gesetzt wird:

```bash
# Docker container başlatın ve ortamı Development olarak ayarlayın
docker run -d -p 5001:80 --name personapi_container -e ASPNETCORE_ENVIRONMENT=Development personapi
```

Dieser Befehl setzt die Arbeitsumgebung auf Development und aktiviert die Swagger-Oberfläche.


Alle Befehle zusammen:

```` bash 
dotnet clean
dotnet run

docker build -t personapi .

docker stop personapi_container
docker rm personapi_container

docker run -d -p 5001:80 personapi

docker run -d -p 5001:80 -e CONNECTIONSTRING="Server=192.168.0.100\\sqlexpress;Database=DbPerson;User Id=sa;Password=sa.2025$;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;" personapi

# docker run -d -p 5001:80 -e CONNECTIONSTRING="Server=192.168.0.100\\sqlexpress;Database=DbPerson;User Id=sa;Password=sa.2025$;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;" ASPNETCORE_ENVIRONMENT=Development personapi

````


Hinweis: Wenn bereits ein Container mit demselben Namen (`personapi_container`) läuft, müssen Sie diesen Container stoppen und löschen. Andernfalls erhalten Sie einen Namenskonfliktfehler. Verwenden Sie die folgenden Befehle, um den vorhandenen Container zu stoppen und zu löschen:

```bash
docker stop personapi_container
docker rm personapi_container
```

Zugriff auf Swagger testen:

Nachdem der Container erfolgreich gestartet wurde, gehen Sie zu [http://localhost:5001/swagger/index.html](http://localhost:5001/swagger/index.html), um den Zugriff auf die Swagger-Oberfläche zu überprüfen.

----

## 11. Code-Updates auf Docker-Container übertragen
Wenn Sie in Ihrem Projekt ein Update durchführen müssen und dieses Update im Docker-Container widerspiegeln möchten, führen Sie die folgenden Schritte aus:

1. **Nehmen Sie die erforderlichen Code-Änderungen vor**: Nehmen Sie Änderungen an Ihren Projektdateien vor und schließen Sie die Updates ab.

2. **Neues Docker-Image erstellen**: Erstellen Sie erneut ein Docker-Image, um den neuen Code widerzuspiegeln. Verwenden Sie dazu den zuvor verwendeten `docker build`-Befehl:

```bash
# Neues Docker-Image erstellen
docker build -t personapi .
```

Dieser Befehl erstellt Ihr vorhandenes Docker-Image mit dem aktualisierten Code neu.


3. **Stoppen und löschen Sie den alten Container**: Stoppen und löschen Sie den vorhandenen Container.

```bash
# Alten Container stoppen und löschen
docker stop personapi_container
docker rm personapi_container
```

4. **Neuen Container erstellen**: Starten Sie den Container mit dem neuen Image neu.

```bash
# Neuen Container erstellen
docker run -d -p 5001:80 --name personapi_container -e ASPNETCORE_ENVIRONMENT=Development personapi
```

Dieser Schritt startet einen aktualisierten Container mit dem neuen Docker-Image.

Durch Befolgen dieser Schritte können Sie Ihr Projekt erfolgreich erstellen, mit Docker ausführen, aktualisieren und über Portainer verwalten.

-----


## 12. Docker-Image exportieren und in Portainer hochladen
Wenn Sie das erstellte Docker-Image über Portainer hochladen möchten, müssen Sie dieses Image exportieren und eine `.tar`-Datei erstellen. Verwenden Sie dazu den folgenden Befehl:

```bash
docker save -o personapi_image.tar personapi
```

Achtung! Stellen Sie sicher, dass die Image-Datei nach der Erstellung korrekt hochgeladen wird. Achten Sie außerdem darauf, dass der Speicherort der Datei, die über Portainer hochgeladen wird, korrekt angegeben ist.

- Melden Sie sich bei Portainer an und gehen Sie zum Abschnitt "Images".
- Klicken Sie auf die Option "Upload Image" und wählen Sie die Datei `personapi_image.tar` aus.
- Nach Abschluss des Uploads können Sie mit diesem Image einen Container erstellen.

-----


## Docker-Image auf Docker-HUB hochladen

Diese Anweisung finden Sie im Abschnitt [Docker-to-HUB-Datei](Docker-to-HUB.readme.md). 


-----


