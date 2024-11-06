
// EINTRAGEN 
using Microsoft.EntityFrameworkCore;
using PersonApi.Data; // Fügen Sie den Namespace hinzu, in dem sich die DbContext-Klasse befindet.
using PersonApi.Services; // <-- EINTRAGEN 

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


// EINTRAGEN - Die Verbindungszeichenfolge wird aus der Datei appsettings.json entnommen.
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


// EINTRAGEN - Die aktualisierte Verbindungszeichenfolge wird dem DbContext hinzugefügt
builder.Services.AddDbContext<PersonDbContext>(options =>
    options.UseSqlServer(connStr)
);


// EINTRAGEN - Service ohne Interface
// builder.Services.AddScoped<PersonService>(); // Register PersonService

// EINTRAGEN - Service mit Interface
builder.Services.AddScoped<IPersonService, PersonService>(); // Register IPersonService und PersonService


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// EINTRAGEN - CORS'u etkinleştir
app.UseCors("AllowAll"); // Tüm domainler, yöntemler ve başlıklara izin ver
//app.UseCors("AllowSpecificMethods"); // Eger GET, POST, ... verilmis ise.
//app.UseCors("AllowSpecificOrigins"); // Sadece belirli domainlere izin ver
//app.UseCors("AllowSpecificMethodsAndHeaders"); // Belirli domainler, belirli başlıklar ve belirli HTTP yöntemlerine izin ver



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
