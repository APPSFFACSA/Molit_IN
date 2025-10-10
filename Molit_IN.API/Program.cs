using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Molit_IN.API.Data;
using Sap.Data.Hana;
using System.Data;
using QuestPDF.Infrastructure;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Configura la licencia de QuestPDF antes de usarla
QuestPDF.Settings.License = LicenseType.Community;


// 2) Variables de entorno (¡esto sí se ejecuta en prod y prod es donde pones tus credenciales!)
builder.Configuration.AddEnvironmentVariables();

// 3) Sólo en Dev: User Secrets
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

Console.WriteLine($"1 ------ ENTORNO: {builder.Environment.EnvironmentName}");
foreach (var kv in builder.Configuration.AsEnumerable())
    Console.WriteLine($"y esto que es {kv.Key} = {kv.Value}");

var cs = builder.Configuration["ConnectionStrings:DatabaseMolit"];
Console.WriteLine($"2----- ConnectionStrings:DatabaseMolit = '{cs}'");
if (string.IsNullOrEmpty(cs))
    throw new InvalidOperationException(
      "3 ------⚠️ No se encontró ConnectionStrings:DatabaseMolit en la configuración");

// Configurar el DbContext usando la cadena de conexión del secrets.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseMolit"))
);
// Registrar la conexión de solo lectura
builder.Services.AddTransient<IDbConnection>(sp =>
    new HanaConnection(builder.Configuration.GetConnectionString("hanaStrings")));

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("BO7YThbqh51BmXcAyReF806mMjHgMDik")),

            // 👇 Elige UNA de estas según tu token:
            RoleClaimType = ClaimTypes.Role         // si pusiste ClaimTypes.Role
            // RoleClaimType = "role"               // si el claim se llama "role"
            // RoleClaimType = "roles"              // si usas "roles"
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(option =>
{
    option.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddControllers();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Definición del esquema Bearer (muestra el botón Authorize)
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
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();

app.UseCors("CorsPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
