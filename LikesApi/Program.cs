using LikesApi.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
//builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("LikesDb"));
var connectString = builder.Configuration.GetConnectionString("LikesDb");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
