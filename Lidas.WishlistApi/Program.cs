using Lidas.WishlistApi;
using Lidas.WishlistApi.Config;
using Lidas.WishlistApi.Database;
using Lidas.WishlistApi.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("WislistDb"));

// Mapper
builder.Services.AddAutoMapper(typeof(AppDbContext));

// Validator
builder.Services.AddValidatorService();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MassTransit
//builder.Services.AddRabbitMQService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
