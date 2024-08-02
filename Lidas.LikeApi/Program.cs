using Lidas.LikeApi;
using Lidas.LikeApi.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectString = builder.Configuration.GetConnectionString("LidasDb");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectString));

// MassTransit
builder.Services.AddMassTransitService(builder.Configuration);

// Cors
var corsPolicy = "MyPolicy";
builder.Services.AddCorsPolicyService(corsPolicy);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(corsPolicy);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
