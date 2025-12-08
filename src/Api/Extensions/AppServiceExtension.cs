using Application.Interfaces.Generics;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Generics;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;  

namespace Api.Extensions;

public static class AppServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection") 
                               ?? config["DATABASE_URL"];
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string not found. Checked 'DefaultConnection' and 'DATABASE_URL'.");
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IRepository<Product>, Repository<Product>>(); 
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPasswordHash, PasswordHash>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<DbSeeder>();

        return services;
    }

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT desta maneira: Bearer {seu token}"
            });
            
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddJwtConfig(this IServiceCollection services, IConfiguration config)
    {
        var secretKey = config["JwtSettings:SecretKey"]
                        ?? throw new InvalidOperationException("JwtSettings:SecretKey is null");

        var keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
        var key = new SymmetricSecurityKey(keyBytes);

        services.AddAuthentication(x =>
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

        return services;
    }

    public static async Task UseDbSeeder(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var services = scope.ServiceProvider;

        try
        {
            var dbContext = services.GetRequiredService<AppDbContext>();

            if (dbContext.Database.IsRelational())
            {
                await dbContext.Database.MigrateAsync(); 
            }
            
            var seeder = services.GetRequiredService<DbSeeder>();
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }
    
    public static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration config)
    {
        var allowedOrigin = config["CorsSettings:AllowedOrigins"] 
                            ?? throw new InvalidOperationException("CorsSettings:AllowedOrigins is missing in appsettings.");

        services.AddCors(options =>
        {
            options.AddPolicy("WebUiPolicy", builder =>
            {
                builder
                    .WithOrigins(allowedOrigin)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }
}