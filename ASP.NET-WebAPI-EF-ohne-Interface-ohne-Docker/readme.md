
ASP.NET-WebAPI-EF-Ohne-Interface-Ohne-Docker

# Einfache ASP.NET WebAPI-Erstellung in VS Code, ohne Interface und ohne Docker

In diesem Leitfaden erkläre ich Ihnen Schritt für Schritt, wie Sie mit Visual Studio Code und .NET 6.0 eine einfache ASP.NET WebAPI erstellen und diese über Docker bereitstellen können. Sie können das Projekt abschließen, indem Sie die Schritte befolgen.


## Datenbankstruktur - MS SQL Server


### Erstellen einer MS SQL Server-Datenbanktabelle

Verwenden Sie das folgende Skript, um die Datenbanktabelle auf MS SQL Server zu erstellen. Dieses Skript erstellt die Tabelle Person:

MS SQL Server-Verbindungsinformationen

```` text

Server: .\sqlexpress
Server: 192.168.0.150\\SQLEXPRESS

Benutzername: sa
Passwort: www

Database: DbPerson

````

```` sql
CREATE TABLE [dbo].[Person](
    [PersonId] [int] IDENTITY(1,1) NOT NULL,
      NULL,
       CONSTRAY KEY CLUSTERED 
    (
        [PersonId] ASC
    )
) ON [PRIMARY]
````

Dieses Skript erstellt die Tabelle Person mit den Feldern PersonId, Vorname, Nachname, Email und GebDatum. PersonId ist als automatisch inkrementierende Identitätsspalte definiert.


### Beispiel-Daten zur Person-Tabelle hinzufügen

Wir können der in MS SQL Server erstellten Person-Tabelle fünf Beispiel-Datensätze zu Testzwecken hinzufügen. Diese Daten gehören zu realistischen, fiktiven Personen, die wir beim Testen oder Entwickeln unserer API verwenden können. Fügen Sie diese Daten mit der folgenden INSERT-Abfrage in die Tabelle ein:

```` sql
INSERT INTO [dbo].[Person] (Vorname, Nachname, Email, GebDatum)
VALUES 
('Max', 'Mustermann', 'max.mustermann@example.com', '1992-03-15'),
('Erika', 'Musterfrau', 'erika.musterfrau@example.com', '1989-06-20'),
('Felix', 'Mustersohn', 'felix.mustersohn@example.com', '1995-09-12'),
('Anna', 'Musterfrau', 'anna.musterfrau@example.com', '1990-01-05'), 
('Lukas', 'Mustermann', 'lukas.mustermann@example.com', '1987-11-30');
````

Diese INSERT-Abfrage fügt der Person-Tabelle fünf neue Personen hinzu. Mit den Feldern Vorname, Nachname, Email und GebDatum werden der Vorname, Nachname, die E-Mail-Adresse und das Geburtsdatum jeder Person eingegeben.


### Abfrage der Beispiel-Daten in der Person-Tabelle

```` sql
SELECT * FROM Person
```` 

Ergebnis:

```` sql
1	Max	Mustermann	max.mustermann@example.com	1992-03-15
2	Erika	Musterfrau	erika.musterfrau@example.com	1989-06-20
3	Felix	Mustersohn	felix.mustersohn@example.com	1995-09-12
4	Anna	Musterfrau	anna.musterfrau@example.com	1990-01-05
5	Lukas	Mustermann	lukas.mustermann@example.com	1987-11-30
````

Diese Daten werden verwendet, um während der Entwicklung und des Testens der API zu verstehen und zu testen, wie CRUD-Operationen (Create, Read, Update, Delete) auf die Daten in der Datenbank angewendet werden.

Auf diese Weise können wir realistische Testszenarien erstellen und die Funktionalität der von uns entwickelten API überprüfen, während wir Datenbankverbindungen und Datenoperationen durchführen.


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

- appsettings.json: Datei mit Konfigurationseinstellungen wie der Datenbank-Verbindungszeichenfolge.

- Controllers/: Ordner, der die API-Controller enthält (PersonController.cs befindet sich hier).

- Data/: Ordner, der den Datenbankkontext (PersonDbContext.cs) und die Modelle, die mit den Datenbanktabellen verknüpft sind (Person.cs), enthält.

- Dockerfile: Konfigurationsdatei, um die API in einem Docker-Container auszuführen.

- PersonApi.csproj: Projektkonfigurationsdatei.


## 1. Neues Projekt erstellen
Öffnen Sie das Terminal, um ein neues Projekt zu erstellen und auf .NET 6.0 abzuzielen:

```bash
dotnet new webapi -o PersonApi --framework net6.0
```

ASP.NET-WebAPI-EF--VSCODE-DE

Wechseln Sie in das Projektverzeichnis:

```bash
cd PersonApi
```

Öffnen Sie das Projekt in Visual Studio Code:

```bash
code .
```

## 2. Port-Einstellung vornehmen
Öffnen Sie die Datei `launchSettings.json` und nehmen Sie die folgende Einstellung vor, damit das Projekt auf Port 5001 läuft:

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

Diese Einstellung sorgt dafür, dass das Projekt auf Port 5001 läuft und die Swagger-Oberfläche in der Entwicklungsumgebung automatisch geöffnet wird.

## 3. Installation des mit .NET 6.0 kompatiblen Pakets Microsoft.AspNetCore.OpenApi
Installieren Sie das Paket Swashbuckle.AspNetCore für Swagger/OpenAPI-Unterstützung, die mit .NET 6.0 kompatibel ist:

```bash
dotnet add package Swashbuckle.AspNetCore --version 6.2.3
```

Hinweis: Wenn später eine andere Version von Microsoft.AspNetCore.OpenApi installiert wurde oder das vorhandene Paket auf eine inkompatible Version aktualisiert wurde, können Sie das Paket mit folgendem Befehl entfernen:

```bash
dotnet remove package Microsoft.AspNetCore.OpenApi
```

----




## 4. Entity Framework-Installation und DB First-Einstellungen

Wir werden die notwendigen Schritte befolgen, um die Datenbank im Projekt mit Entity Framework zu verwalten.


### Installation des Entity Framework-Pakets

Installieren Sie das Entity Framework Core-Paket über NuGet:

```` terminal 

dotnet new tool-manifest
dotnet tool install dotnet-ef --version 6.0.35

dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.35
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6.0.35
dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.35

dotnet add package Microsoft.Extensions.Configuration.Json --version 6.0.0

````

Diese Pakete bieten die erforderlichen Tools für Migrationen und Konfigurationen im Projekt mit Entity Framework Core, das mit MS SQL Server kompatibel ist. Durch das Hinzufügen dieser Pakete können Sie eine vollständige EF Core-Installation im Projekt durchführen.

### Hinzufügen der Verbindungszeichenfolge zur Datei `appsettings.json`

Öffnen Sie die Datei `appsettings.json` und fügen Sie die Datenbankverbindungseinstellungen hinzu.

```` json
{

  "ConnectionStrings": {
    "DefaultConnection": "Server=192.168.0.150\\SQLEXPRESS;Database=DbPerson;User Id=sa;Password=www;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;"
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

Diese Einstellung ermöglicht die Verbindung zur Datenbank `DbPerson`.

----

### Erstellung von Model- und DbContext-Klasse mit DB First

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

### Controller-Erstellung

`Controllers/PersonController.cs` Datei erstellen:

```` csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonApi.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly PersonDbContext _context;

        public PersonController(PersonDbContext context)
        {
            _context = context;
        }

        // GET: api/Person
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPeople()
        {
            return await _context.People.ToListAsync();
        }

        // GET: api/Person/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _context.People.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        // POST: api/Person
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerson), new { id = person.PersonId }, person);
        }

        // PUT: api/Person/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(int id, Person person)
        {
            if (id != person.PersonId)
            {
                return BadRequest();
            }

            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Person/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.People.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonExists(int id)
        {
            return _context.People.Any(e => e.PersonId == id);
        }
    }
}

````

Die Klasse `PersonController` ist der Controller, der die eingehenden HTTP-Anfragen an die API verarbeitet. 
Die CRUD-Operationen (Create, Read, Update, Delete) werden OHNE `IPersonService` durchgeführt. Jede Aktion (z. B. Hinzufügen einer neuen Person oder Löschen einer Person) wird an den entsprechenden API-Endpunkt weitergeleitet und eine geeignete Antwort an den Client zurückgegeben.

 - `GET` : Ruft eine Liste aller Personen oder eine spezifische Person aufgrund ihrer ID ab.
 - `POST` : Fügt eine neue Person in die Datenbank ein.
 - `PUT`: Aktualisiert die Informationen einer bestehenden Person.
 - `DELETE`: Entfernt eine Person aus der Datenbank.

Dieser Controller verwendet die standardmäßigen HTTP-Methoden für CRUD-Operationen: GET, POST, PUT und DELETE. Die Methoden sind mit umfangreichen Kommentaren ausgestattet, um deren Funktion zu erklären.



### program.cs

```` csharp
// EINTRAGEN 
using Microsoft.EntityFrameworkCore;
using PersonApi.Data; // Namespace der DbContext-Klasse hinzufügen.

...

// EINTRAGEN - Die Verbindungszeichenfolge aus appsettings.json hinzufügen und DbContext einfügen
builder.Services.AddDbContext<PersonDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
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

- `builder.Services.AddDbContext<PersonDbContext>(...)` : Fügt die PersonDbContext-Klasse als Service hinzu und verwendet die Verbindungszeichenfolge DefaultConnection aus der Datei appsettings.json für die Datenbankverbindung.

- `options.UseSqlServer(...)` : Stellt sicher, dass Entity Framework Core SQL Server verwendet.


```bash
dotnet run
```

Testen Sie die API, indem Sie zu `http://localhost:5001/swagger/index.html` navigieren. Wenn Sie jetzt `http://localhost:5001/` aufrufen, werden Sie automatisch auf die Swagger-Seite weitergeleitet.

----




## 7. CORS-Konfiguration durch Bearbeiten der Datei `Program.cs`

Aktualisieren Sie die Datei `Program.cs` wie folgt. Mit dieser Änderung wird die Haupt-URL `http://localhost:5001/` automatisch auf die Swagger-Seite umgeleitet:

```csharp
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


// Die Verbindungszeichenfolge aus appsettings.json hinzufügen und DbContext einfügen
builder.Services.AddDbContext<PersonDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// EINTRAGEN - CORS aktivieren
app.UseCors("AllowAll"); // Erlaubt alle Domains, Methoden und Header
//app.UseCors("AllowSpecificMethods"); // Wenn GET, POST, ... angegeben sind.
//app.UseCors("AllowSpecificOrigins"); // Erlaubt nur bestimmte Domains
//app.UseCors("AllowSpecificMethodsAndHeaders"); // Erlaubt bestimmte Domains, bestimmte Header und bestimmte HTTP-Methoden

// EINTRAGEN - Configure the HTTP request pipeline.
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


### NICHT BENÖTIGTE DATEIEN LÖSCHEN

Controllers / WeatherForecastController.cs wird gelöscht.

WeatherForecast.cs wird gelöscht.

----




