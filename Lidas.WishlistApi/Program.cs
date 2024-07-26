using Lidas.WishlistApi;
using Lidas.WishlistApi.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("WislistDb"));

// Mapper
builder.Services.AddAutoMapper(typeof(AppDbContext));

// Validator
builder.Services.AddValidatorService();

// Authentication
builder.Services.AddAuthenticationService(builder.Configuration);

// Controller
builder.Services.AddControllers();

// Swagger
builder.Services.AddSwaggerService();

// MassTransit
builder.Services.AddRabbitMQService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
