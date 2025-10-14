using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Molit_IN.API.Data;
using QuestPDF.Infrastructure;
using Sap.Data.Hana;
using System.Data;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Licencia QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

// Variables de entorno y (solo Dev) secrets
builder.Configuration.AddEnvironmentVariables();
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Validaciones de cadenas
var sqlCs = builder.Configuration.GetConnectionString("DatabaseMolit");
if (string.IsNullOrWhiteSpace(sqlCs))
    throw new InvalidOperationException("⚠️ Falta ConnectionStrings:DatabaseMolit");

var hanaCs = builder.Configuration.GetConnectionString("hanaStrings");
if (string.IsNullOrWhiteSpace(hanaCs))
    throw new InvalidOperationException("⚠️ Falta ConnectionStrings:hanaStrings");

// DbContext (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(sqlCs));

// HANA por request (usar using al consumirla)
builder.Services.AddScoped<IDbConnection>(_ => new HanaConnection(hanaCs));

// Auth/JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Molit_IN.API",
            ValidAudience = "Molit_IN.Client",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("BO7YThbqh51BmXcAyReF806mMjHgMDik")
            ),
            RoleClaimType = ClaimTypes.Role // ← asegúrate que coincida con tu token
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese el token con el prefijo: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");     // ← aquí
app.UseAuthentication();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.Run();
