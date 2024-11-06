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
