using Application.Common.Interfaces;
using Application.Features.Auth.Repositories;
using Application.Features.Carts.Repositories;
using Application.Features.Categories.Repositories;
using Application.Features.Orders.Repositories;
using Application.Features.ProductReviews.Repositories;
using Application.Features.Products.Repositories;
using Application.Features.Reviews.Repositories;
using Infrastructure.Data;
using Infrastructure.Generics;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = GetConnectionString(configuration);
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IProductReviewRepository, ProductReviewRepository>();

        // Infrastructure Services
        services.AddScoped<IProfanityFilterService, ProfanityFilterService>();
        services.AddScoped<IPasswordHash, PasswordHash>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<DbSeeder>(); // Seeder

        // JWT Authentication Config
        AddJwtAuthentication(services, configuration);

        return services;
    }

    private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IConfiguration>((options, config) =>
            {
                var secretKey = config["JwtSettings:SecretKey"]
                                ?? throw new InvalidOperationException("JwtSettings:SecretKey is null");

                var issuer = config["JwtSettings:Issuer"];
                var audience = config["JwtSettings:Audience"];

                var keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
                var key = new SymmetricSecurityKey(keyBytes);

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ValidateIssuer = !string.IsNullOrEmpty(issuer),
                    ValidIssuer = issuer,
                    ValidateAudience = !string.IsNullOrEmpty(audience),
                    ValidAudience = audience
                };
            });

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();
    }

    private static string GetConnectionString(IConfiguration configuration)
    {
        // 1. Try to get generic ConnectionString (Render pushes DATABASE_URL)
        var connectionString = configuration["DATABASE_URL"];

        // 2. If present, it might be a URI (postgres://) or a standard string.
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return BuildConnectionStringFromUrl(connectionString);
        }

        // 3. Fallback to appsettings.json (Local Development)
        // We strictly avoid "Data Source" here to prevent SQLite confusion in Production if DATABASE_URL is missing
        var defaultConn = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(defaultConn) && !defaultConn.Contains("Data Source="))
        {
            return defaultConn;
        }

        // 4. Ultimate Fallback (Local Docker/Dev)
        return "Host=localhost;Database=DarklynStore;Username=postgres;Password=postgres";
    }

    private static string BuildConnectionStringFromUrl(string url)
    {
        // Check if it's already a clean connection string (Key=Value) or a URI
        if (!url.StartsWith("postgres://") && !url.StartsWith("postgresql://"))
        {
            return url;
        }

        try 
        {
            var uri = new Uri(url);
            var userInfo = uri.UserInfo;
            var database = uri.AbsolutePath.TrimStart('/');
            
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Database = database,
                SslMode = SslMode.Prefer
            };

            if (!string.IsNullOrEmpty(userInfo))
            {
                var parts = userInfo.Split(':', 2);
                if (parts.Length > 0) builder.Username = parts[0];
                if (parts.Length > 1) builder.Password = parts[1];
            }

            return builder.ToString();
        }
        catch
        {
            // If parsing fails, return original and let Npgsql throw the specific error
            return url;
        }
    }
}
