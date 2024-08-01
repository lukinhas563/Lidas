using Lidas.WishlistApi;
using Lidas.WishlistApi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database
//services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("WislistDb"));
var connectString = builder.Configuration.GetConnectionString("WishlistDb");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectString));

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

// Request
builder.Services.AddRequestService();

// Cors
var corsPolicy = "MyPolicy";
builder.Services.AddCorsPolicyService(corsPolicy);

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
