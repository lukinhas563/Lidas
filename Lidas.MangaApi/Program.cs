using FluentValidation;
using Lidas.MangaApi.Config;
using Lidas.MangaApi.Mapper;
using Lidas.MangaApi.Persist;
using Lidas.MangaApi.Services;
using Lidas.MangaApi.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectString = builder.Configuration.GetConnectionString("LidasCs");
//builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("LidasDb"));
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectString));
builder.Services.AddAutoMapper(typeof(AppMapper));
builder.Services.AddValidatorsFromAssemblyContaining<MangaValidator>();

// Provider
var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings");
builder.Services.Configure<CloudinarySettings>(cloudinarySettings);
builder.Services.AddSingleton<ImageProvider>();


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

    var xmlFile = "Lidas.MangaApi.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);
});

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

app.Run();
