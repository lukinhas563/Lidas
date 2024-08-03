using Lidas.MangaApi;
using Lidas.MangaApi.Mapper;
using Lidas.MangaApi.Persist;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("LidasDb"));
var connectString = builder.Configuration.GetConnectionString("LidasCs");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectString));

// Mapper
builder.Services.AddAutoMapper(typeof(AppMapper));

// Validator
builder.Services.AddValidatorsService();

// Provider
builder.Services.AddProviderService(builder.Configuration);

// Cors
string corsPolicy = "MyPolicy";
builder.Services.AddCorsService(corsPolicy);

// Authentication
builder.Services.AddAuthenticationService(builder.Configuration);

// MassTransit
builder.Services.AddMassTransitService(builder.Configuration);

// Request service
builder.Services.AddRequestService();

// Controller
builder.Services.AddControllers()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

// Swagger
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
