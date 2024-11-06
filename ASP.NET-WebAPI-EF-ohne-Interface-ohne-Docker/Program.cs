
// EINTRAGEN 
using Microsoft.EntityFrameworkCore;
using PersonApi.Data; // Namespace der DbContext-Klasse hinzufügen.


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


// EINTRAGEN - Die Verbindungszeichenfolge aus appsettings.json hinzufügen und DbContext einfügen
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
