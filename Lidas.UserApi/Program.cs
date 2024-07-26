using Lidas.UserApi;
using Lidas.UserApi.Config;
using Lidas.UserApi.Interfaces;
using Lidas.UserApi.Mapper;
using Lidas.UserApi.Persist;
using Lidas.UserApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database
//builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("UserDb"));
var connectString = builder.Configuration.GetConnectionString("UserDb");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectString));

// Mapper
builder.Services.AddAutoMapper(typeof(AppMapper));

// Validator
builder.Services.AddValidatorsService();

// Token settings
builder.Services.AddTokenService(builder.Configuration);

// Hash
builder.Services.AddCryptographyService();

// Email settings
builder.Services.AddEmailService(builder.Configuration);

// Cors
string corsPolicy = "MyPolicy";
builder.Services.AddCorsService(corsPolicy);

// Authentication
builder.Services.AddAuthenticationService(builder.Configuration);

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Lidas Users API",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    var xmlFile = "Lidas.UserApi.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(corsPolicy);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
