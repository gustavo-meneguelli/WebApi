using Api.Extensions;
using Application.Extensions;
using Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging estruturado com Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File("logs/api-log-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .MinimumLevel.Information());

// Serviços Básicos
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Cache em memória para dados que mudam raramente (ex: categorias)
builder.Services.AddMemoryCache();



// ...

// Registro Direto das Camadas
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSwaggerConfig();
// builder.Services.AddJwtConfig Removido (agora dentro de Infrastructure)

// FluentValidation e AutoMapper agora são registrados em Application layer
// builder.Services.AddValidatorsFromAssemblyContaining... Removido
// builder.Services.AddAutoMapper... Removido

builder.Services.AddCorsConfig(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<Api.Middlewares.GlobalErrorHandlerMiddleware>();

app.UseMiddleware<Api.Middlewares.GlobalErrorHandlerMiddleware>();

// Enable Swagger in ALL environments (including Production/Render)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("WebUiPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health Check endpoint para Docker/Kubernetes
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithTags("Health")
    .AllowAnonymous();

//Check Seeder
await app.UseDbSeeder();

app.Run();
