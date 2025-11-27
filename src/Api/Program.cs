using Application.Interfaces;
using Application.Services;
using Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, configuration) =>
    configuration
        .WriteTo.Console() 
        .WriteTo.File(
            path: "logs/api-log-.txt", 
            rollingInterval: RollingInterval.Day, 
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .MinimumLevel.Information());

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();

builder.Services.AddAutoMapper(typeof(Application.Mappings.MappingProfile));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT desta maneira: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

var secretKey = builder.Configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JwtSettings:SecretKey is null");

var keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);

var key = new SymmetricSecurityKey(keyBytes);

builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true; 

        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = key,

            ValidateLifetime = true,

            ValidateIssuer = false,
            
            ValidateAudience = false
        };
    });

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPasswordHash, PasswordHash>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AppDbContext>();

var app = builder.Build();

app.UseMiddleware<Api.Middlewares.GlobalErrorHandlerMiddleware>();

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

public partial class Program { }