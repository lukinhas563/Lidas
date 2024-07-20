using FluentValidation;
using Lidas.UserApi.Config;
using Lidas.UserApi.Mapper;
using Lidas.UserApi.Persist;
using Lidas.UserApi.Services;
using Lidas.UserApi.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("UserDb"));
var connectString = builder.Configuration.GetConnectionString("UserDb");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectString));
builder.Services.AddAutoMapper(typeof(AppMapper));
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();
builder.Services.AddScoped<UserValidator>();

// Token settings
var tokenSettings = builder.Configuration.GetSection("JWT");
builder.Services.Configure<TokenSettings>(tokenSettings);
builder.Services.AddScoped<TokenService>();

// Hash
builder.Services.AddSingleton<CryptographyService>();

// Email settings
var emailSettings = builder.Configuration.GetSection("SMTP");
builder.Services.Configure<EmailSettings>(emailSettings);
builder.Services.AddSingleton<EmailService>();

// Authentication
var token = builder.Configuration.GetSection("JWT").Get<TokenSettings>();
var key = Encoding.ASCII.GetBytes(token.Key);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
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

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
