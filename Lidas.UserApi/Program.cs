using Lidas.UserApi.Config;
using Lidas.UserApi.Mapper;
using Lidas.UserApi.Persist;
using Lidas.UserApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("UserDb"));
builder.Services.AddAutoMapper(typeof(AppMapper));

// Token settings
var tokenSettings = builder.Configuration.GetSection("JWT");
builder.Services.Configure<TokenSettings>(tokenSettings); 
builder.Services.AddSingleton<TokenService>();

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
