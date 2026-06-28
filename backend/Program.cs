using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using ContactListApp.Models;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ContactListApp.Services;


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

// JWT configuration
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Get JwtSettings from appsettings.json
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["Secret"]; // get secret key from JwtSettings

    // set parameters for JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, 
        ValidateAudience = false, // false only for development purposes, set true for production
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)) // secret key from appsettings.json
    };
});

// Cross Origin configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => // allow all only for development and testing purposes, not for production!
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowAll"); // allow requests from any orgin, only for development and testing!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    // migration of database while starting app
    var db = scope.ServiceProvider.GetRequiredService<ContactsContext>();
    db.Database.Migrate();
}

app.Run();
