using FluentValidation;
using Lidas.MangaApi.Config;
using Lidas.MangaApi.Interfaces;
using Lidas.MangaApi.Mapper;
using Lidas.MangaApi.Persist;
using Lidas.MangaApi.Services;
using Lidas.MangaApi.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectString = builder.Configuration.GetConnectionString("LidasCs");
//builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("LidasDb"));
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectString));
builder.Services.AddAutoMapper(typeof(AppMapper));

// Validator
builder.Services.AddValidatorsFromAssemblyContaining<MangaValidator>();
builder.Services.AddScoped<IValidatorService, Validator>();

// Provider
var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings");
builder.Services.Configure<CloudinarySettings>(cloudinarySettings);
builder.Services.AddScoped<IProvider, ImageProvider>();

// Cors
string corsPolicy = "MyPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy, policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// Authentication
var token = builder.Configuration.GetSection("JWT").Get<TokenSettings>();
var key = Encoding.ASCII.GetBytes(token.Key);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});


builder.Services.AddControllers()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Lidas Manga API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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

    var xmlFile = "Lidas.MangaApi.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);
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
