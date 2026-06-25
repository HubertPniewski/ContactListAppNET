using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using ContactListApp.Models;
using dotenv.net;


// Load environment variables from .env file
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));


// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection"); // get connection string from appsettings.Development.json
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD"); // get the password to db from .env
if (!connectionString.EndsWith(";")) connectionString += ";";
connectionString += $"Password={dbPassword};"; // add the password to the connectionString
builder.Services.AddDbContext<ContactsContext>(opt => opt.UseNpgsql(connectionString)); // DB context

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    // migration of database while starting app
    var db = scope.ServiceProvider.GetRequiredService<ContactsContext>();
    db.Database.Migrate();
}

app.Run();
