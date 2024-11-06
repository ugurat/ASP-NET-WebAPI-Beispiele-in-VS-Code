

# Einfache ASP.NET WebAPI Erstellung mit VS Code und Veröffentlichung mit Docker

In diesem Leitfaden werde ich Schritt für Schritt erklären, wie man mit Visual Studio Code und .NET 6.0 eine einfache ASP.NET WebAPI erstellt und diese API auf Docker veröffentlicht. Im Projekt wird ein Person-Modell mit einigen Beispielwerten über den PersonController aufgelistet.

Durch das Befolgen der Schritte im Leitfaden können Sie das Projekt mit Visual Studio Code einrichten und über Docker veröffentlichen.

## Ordnerstruktur
Ihre Projektordnerstruktur sollte wie folgt aussehen:

```
Projekt/
    ├── PersonApi/
        ├── Controllers/
        │       └── PersonController.cs
        ├── Models/
        │       └── Person.cs
        ├── Services/
        │       └── PersonService.cs
        ├── Dockerfile
        └── PersonApi.csproj
```


## 1. Neues Projekt erstellen
Öffnen Sie das Terminal, um ein neues Projekt zu erstellen und .NET 6.0 als Ziel festzulegen:

Bsp: `C:\Projekt\>_`

```bash
dotnet new webapi -o PersonApi --framework net6.0
```

--framework net6.0

Wechseln Sie in das Projektverzeichnis:

```bash
cd PersonApi
```

Öffnen Sie das Projekt in Visual Studio Code:

```bash
code .
```

## 2. Port-Einstellungen vornehmen
Öffnen Sie die Datei `/Properties/launchSettings.json` und nehmen Sie die folgenden Einstellungen vor, damit das Projekt auf Port 5001 läuft:

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

## 4. Erstellung des Person-Modells
Nach diesen Einstellungen können Sie das Modell für das Projekt erstellen.

Erstellen Sie einen Ordner namens `Models`.

Erstellen Sie die Datei `Models/Person.cs` und fügen Sie folgenden Code hinzu:

```csharp
namespace PersonApi.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public int Alter { get; set; }
    }
}
```

## 5. PersonService Klasse und Beispielobjekte
Erstellen Sie die Datei `Services/PersonService.cs`:

```csharp
using PersonApi.Models;

namespace PersonApi.Services
{
    public class PersonService
    {
        private static List<Person> persons = new List<Person>
        {
            new Person { Id = 1, Vorname = "Max", Nachname = "Mustermann", Alter = 50 },
            new Person { Id = 2, Vorname = "Erika", Nachname = "Musterfrau", Alter = 45 },
            new Person { Id = 3, Vorname = "Felix", Nachname = "Mustersohn", Alter = 20 }
        };

        public List<Person> GetPersons()
        {
            return persons;
        }
    }
}
```


## 6. Erstellung des Controllers
Erstellen Sie die Datei `Controllers/PersonController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using PersonApi.Services;
using PersonApi.Models;

namespace PersonApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase {

        private readonly PersonService _personService = new PersonService();

        [HttpGet]
        public ActionResult<List<Person>> Get() {
            return _personService.GetPersons();
        }
    }
}
```


## 7. Bearbeitung der Datei Program.cs
Aktualisieren Sie die Datei `Program.cs` wie folgt. Mit dieser Änderung wird automatisch auf die Swagger-Seite weitergeleitet, wenn die Haupt-URL `http://localhost:5001/` aufgerufen wird:

```csharp
var builder = WebApplication.CreateBuilder(args);

// EINTRAGEN - CORS-Konfiguration hinzufügen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
                // zulässige Domains:
                .AllowAnyOrigin() // Erlaubt den Zugriff von allen Domains. // Dies bedeutet, dass auf die API von jeder Domain aus zugegriffen werden kann.
                //.WithOrigins("https://www.ornek1.com", "https://www.ornek2.com") // Sadece belirtilen domainlere izin ver

                // zulässige Methoden:
                .AllowAnyMethod() // Erlaubt alle HTTP-Methoden (z.B. GET, POST, PUT, DELETE). // Dies bedeutet, dass alle HTTP-Anfragen an die API akzeptiert werden.
                //.WithMethods("GET", "POST") // Sadece GET ve POST metodlarına izin ver

                // zulässige Anfragen:
                .AllowAnyHeader() // Erlaubt alle HTTP-Header. // Diese Einstellung ermöglicht es, jeder Anfrage an die API beliebige Header hinzuzufügen.
                //.WithHeaders("Content-Type", "Authorization"); // Sadece belirtilen başlıklara izin ver
                ;

    });
});

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
```


### WithHeaders - häufig verwendete Header

Je nach Bedarf können folgende häufig verwendete Header erlaubt werden:

```` csharp
.WithHeaders("Accept", "Accept-Language", "Cache-Control", "User-Agent", "Origin", "X-Requested-With", "Authorization", "Content-Type", "Cookie", "Referer");
````
Erläuterungen:

- `Accept` : Gibt an, welchen Inhaltstyp der Client akzeptieren kann.
- `Accept-Language` : Gibt die vom Client bevorzugten Spracheinstellungen an.
- `Cache-Control` : Gibt die Caching-Politik an.
- `User-Agent` : Gibt Informationen über den anfragenden Client an.
- `Origin` : Gibt die Domain an, von der die Anfrage stammt.
- `X-Requested-With` : Definiert AJAX-Anfragen.
- `Authorization` : Authentifizierungs-Header.
- `Content-Type` : Gibt den Typ des gesendeten Inhalts an.
- `Cookie` : Enthält Cookie-Informationen.
- `Referer` : Gibt an, von welcher Seite die Anfrage stammt



## 8. Projekt kompilieren und testen
Führen Sie das Projekt im Terminal aus:

```bash
dotnet run
```

Testen Sie die API, indem Sie zu `http://localhost:5001/swagger/index.html` navigieren. Wenn Sie jetzt `http://localhost:5001/` aufrufen, werden Sie automatisch auf die Swagger-Seite weitergeleitet.


## 9. Erstellung der Dockerfile
Erstellen Sie im Projektstammverzeichnis eine Datei namens `Dockerfile` und fügen Sie folgenden Inhalt hinzu:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./*.csproj ./
RUN dotnet restore "./PersonApi.csproj"

# Kopieren Sie den gesamten Quellcode und erstellen Sie das Projekt
COPY . ./
RUN dotnet build "./PersonApi.csproj" -c Release -o /app/build

# Bereitstellen der Veröffentlichungsversion
FROM build AS publish
RUN dotnet publish "./PersonApi.csproj" -c Release -o /app/publish

# Erstellen des finalen Images und Festlegen des Arbeitsverzeichnisses
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PersonApi.dll"]
```

----

## 10. Docker-Image erstellen und ausführen

Erstellen Sie das Docker-Image im Terminal:

```bash
# Docker-Image erstellen
docker build -t personapi .
```

Starten Sie den Docker-Container und setzen Sie die Umgebung auf "Development":

```bash
# Docker-Container starten und Umgebung auf Development setzen
docker run -d -p 5001:80 --name personapi_container -e ASPNETCORE_ENVIRONMENT=Development personapi
```

URL zum Testen  [http://localhost:5001/](http://localhost:5001/)


Dieser Befehl setzt die Umgebung auf Development und aktiviert die Swagger-Oberfläche.

Hinweis: Falls bereits ein Container mit demselben Namen (`personapi_container`) läuft, müssen Sie diesen Container stoppen und löschen, um Namenskonflikte zu vermeiden. Verwenden Sie die folgenden Befehle, um den vorhandenen Container zu stoppen und zu löschen:

```bash
docker stop personapi_container
docker rm personapi_container
```

Swagger-Zugriff testen:

Nachdem der Container erfolgreich gestartet wurde, überprüfen Sie den Zugriff auf die Swagger-Oberfläche, indem Sie zu [http://localhost:5001/swagger/index.html](http://localhost:5001/swagger/index.html) navigieren.

----

# Die nächsten Schritte sind optional 

----

## 11. Code-Updates im Docker-Container anwenden
Falls Sie eine Aktualisierung in Ihrem Projekt vornehmen müssen und diese im Docker-Container anwenden möchten, folgen Sie den folgenden Schritten:

1. **Nehmen Sie die erforderlichen Code-Updates vor**: Bearbeiten Sie die Projektdateien und schließen Sie die Änderungen ab.

2. **Erstellen Sie ein neues Docker-Image**: Um die neuen Codes widerzuspiegeln, müssen Sie das Docker-Image erneut erstellen. Verwenden Sie dazu den bereits verwendeten Befehl `docker build`:

```` bash
    # Neues Docker-Image erstellen
    docker build -t personapi .
````

Dieser Befehl wird Ihr vorhandenes Docker-Image mit dem aktualisierten Code neu erstellen.


3. **Stoppen und löschen Sie den alten Container**: Stoppen und löschen Sie den vorhandenen Container.

```bash
    # Alten Container stoppen und löschen
    docker stop personapi_container
    docker rm personapi_container
```

4. **Erstellen Sie einen neuen Container**: Starten Sie den Container mit dem neuen Image neu.

```bash
   # Neuen Container erstellen
   docker run -d -p 5001:80 --name personapi_container -e ASPNETCORE_ENVIRONMENT=Development personapi
```

Dieser Schritt startet einen aktualisierten Container mit dem neuen Docker-Image.

Mit diesen Schritten können Sie Ihr Projekt erfolgreich erstellen, mit Docker ausführen, aktualisieren und über Portainer verwalten.


-----

